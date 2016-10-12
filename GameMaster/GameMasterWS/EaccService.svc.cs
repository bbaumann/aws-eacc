using eacc;
using eacc.gamemaster.process;
using System;
using System.Collections.Generic;
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
    public class EaccService : IEaccService
    {
        public GetAllDataResult GetAllData()
        {
            List<Vehicle> vehicles = VehicleProcess.GetAll(true);
            List<Team> teams = TeamProcess.GetAll(true);
            List<Achievement> achievements = AchievmentProcess.GetAllAchievments();


            List<Duplicate> duplicates = new List<Duplicate>();
            var dups = vehicles.GroupBy(v => v.Square);
            foreach (var item in dups)
            {
                if (item.Count() > 1)
                {
                    duplicates.Add(new Duplicate(item));
                }
            }
            return new GetAllDataResult()
            {
                Vehicles = vehicles,
                Teams = teams,
                Achievements = achievements,
                Duplicates = duplicates
            };
        }

        public GetAllDataResult GetTeamData()
        {
            var headers = OperationContext.Current.IncomingMessageProperties["httpRequest"];
            var secretKey = ((HttpRequestMessageProperty)headers).Headers["X-EACC-SECRET"];

            var teamId = TeamProcess.GetTeamIdFromSecretKey(secretKey);
            
            if (teamId == -1)
            {
                return null;
            }
            var res = getAllDataByTeamId(teamId);
            res.Achievements = new List<Achievement>();
            return res;
        }

        public GetAllDataResult GetAllDataByTeamId(string sTeamId)
        {
            var teamId = int.Parse(sTeamId);
            return getAllDataByTeamId(teamId);
        }

        private GetAllDataResult getAllDataByTeamId(int teamId)
        {

            List<Vehicle> vehicles = VehicleProcess.GetAll(true);
            List<Team> teams = TeamProcess.GetAll(true);
            foreach (var team in teams.Where(t => t.Id != teamId))
            {
                team.Score = -1;
                team.Money = -1;
            }
            List<Achievement> achievements = AchievmentProcess.GetAllAchievments();


            List<Duplicate> duplicates = new List<Duplicate>();
            var dups = vehicles.GroupBy(v => v.Square);
            foreach (var item in dups)
            {
                if (item.Count() > 1 && item.Any(v => v.TeamId == teamId))
                {
                    duplicates.Add(new Duplicate(item));
                }
            }
            return new GetAllDataResult()
            {
                Vehicles = vehicles.Where(v => v.TeamId == teamId || duplicates.Select(d => d.Square).Contains(v.Square)).ToList(),
                Teams = teams,
                Achievements = achievements,
                Duplicates = duplicates
            };
        }

        public GetRegisterCarResult RegisterCar(string instanceName)
        {
            var headers = OperationContext.Current.IncomingMessageProperties["httpRequest"];
            var secretKey = ((HttpRequestMessageProperty)headers).Headers["X-EACC-SECRET"];
            return new GetRegisterCarResult
            {
                IsValid = VehicleProcess.RegisterCar(instanceName, secretKey)
            };
        }

        public void Options()
        {
            return;
        }
    }
}
