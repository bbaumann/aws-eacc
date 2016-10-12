using com.eurosport.data;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Util;
using eacc;
using System.Transactions;
using com.eurosport.logging;

namespace eacc.gamemaster.process
{
    public static class VehicleProcess
    {
        private static string PS_VEHICLE = "PS_VEHICLE";
        private static string PS_ALL_VEHICLES = "PS_ALL_VEHICLES";
        private static string PI_VEHICLE = "PI_VEHICLE";
        private static string PU_TURNS_TO_MISS = "PU_TURNS_TO_MISS";
        private static string PU_FIGHT_WON = "PU_FIGHT_WON";



        public static List<Vehicle> GetAll(bool onlyActive)
        {
            List<Vehicle> res = new List<Vehicle>();
            Query query = new Query();
            if (onlyActive)
            {
                query.CreateParameter("DEL_BL", 0);
            }
            DataRowCollection rows = query.GetRowsFromSP(Database.AFP, PS_ALL_VEHICLES);
            if (rows != null && rows.Count > 0)
            {
                int previousId = -1;
                List<DataRow> accu = new List<DataRow>();
                foreach (DataRow row in rows)
                {
                    int currentId = (int)row["VCL_ID"];
                    if (previousId != currentId)
                    {
                        if (previousId != -1)
                        {
                            res.Add(new Vehicle(accu));
                        }
                        accu = new List<DataRow>();
                        previousId = currentId;
                    }
                    accu.Add(row);
                }
                res.Add(new Vehicle(accu));
            }
            return res;
        }
        

        public static void Move(Vehicle vehicle, int nbSquares)
        {
            vehicle.Square += nbSquares;
            Save(vehicle);
            vehicle.SendSquare();

            int requestedSquare = vehicle.RequestSquare();
            if (requestedSquare != vehicle.Square)
            {
                PenaltyProcess.MissNextTurn(vehicle, 1);
                vehicle.SendSquare();
            }
        }

        public static void Save(Vehicle vehicle)
        {
            Query query = new Query();
            query.CreateParameter("VCL_DOMAIN_CH", vehicle.Domain);
            query.CreateParameter("VCL_NAME_CH", vehicle.Name);
            query.CreateParameter("VCL_SQUARE_NU", vehicle.Square);
            query.CreateParameter("TEA_ID", vehicle.TeamId);
            query.CreateParameter("DEL_BL", vehicle.IsAlive ? 0 : 1);
            query.CreateParameter("VCL_ID", (vehicle.Id == -1 ? DBNull.Value : (object)vehicle.Id));
            query.CreateParameter("VCL_TURNS_TO_MISS_NU", vehicle.TurnsToMiss);
            query.CreateParameter("VCL_FIGHT_WON_NU", vehicle.FightWon);
            query.CreateOutputParameter("@OUTPUT_VCL_ID", DbType.Int32);


            PrepareModulesQuery(query, vehicle.Modules);

            query.ExecuteProcedure(Database.AFP, PI_VEHICLE);
            vehicle.Id = (int)query.OutputParams["@OUTPUT_VCL_ID"].Value;

        }

        public static void UpdateTurnsToMiss(Vehicle vehicle)
        {
            if (vehicle.Id == -1)
                return;
            Query query = new Query();
            query.CreateParameter("VCL_ID", vehicle.Id);
            query.CreateParameter("VCL_TURNS_TO_MISS_NU", vehicle.TurnsToMiss);
            query.ExecuteProcedure(Database.AFP, PU_TURNS_TO_MISS);
        }

        public static void UpdateFightWon(Vehicle vehicle)
        {
            if (vehicle.Id == -1)
                return;
            Query query = new Query();
            query.CreateParameter("VCL_ID", vehicle.Id);
            query.CreateParameter("VCL_FIGHT_WON_NU", vehicle.FightWon);
            query.ExecuteProcedure(Database.AFP, PU_FIGHT_WON);
        }


        public static Vehicle Get(int id)
        {
            Vehicle res = null;
            Query query = new Query();
            query.CreateParameter("VCL_ID", id);
            DataSet set = query.GetDataSetFromSP(Database.AFP, PS_VEHICLE);
            if (set != null && set.Tables.Count == 2)
            {
                res = new Vehicle(set);
            }
            return res;
        }

        public static bool RegisterCar(string instance, string teamSecretKey)
        {
            //existence de l'instance 
            string domain = null;
            
            var teamId = TeamProcess.GetTeamIdFromSecretKey(teamSecretKey);
            domain = Vehicle.GetDomain(instance, teamId);

            if (teamId < 1)
                return false;

            Vehicle vehicle = new Vehicle()
            {
                Name = instance,
                Domain = domain,
                TeamId = teamId,
            };

            //On va chercher les fichiers/messages/ligne en bases pour finir la construction de l'objet véhicule
            if (vehicle.FillEngines() == -1
                || vehicle.FillPilots() == -1
                || vehicle.FillWeapons() == -1)
            {
                return false;
            }

            //Demander à l'instance son nb de modules et leur mode d'accès
            int nbengine = vehicle.RequestNbEngine();
            int nbpilot = vehicle.RequestNbPilot();
            int nbweapon = vehicle.RequestNbWeapon();

            Log.Debug("engine : {0}/{1}", vehicle.NbEngines, nbengine);
            Log.Debug("pilot : {0}/{1}", vehicle.NbPilots, nbpilot);
            Log.Debug("weapon : {0}/{1}", vehicle.NbWeapons, nbweapon);


            //Comparer au vrai nb de fichiers dans le S3/lignes dans la table Dynamodb etc.
            if (vehicle.NbEngines != nbengine
                || vehicle.NbPilots != nbpilot
                || vehicle.NbWeapons != nbweapon)
            {
                return false;
            }

            using (TransactionScope scope = new TransactionScope())
            {

                //Team pour le budget
                var team = TeamProcess.Get(teamId);

                if (!vehicle.CanBeRegistered(team.Money))
                {
                    return false;
                }

                Save(vehicle);
                team.Money -= vehicle.GetPrice();
                TeamProcess.UpdateTeam(team);
                scope.Complete();
            }
            return true;

        }

        public static void Stop(Vehicle vehicle)
        {
            vehicle.Stop();
            VehicleProcess.Save(vehicle);
        }


        #region Helpers
        private static void PrepareModulesQuery(Query query, List<Module> modules)
        {
            DataTable modulesDataTable = CreateAssociationsDataTable(modules);

            if (modulesDataTable != null)
                query.CreateParameter("@TVP_MODULES", modulesDataTable);
        }

        private static DataTable CreateAssociationsDataTable(List<Module> associations)
        {
            if (associations != null && associations.Count > 0)
            {
                TableValueParameter tvpDefinition = DataTableHelper.TVP_INT_INT_CHAR;

                List<object[]> values = new List<object[]>(associations.Count);
                foreach (Module assoc in associations)
                {
                    values.Add(CreateAssociationsDataTableRow(assoc));
                }

                return Query.CreateParameterDatatable(tvpDefinition, values.ToArray());
            }
            return null;
        }

        private static object[] CreateAssociationsDataTableRow(Module module)
        {
            object[] tempTab = new object[3];

            tempTab[0] = module.Id;
            tempTab[1] = module.ModuleType;
            tempTab[2] = module.Name;

            return tempTab;
        }

        #endregion

    }
}
