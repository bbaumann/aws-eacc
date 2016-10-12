using eacc;
using eacc.gamemaster.process;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace eacc.gamemaster.ws
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class TestService : ITestService
    {
        private List<string> ReadFileAndFetchStringInSingleLine(string file)
        {
            List<string> res = new List<string>();
            try
            {
                using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BufferedStream bs = new BufferedStream(fs))
                    {
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            string str;
                            while ((str = sr.ReadLine()) != null)
                            {
                                res.Add(str);
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public List<string> GetLogs(string teamId)
        {
            int tId = int.Parse(teamId);
            if (tId == 41)
            {
                return ReadFileAndFetchStringInSingleLine(@"c:\tmp\Team41.log");
            }
            else if (tId == 40)
            {
                return ReadFileAndFetchStringInSingleLine(@"c:\tmp\Team40.log");
            }
            return ReadFileAndFetchStringInSingleLine(@"c:\tmp\Team39.log");
        }

        public string TestFight(string lambdaUrl, string nbweapon, string othernbweapon)
        {
            Vehicle yours = Vehicle.CreateOne(-1);
            yours.Modules.Clear();
            yours.Modules.Add(Module.CreateOne(ModuleType.Pilot));
            yours.Modules.Add(Module.CreateOne(ModuleType.Engine));
            for (int i = 0; i < int.Parse(nbweapon); i++)
            {
                yours.Modules.Add(Module.CreateOne(ModuleType.Weapon));
            }
            if (lambdaUrl.Equals("https://7j3jqr5l7i.execute-api.eu-central-1.amazonaws.com/prod/action"))
            {
                yours.TeamId = 41;
            }
            else if (lambdaUrl.Equals("https://muldmr3tzh.execute-api.eu-central-1.amazonaws.com/prod/action"))
            {
                yours.TeamId = 40;
            }
            else
            {
                yours.TeamId = 39;
            }

            Vehicle pirate = Vehicle.CreateOne();
            pirate.Modules.Clear();
            pirate.Modules.Add(Module.CreateOne(ModuleType.Pilot));
            pirate.Modules.Add(Module.CreateOne(ModuleType.Engine));
            for (int i = 0; i < int.Parse(othernbweapon); i++)
            {
                pirate.Modules.Add(Module.CreateOne(ModuleType.Weapon));
            }
            pirate.TeamId = 42;

            var res = FightProcess.TestFight(yours, pirate, lambdaUrl);
            
            if (res.HasValue)
            {
                if (res.Value)
                    return "You win!";
                //else
                return "Pirates win!";
            }
            return "Draw";
        }
    }
}
