using com.eurosport.data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eacc.gamemaster.process
{
    public static class TeamProcess
    {
        private static string PS_ALL_TEAMS = "PS_ALL_TEAMS";
        private static string PU_TEAM = "PU_TEAM";
        private static string PS_HALL_OF_FAME = "PS_HALLOFFAME";

        public static List<Team> GetAll(bool withAchievements = false)
        {
            List<Team> res = new List<Team>();
            Query query = new Query();
            DataRowCollection rows = query.GetRowsFromSP(Database.AFP, PS_ALL_TEAMS);
            if (rows != null && rows.Count > 0)
            {
                foreach (DataRow row in rows)
                {
                    var team = new Team(row);
                    team.AchievmentList = AchievmentProcess.GetAchievements(team.Id);
                    res.Add(team);
                }
            }

            return res;
        }

       
        public static void UpdateTeam(Team team)
        {
            Query query = new Query();
            query.CreateParameter("TEA_ID", team.Id);
            query.CreateParameter("TEA_MONEY_NU", team.Money);
            query.CreateParameter("TEA_SCORE_NU", team.Score);
            query.ExecuteProcedure(Database.AFP, PU_TEAM);

            if (team.Money > 300)
            {
                AchievmentProcess.AddAchievementToTeam(team.Id, AchievmentsEnum.Picsou);
            }
        }

        public static Team Get(int id)
        {
            return GetAll().FirstOrDefault(t => t.Id == id);
        }


        public static int GetTeamIdFromSecretKey(string teamSecretKey)
        {
            var id = TeamProcess.GetAll().FirstOrDefault(t => t.SecretKey == teamSecretKey)?.Id;
            if (id != null)
                return (int)id;
            return -1;
        }

        public static int ComputeScore(int teamId)
        {
            var vehiclestmp = VehicleProcess.GetAll(false);

            var vehicles = vehiclestmp.Where(v => v.TeamId == teamId).ToList();

            int score = 0;
            foreach (var vehicle in vehicles)
            {
                score += vehicle.ComputeScore();
            }
            List<Achievement> achievements = AchievmentProcess.GetAchievements(teamId);
            foreach (var achievement in achievements)
            {
                score += achievement.PosReward;
            }

            Query query = new Query();
            var rows = query.GetRowsFromSP(Database.AFP, PS_HALL_OF_FAME);
            int maxDstVehicle = rows.Cast<DataRow>().Max(r => (int)r["MAX_DISTANCE"]);
            int maxDstTeam = rows.Cast<DataRow>().Max(r => (int)r["SUM_DISTANCE"]);
            int maxFightVehicle = rows.Cast<DataRow>().Max(r => (int)r["MAX_FIGHT"]);
            int maxFightTeam = rows.Cast<DataRow>().Max(r => (int)r["SUM_FIGHT"]);
            IEnumerable<int> teamMaxDstVehicle = rows.Cast<DataRow>().Where(r => (int)r["MAX_DISTANCE"] == maxDstVehicle).Select(r => (int)r["TEA_ID"]);
            IEnumerable<int> teamMaxDstTeam = rows.Cast<DataRow>().Where(r => (int)r["SUM_DISTANCE"] == maxDstTeam).Select(r => (int)r["TEA_ID"]);
            IEnumerable<int> teamMaxFightVehicle = rows.Cast<DataRow>().Where(r => (int)r["MAX_FIGHT"] == maxFightVehicle).Select(r => (int)r["TEA_ID"]);
            IEnumerable<int> teamMaxFightTeam = rows.Cast<DataRow>().Where(r => (int)r["SUM_FIGHT"] == maxFightTeam).Select(r => (int)r["TEA_ID"]);

            //if (teamMaxDstVehicle.Contains(teamId))
            //    score += 20;
            //if (teamMaxDstTeam.Contains(teamId))
            //    score += 20;
            //if (teamMaxFightVehicle.Contains(teamId))
            //    score += 20;
            //if (teamMaxFightTeam.Contains(teamId))
            //    score += 20;

            return score;
        }
    }
}
