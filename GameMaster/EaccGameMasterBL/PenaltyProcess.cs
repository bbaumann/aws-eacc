using com.eurosport.logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eacc.gamemaster.process
{
    public static class PenaltyProcess
    {
        public static void RemoveModule(Vehicle vehicle, ModuleType? moduleType = null)
        {
            Module toDel = null;
            if (moduleType.HasValue)
            {
                toDel = vehicle.Modules.Where(m => m.ModuleType == moduleType.Value).ToList().RandomElement();
            }
            else
            {
                toDel = vehicle.Modules.RandomElement();
            }
            vehicle.DeleteModule(toDel);
            if (!vehicle.CanBeDriven())
            {
                AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Ulysse31);
                VehicleProcess.Stop(vehicle);
            }
            else
            {
                int tmp = 0;
                int expected = 0;
                switch (toDel.ModuleType)
                {
                    case ModuleType.Pilot:
                        tmp = vehicle.RequestNbPilot();
                        expected = vehicle.NbPilots;
                        break;
                    case ModuleType.Weapon:
                        tmp = vehicle.RequestNbEngine();
                        expected = vehicle.NbWeapons;
                        break;
                    case ModuleType.Engine:
                        tmp = vehicle.RequestNbWeapon();
                        expected = vehicle.NbEngines;
                        break;
                    default:
                    case ModuleType.None:
                        break;
                }

                if (tmp != expected)
                {
                    PenaltyProcess.MissNextTurn(vehicle, 1);
                    //Envoie de la bonne valeur
                    if (toDel.ModuleType == ModuleType.Pilot)
                    {
                        vehicle.SendNbPilot();
                    }
                    else if (toDel.ModuleType == ModuleType.Engine)
                    {
                        vehicle.SendNbEngine();
                    }
                    else if (toDel.ModuleType == ModuleType.Weapon)
                    {
                        vehicle.SendNbWeapon();
                    }
                }
                else
                {
                    //check if module is "physically" deleted
                    if (toDel.ModuleType == ModuleType.Pilot)
                    {
                        int realPilots = vehicle.GetRealPilots();
                        if (realPilots != expected)
                        {
                            PenaltyProcess.MissNextTurn(vehicle, 1);
                            Log.Error("[Team #{0}] Vehicle {1} has been asked to remove a pilot module but its SQS queue is not up to date. Expected {2}, Got {3}", vehicle.TeamId, vehicle.Id, expected, realPilots);
                        }
                    }
                    else if (toDel.ModuleType == ModuleType.Engine)
                    {
                        List<Module> realEngines = vehicle.GetRealEngines();
                        if (realEngines.Any(e => e.Name.Equals(toDel.Name, StringComparison.OrdinalIgnoreCase)) 
                            || realEngines.Count != expected)
                        {
                            PenaltyProcess.MissNextTurn(vehicle, 1);
                            Log.Error("[Team #{0}] Vehicle {1} has been asked to remove the engine module {2} but either the file is still present or the count mismatch. Expected {2}, Got {3}", vehicle.TeamId, vehicle.Id, toDel.Name,expected, realEngines.Count);
                        }
                    }
                    else if (toDel.ModuleType == ModuleType.Weapon)
                    {
                        List<Module> realWeapons = vehicle.GetRealWeapons();
                        if (realWeapons.Any(e => e.Name.Equals(toDel.Name, StringComparison.OrdinalIgnoreCase))
                            || realWeapons.Count != expected)
                        {
                            PenaltyProcess.MissNextTurn(vehicle, 1);
                            Log.Error("[Team #{0}] Vehicle {1} has been asked to remove the weapon module {2} but either the item is still present or the count mismatch. Expected {2}, Got {3}", vehicle.TeamId, vehicle.Id, toDel.Name, expected, realWeapons.Count);
                        }
                    }
                }
            }       
        }

        public static void MissNextTurn(Vehicle vehicle, int value)
        {
            vehicle.TurnsToMiss = vehicle.TurnsToMiss + value;
            VehicleProcess.UpdateTurnsToMiss(vehicle);
        }

    }
}
