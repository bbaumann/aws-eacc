using eacc;
using eacc.gamemaster.process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMasterTester
{
    class Program
    {
        static void Main(string[] args)
        {

            //TestAchievments();
            LetsGo();

            //TestRegisterCar();
            //CreateCars();
            //RaceTester();
            //RaceTester();
            //VechileTester();
            //CombatTester();
        }

        private static void TestAchievments()
        {
            var v = Vehicle.CreateOne();
            AchievmentProcess.AddAchievementToTeam(v, AchievmentsEnum.Rapidecommeléclair);
            AchievmentProcess.AddAchievementToTeam(v.TeamId, AchievmentsEnum.Arméjusquauxdents);
        }

        private static void LetsGo()
        {
            List<int> randomDistrib = new List<int>() { 50, 60, 70, 85, 100 };
            Random r = new Random();
            while (true)
            {
                RaceProcess.ComputeAndStoreScore();
                Console.WriteLine("===== EACC =====");
                Console.WriteLine("1 - Race");
                Console.WriteLine("2 - More Combat");
                Console.WriteLine("3 - Pirates!");
                Console.WriteLine("4 - Wreck Exploring");
                Console.WriteLine("5 - Mined Zone");
                Console.WriteLine("6 - Garage");
                Console.WriteLine("7 - Random Event");
                Console.WriteLine("8 - Print Random Event distribution");
                Console.WriteLine("9 - Update Event distribution");
                Console.WriteLine("99 - Exit");
                Console.WriteLine("================");
                int i = int.Parse(Console.ReadLine());
                switch (i)
                {
                    case 1:
                        RaceProcess.DoRace();
                        break;
                    case 2:
                        FightProcess.CheckForFights();
                        break;
                    case 3:
                        RaceProcess.DoPirates(VehicleProcess.GetAll(true));
                        break;
                    case 4:
                        RaceProcess.DoExploreWreck(VehicleProcess.GetAll(true));
                        break;
                    case 5:
                        RaceProcess.DoMinedZone(VehicleProcess.GetAll(true));
                        break;
                    case 6:
                        RaceProcess.DoGarage(VehicleProcess.GetAll(true));
                        break;
                    case 7:
                        var vehicles = VehicleProcess.GetAll(true);                        
                        foreach (var vehicle in vehicles)
                        {
                            int dice = r.Next() % randomDistrib.Last();
                            if (dice < randomDistrib[0])
                            {
                                //nothing
                            }
                            else if (dice < randomDistrib[1])
                            {
                                RaceProcess.DoPirates(new List<Vehicle>() { vehicle });
                            }
                            else if (dice < randomDistrib[2])
                            {
                                RaceProcess.DoGarage(new List<Vehicle>() { vehicle });
                            }
                            else if (dice < randomDistrib[3])
                            {
                                RaceProcess.DoExploreWreck(new List<Vehicle>() { vehicle });
                            }
                            else
                            {
                                RaceProcess.DoMinedZone(new List<Vehicle>() { vehicle });
                            }
                        }
                        break;
                    case 8:
                        Console.WriteLine("nothing,pirates,garage,wreck,minedZone,total");
                        Console.WriteLine(String.Join(",", randomDistrib.Select(j => j.ToString())));
                        break;
                    case 9:
                        randomDistrib.Clear();
                        for (int j = 0; j < 5; j++)
                        {
                            randomDistrib.Add(int.Parse(Console.ReadLine()));
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        private static void TestRegisterCar()
        {
            Vehicle v = Vehicle.CreateOne();
            v.Modules.Clear();
            v.FillPilots();
            v.FillEngines();
            v.FillWeapons();
        }

        private static void CreateCars()
        {
            for (int i = 0; i < 30; i++)
            {
                var toSave = Vehicle.CreateOne(-1);
                toSave.TeamId = i % 3 +1;
                VehicleProcess.Save(toSave);
            }
        }

        private static void RaceTester()
        {
            RaceProcess.DoRace();
        }

        private static void VechileTester()
        {
            var toSave = Vehicle.CreateOne(-1);
            VehicleProcess.Save(toSave);
            var vehicle = VehicleProcess.Get(toSave.Id);
        }

        private static void CombatTester()
        {
            //Deux vaisseaux de teams différentes
            CombatTesterTwoVehicles();
            //Deux vaisseaux de même team
            CombatTesterTwoVehiclesSameTeam();
            //5 vaisseaux de même team
            CombatTesterFiveVehiclesSameTeam();
            //3 vaisseeaux de team différentes
            CombatTesterThreeVehicles();
            //4 vaisseaux de team différentes
            CombatTesterFourVehicles();
        }

        private static void CombatTesterFourVehicles()
        {
            var fighters = new List<Vehicle>()
            {
                Vehicle.CreateOne(),
                Vehicle.CreateOne(),
                Vehicle.CreateOne(),
                Vehicle.CreateOne()
            };
            fighters[0].TeamId = 0;
            fighters[1].TeamId = 1;
            fighters[2].TeamId = 2;

            FightProcess.ResolveFight(fighters);
        }

        private static void CombatTesterThreeVehicles()
        {
            var fighters = new List<Vehicle>()
            {
                Vehicle.CreateOne(),
                Vehicle.CreateOne(),
                Vehicle.CreateOne()
            };
            fighters[0].TeamId = 0;
            fighters[1].TeamId = 1;
            fighters[2].TeamId = 2;

            FightProcess.ResolveFight(fighters);

        }

        private static void CombatTesterFiveVehiclesSameTeam()
        {
            var fighters = new List<Vehicle>()
            {
                Vehicle.CreateOne(),
                Vehicle.CreateOne(),
                Vehicle.CreateOne(),
                Vehicle.CreateOne(),
                Vehicle.CreateOne()
            };

            fighters.ForEach(v => v.TeamId = 1);

            FightProcess.ResolveFight(fighters);
        }

        private static void CombatTesterTwoVehiclesSameTeam()
        {
            var fighters = new List<Vehicle>()
            {
                Vehicle.CreateOne(),
                Vehicle.CreateOne()
            };

            fighters.ForEach(v => v.TeamId = 1);

            FightProcess.ResolveFight(fighters);
        }

        private static void CombatTesterTwoVehicles()
        {
            var v1 = Vehicle.CreateOne();
            var v2 = Vehicle.CreateOne();
            v2.TeamId = (v1.TeamId + 1) % 3;

            VehicleProcess.Save(v1);
            VehicleProcess.Save(v2);
            var fighters = new List<Vehicle>() { v1, v2 };

            FightProcess.ResolveFight(fighters);
        }
    }
}
