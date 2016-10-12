using com.eurosport.logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eacc.gamemaster.process
{
    public static class RaceProcess
    {
        #region Race

        public static void DoRace()
        {
            Log.Debug("Do Race");
            List<Vehicle> vehicles = VehicleProcess.GetAll(true);
            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle.TurnsToMiss > 0)
                {
                    PenaltyProcess.MissNextTurn(vehicle, -1);
                }
                else
                {
                    if (vehicle.NbEngines <= 0)
                    {
                        AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.PanneEssence);
                        VehicleProcess.Stop(vehicle);
                    }
                    else
                    {
                        int tmp = vehicle.RequestNbEngine();
                        if (tmp == vehicle.NbEngines)
                        {
                            VehicleProcess.Move(vehicle, tmp);
                            if(tmp >= 8)
                                AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Rapidecommeléclair);
                        }
                        else
                        {
                            PenaltyProcess.MissNextTurn(vehicle, 1);
                            vehicle.SendNbEngine();
                        }
                    }
                }
            }
            //les vehicules ont bougé, on a vérifié leur position
            FightProcess.CheckForFights();
            ComputeAndStoreScore();
        }

        public static void ComputeAndStoreScore()
        {
            var teams = TeamProcess.GetAll();
            foreach (var team in teams)
            {
                int score = TeamProcess.ComputeScore(team.Id);
                team.Score = score;
                TeamProcess.UpdateTeam(team);
            }
            
        }

        #endregion

        #region Pirates

        public static void DoPirates(Vehicle vehicle)
        {
            DoPirates(new List<Vehicle>() { vehicle });
        }

        public static void DoPirates(List<Vehicle> vehicles)
        {
            foreach (var vehicle in vehicles)
            {
                var pirate = Vehicle.CreateOne(-1);
                pirate.TeamId = 42;
                pirate.Name = "Flying Dutchman";
                pirate.Square = vehicle.Square;

                FightProcess.DoFight(pirate, vehicle);
            }
        }

        #endregion

        #region Mined Zone

        private const int MINED_ZONE_EXPLOSION_RATE = 80;
        private static Random r = new Random();

        public static void DoMinedZone(Vehicle vehicle)
        {
            DoMinedZone(new List<Vehicle>() { vehicle });
        }

        public static void DoMinedZone(List<Vehicle> vehicles)
        {
            foreach (var vehicle in vehicles)
            {
                if (vehicle.TurnsToMiss == 0)
                {
                    bool? doesGoThrough = vehicle.RequestMinedZoneAction();
                    if (doesGoThrough.HasValue && doesGoThrough.Value)
                    {
                        int dice = r.Next() % 101;
                        if (dice < MINED_ZONE_EXPLOSION_RATE)
                        {
                            PenaltyProcess.RemoveModule(vehicle);
                        }
                        else
                        {
                            AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Asduvolant);
                        }
                    }
                    else
                    {
                        PenaltyProcess.MissNextTurn(vehicle, 3);
                    }
                }
            }
        }

        #endregion

        #region Wreck Exploring

        private const int WRECK_NOTHING_RATE = 50;
        private const int WRECK_ENGINE_RATE = 75;
        private const int WRECK_WEAPON_RATE = 90;
        private const int WRECK_PILOT_RATE = 98;
        private const int WRECK_EXPLOSION_RATE = 100;

        public static void DoExploreWreck(Vehicle vehicle)
        {
            DoExploreWreck(new List<Vehicle>() { vehicle });
        }

        public static void DoExploreWreck(List<Vehicle> vehicles)
        {
            foreach (var vehicle in vehicles)
            {
                if (vehicle.TurnsToMiss == 0)
                {
                    bool? shouldExplore = vehicle.RequestExploreWreckAction();
                    if (!shouldExplore.HasValue)
                    {
                        PenaltyProcess.MissNextTurn(vehicle, 1);
                    }
                    else
                    {
                        if (shouldExplore.Value)
                        {
                            Log.Info("[Team #{0}] Vehicle {1} explores a wreck.", vehicle.TeamId, vehicle.Id);
                            PenaltyProcess.MissNextTurn(vehicle, 1);
                            var dice = r.Next() % 101;
                            if (dice <= WRECK_NOTHING_RATE)
                            {
                                Log.Info("[Team #{0}] Vehicle {1} finds nothing.", vehicle.TeamId, vehicle.Id);
                            }
                            else if (dice <= WRECK_ENGINE_RATE)
                            {
                                Log.Info("[Team #{0}] Vehicle {1} finds an engine module.", vehicle.TeamId, vehicle.Id);
                                if (vehicle.SendEngine() != null)
                                {
                                    VehicleProcess.Save(vehicle);
                                    AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Trisélectif);
                                }
                            }
                            else if (dice <= WRECK_WEAPON_RATE)
                            {
                                Log.Info("[Team #{0}] Vehicle {1} finds a weapon module.", vehicle.TeamId, vehicle.Id);
                                if (vehicle.SendWeapon() != null)
                                {
                                    VehicleProcess.Save(vehicle);
                                    AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Trisélectif);
                                }
                            }
                            else if (dice <= WRECK_PILOT_RATE)
                            {
                                Log.Info("[Team #{0}] Vehicle {1} finds survivors and a pilot module .", vehicle.TeamId, vehicle.Id);
                                if (vehicle.SendPilot() != null)
                                {
                                    VehicleProcess.Save(vehicle);
                                    AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Trisélectif);
                                }
                            }
                            else if (dice <= WRECK_EXPLOSION_RATE)
                            {
                                Log.Info("[Team #{0}] Vehicle {1} finds nothing but snakes. Pilot is stung to death. Vehicle loses a pilot module.", vehicle.TeamId, vehicle.Id);
                                AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Snakesonacar);
                                PenaltyProcess.RemoveModule(vehicle, ModuleType.Pilot);
                            }
                        }
                        else
                        {
                            Log.Info("[Team #{0}] Vehicle {1} refuses to explore a wreck.", vehicle.TeamId, vehicle.Id);
                            PenaltyProcess.RemoveModule(vehicle, ModuleType.Pilot);
                        }
                    }
                }
            }
        }

        #endregion

        #region Garage

        public static void DoGarage(Vehicle vehicle)
        {
            DoGarage(new List<Vehicle>() { vehicle });
        }

        public static void DoGarage(List<Vehicle> vehicles)
        {
            foreach (var vehicle in vehicles)
            {
                Log.Info("[Team #{0}] Vehicle {1} goes to the garage.", vehicle.TeamId, vehicle.Id);
                ModuleType? whatToBuy = vehicle.RequestGarageAction();
                if (!whatToBuy.HasValue)
                {
                    PenaltyProcess.MissNextTurn(vehicle, 1);
                }
                else
                {
                    Team team = TeamProcess.Get(vehicle.TeamId);
                    int price = Module.GetCost(whatToBuy.Value);
                    if (price > team.Money)
                    {
                        Log.Info("[Team #{0}] Vehicle {1} tries to buy a module without enough money.", vehicle.TeamId, vehicle.Id);
                    }
                    else
                    {
                        switch (whatToBuy.Value)
                        {
                            case ModuleType.Pilot:
                                PenaltyProcess.MissNextTurn(vehicle, 1);
                                Log.Info("[Team #{0}] Vehicle {1} buys a pilot module.", vehicle.TeamId, vehicle.Id);
                                if (vehicle.SendPilot() != null)
                                {
                                    VehicleProcess.Save(vehicle);
                                    AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Pimpmyride);
                                }
                                break;
                            case ModuleType.Weapon:
                                PenaltyProcess.MissNextTurn(vehicle, 1);
                                Log.Info("[Team #{0}] Vehicle {1} buys a pilot module.", vehicle.TeamId, vehicle.Id);
                                if (vehicle.SendWeapon() != null)
                                {
                                    VehicleProcess.Save(vehicle);
                                    AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Pimpmyride);
                                }
                                break;
                            case ModuleType.Engine:
                                PenaltyProcess.MissNextTurn(vehicle, 1);
                                Log.Info("[Team #{0}] Vehicle {1} buys a pilot module.", vehicle.TeamId, vehicle.Id);
                                if (vehicle.SendEngine() != null)
                                {
                                    VehicleProcess.Save(vehicle);
                                    AchievmentProcess.AddAchievementToTeam(vehicle, AchievmentsEnum.Pimpmyride);
                                }
                                break;
                            case ModuleType.None:
                            default:
                                Log.Info("[Team #{0}] Vehicle {1} buys nothing.", vehicle.TeamId, vehicle.Id);
                                break;
                        }
                        if (price > 0)
                        {
                            team.Money -= price;
                            TeamProcess.UpdateTeam(team);
                        }

                    }
                }
            }
        }


        #endregion
    }
}
