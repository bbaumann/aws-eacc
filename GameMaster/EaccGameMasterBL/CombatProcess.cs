using com.eurosport.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eacc.gamemaster.process
{
    public static class FightProcess
    {

        public static void CheckForFights()
        {
            List<Vehicle> allVehicles = VehicleProcess.GetAll(true);
            var dico = allVehicles.GroupBy<Vehicle, int>(v => v.Square);
            foreach (var item in dico)
            {
                if (item.Count() > 1)
                    ResolveFight(item.ToList());
            }
        }

        public static void ResolveFight(List<Vehicle> pVehicleList)
        {
            List<Vehicle> vehicleList = new List<Vehicle>(pVehicleList);

            while (vehicleList.Count > 1)
            {
                List<Vehicle> candidates = SelectTwoVehiclesToFight(vehicleList);
                if (candidates == null)
                {
                    return;
                }
                Vehicle firstVehicle = candidates[0];
                Vehicle secondVehicle = candidates[1];


                bool shouldThereBeFight = DoYouWantToFight(firstVehicle, secondVehicle);

                //On enlève les vaisseux (et les équipes s'il faut) des candidats au combat.
                vehicleList.Remove(firstVehicle);
                vehicleList.Remove(secondVehicle);

                if (shouldThereBeFight)
                {
                    DoFight(firstVehicle, secondVehicle);
                }
                else
                {
                    AchievmentProcess.AddAchievementToTeam(firstVehicle, AchievmentsEnum.Pacifiste);
                    AchievmentProcess.AddAchievementToTeam(secondVehicle, AchievmentsEnum.Pacifiste);
                }
            }
        }

        public static bool? TestFight(Vehicle yours, Vehicle pirate, string lambdaUrl)
        {
            Vehicle firstVehicle = yours;
            Vehicle secondVehicle = pirate;
            Log.Info("[Team #{0}] [Team #{1}] ============ NEW FIGHT ============", firstVehicle.TeamId, secondVehicle.TeamId);

            Fight fight = new Fight(firstVehicle, secondVehicle);
            fight.Fighters.Single(f => f.Vehicle.TeamId == yours.TeamId).IAUrl = lambdaUrl;
            List<Vehicle> invalidFighters = fight.GetInvalidFighters();
            if (invalidFighters.Count > 0)
            {
                foreach (var item in invalidFighters)
                {
                    Log.Info("[Team #{2}] [Team #{1}] Vehicle #{0} has no weapon, he is defeated", item.Id, firstVehicle.TeamId, secondVehicle.TeamId);
                    AchievmentProcess.AddAchievementToTeam(item, AchievmentsEnum.ClicClic);
                    VehicleProcess.Stop(item);
                }
            }
            else
            {
                Vehicle[] turns = new Vehicle[2] { firstVehicle, secondVehicle };
                foreach (var fighter in turns)
                {
                    if (fighter.NbWeapons >= 8)
                    {
                        AchievmentProcess.AddAchievementToTeam(fighter, AchievmentsEnum.Arméjusquauxdents);
                    }
                }
                int i = 0;
                //Qui commence?
                if (DateTime.Now.Millisecond % 2 == 0)
                {
                    i = 1;
                }
                Vehicle nextVehicle = null;
                while (!fight.IsWon() && !fight.IsDraw())
                {
                    i++;
                    nextVehicle = turns[i % 2];
                    int[] move = RequestMoveFromVehicle(nextVehicle, fight.Fighters);
                    Log.Debug("[Team #{0}][Team #{1}] Vehicle {2} plays {3}", turns[0].TeamId, turns[1].TeamId, nextVehicle.Id, "[" + String.Join(",", move) + "]");
                    bool isMoveOK = fight.Move(nextVehicle, move);
                    if (!isMoveOK)
                    {
                        Log.Info("[Team #{0}] Move is not Valid", nextVehicle.TeamId);
                        Log.Info("[Team #{1}] Vehicle {0} lost", nextVehicle.Id, nextVehicle.TeamId);
                        fight.Fighters.Single(v => v.VehicleId != nextVehicle.Id).IsWinner = true;
                    }
                }
                if (fight.IsWon())
                {
                    var winner = fight.GetWinner();
                    Log.Info("[Team #{2}] [Team #{1}] Vehicle {0} won!", winner.VehicleId, firstVehicle.TeamId, secondVehicle.TeamId);
                    var loser = fight.GetLoser();
                    if (winner.TeamId == 42)
                        return false;
                    //else
                    return true;
                }
                else if (fight.IsDraw())
                {
                    Log.Info("[Team #{2}] [Team #{1}] Draw!", firstVehicle.TeamId, secondVehicle.TeamId);
                }
            }
            return null;
        }

        public static void DoFight(Vehicle firstVehicle, Vehicle secondVehicle)
        {
            Log.Info("[Team #{0}] [Team #{1}] ============ NEW FIGHT ============", firstVehicle.TeamId, secondVehicle.TeamId);
            
            Fight fight = new Fight(firstVehicle,secondVehicle);
            List<Vehicle> invalidFighters = fight.GetInvalidFighters();
            if (invalidFighters.Count > 0)
            {
                foreach (var item in invalidFighters)
                {
                    Log.Info("[Team #{2}] [Team #{1}] Vehicle #{0} has no weapon, he is defeated", item.Id, firstVehicle.TeamId, secondVehicle.TeamId);
                    AchievmentProcess.AddAchievementToTeam(item, AchievmentsEnum.ClicClic);
                    VehicleProcess.Stop(item);
                }
            }
            else
            {
                Vehicle[] turns = new Vehicle[2] { firstVehicle, secondVehicle };
                foreach (var fighter in turns)
                {
                    if (fighter.NbWeapons >= 8)
                    {
                        AchievmentProcess.AddAchievementToTeam(fighter, AchievmentsEnum.Arméjusquauxdents);
                    }
                }
                int i = 0;
                //Qui commence?
                if (DateTime.Now.Millisecond % 2 == 0)
                {
                    i = 1;
                }
                Vehicle nextVehicle = null;
                while (!fight.IsWon() && !fight.IsDraw())
                {
                    i++;
                    nextVehicle = turns[i % 2];
                    int[] move = RequestMoveFromVehicle(nextVehicle, fight.Fighters);
                    bool isMoveOK = fight.Move(nextVehicle, move);
                    if (!isMoveOK)
                    {
                        Log.Info("[Team #{0}] Move is not Valid",nextVehicle.TeamId);
                        Log.Info("[Team #{1}] Vehicle {0} lost", nextVehicle.Id, nextVehicle.TeamId);
                        fight.Fighters.Single(v => v.VehicleId != nextVehicle.Id).IsWinner = true;
                    }
                }
                if (fight.IsWon())
                {
                    var winner = fight.GetWinner();
                    Log.Warn("[Team #{2}] [Team #{1}] Vehicle {0} won!", winner.VehicleId, firstVehicle.TeamId, secondVehicle.TeamId);
                    var loser = fight.GetLoser();
                    if (winner.Vehicle.NbWeapons + 3 <= loser.Vehicle.NbWeapons)
                    {
                        AchievmentProcess.AddAchievementToTeam(winner.Vehicle, AchievmentsEnum.Stratège);
                    }
                    if (!loser.IsPirate())
                    {
                        PenaltyProcess.RemoveModule(loser.Vehicle);
                        PenaltyProcess.MissNextTurn(loser.Vehicle, 1);
                    }
                    else
                    {
                        AchievmentProcess.AddAchievementToTeam(winner.Vehicle, AchievmentsEnum.MadMax);
                    }

                    winner.Vehicle.FightWon++;
                    VehicleProcess.UpdateFightWon(winner.Vehicle);

                }
                else if (fight.IsDraw())
                {
                    Log.Info("[Team #{2}] [Team #{1}] Draw!", firstVehicle.TeamId, secondVehicle.TeamId);
                }
            }
        }

        private static Random r = new Random();

        private static int[] RequestMoveFromVehicle(Vehicle vehicle, List<Fighter> fighters)
        {
            Fighter fighter = fighters.Single(f => f.VehicleId == vehicle.Id);
            Fighter other = fighters.Single(f => f.VehicleId != vehicle.Id);

            if (String.IsNullOrEmpty(fighter.IAUrl))
            {
                if (fighter.IsPirate())
                {
                    fighter.IAUrl = "https://g53zh5z4tj.execute-api.eu-central-1.amazonaws.com/prod/action";
                }
                else
                {
                    fighter.FillIAUrl();
                }
            }
            WebClient wc = new WebClient();
            string uri = String.Format("{0}?yourmoves={1}&othermoves={2}&cards={3}&othernbcards={4}", fighter.IAUrl, fighter.PrintMoves(), other.PrintMoves(), fighter.PrintCards(), other.NbCards);
            string res = wc.DownloadString(uri);
            JObject jObject = JObject.Parse(res);
            int[] array = new int[0];
            if (res.StartsWith("["))
            {
                array = jObject.ToObject<int[]>();
            }
            else
            {
                JToken jToken = jObject.GetValue("res");
                if (jToken != null)
                {
                    array = jToken.Values<int>().ToArray();
                }
            }
            return array;

        }

        private static List<Vehicle> SelectTwoVehiclesToFight(List<Vehicle> vehicleList)
        {
            if (vehicleList == null
               || vehicleList.Count <= 1
               )
            {
                Log.Error("Not enough vehicle to fight");
                return null;
            }

            Dictionary<int, List<Vehicle>> vehicleByTeamId = new Dictionary<int, List<Vehicle>>();
            foreach (var vehicle in vehicleList)
            {
                List<Vehicle> tmp = null;
                if (!vehicleByTeamId.TryGetValue(vehicle.TeamId, out tmp))
                {
                    tmp = new List<Vehicle>();
                    vehicleByTeamId[vehicle.TeamId] = tmp;
                }
                tmp.Add(vehicle);
            }

            if (vehicleByTeamId.Count <= 1)
            {
                Log.Error("Only 1 team remains to fight");
                return null;
            }
            var firstTeam = vehicleByTeamId.RandomElement();
            var secondTeam = firstTeam;
            int ntimes = 0;
            while (secondTeam.Equals(firstTeam) && ntimes < 100)
            {
                var tmp = vehicleByTeamId.RandomElement();
                if (!tmp.Equals(firstTeam))
                {
                    secondTeam = tmp;
                }
                ntimes++;
            }
            if (secondTeam.Equals(firstTeam))
            {
                Log.Warn("Could not select 2 teams to fight");
                return null;
            }

            return new List<Vehicle>()
            {
                firstTeam.Value.RandomElement(),
                secondTeam.Value.RandomElement()
            };
        }

        private static bool DoYouWantToFight(Vehicle firstVehicle, Vehicle secondVehicle)
        {
            bool shouldFight = false;
            //Demander aux deux véhicules s'ils veulent combattre
            shouldFight = DoYouWantToFightHelper(firstVehicle, secondVehicle) || shouldFight;
            shouldFight = DoYouWantToFightHelper(secondVehicle, firstVehicle) || shouldFight;

            return shouldFight;
        }

        private static bool DoYouWantToFightHelper(Vehicle attacker, Vehicle defender)
        {
            if (eacc.Properties.Settings.Default.UseMock)
            {
                return true;
            }
            bool shouldFight = false;
            WebClient wc = new WebClient();
            Uri baseUri = new Uri("http://"+attacker.Domain);
            Uri shouldFightUri = new Uri(baseUri, "/fight/shouldfight?TeamId=" + defender.TeamId.ToString() + "&VehicleId=" + defender.Id);
            try
            {
                string res = wc.DownloadString(shouldFightUri);
                if (res.Contains("true"))
                {
                    shouldFight = true;
                }
                else
                {
                    dynamic results = JsonConvert.DeserializeObject<dynamic>(res);
                    shouldFight = (results.Result == 1);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{2}] Could not call {0}, for Vehicle Id {1}", e, shouldFightUri, attacker.Id, attacker.TeamId);
                PenaltyProcess.MissNextTurn(attacker, 1);
            }
            return shouldFight;
        }

    }

}
