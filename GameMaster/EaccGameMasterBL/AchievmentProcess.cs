using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.eurosport.data;

namespace eacc.gamemaster.process
{
    public static class AchievmentProcess
    {

        public static string PI_ACH_TEA = "PI_ACH_TEA";
        public static string PS_ALL_ACHIEVMENTS = "PS_ALL_ACHIEVMENTS";
        public static string PS_ALL_ACH_TEA = "PS_ACHIEVEMENTS";

        public static List<Achievement> GetAllAchievments()
        {
            List<Achievement> res = new List<Achievement>();
            Query query = new Query();
            DataRowCollection rows = query.GetRowsFromSP(Database.AFP, PS_ALL_ACHIEVMENTS);
            if (rows != null && rows.Count > 0)
            {
                foreach (DataRow row in rows)
                {
                    res.Add(new Achievement(row));
                }
            }
            return res;
        }

        /// <summary>
        /// Ajout l'achievment et applique le bonus
        /// </summary>
        /// <param name="vehicule"></param>
        /// <param name="achEnum"></param>
        public static void AddAchievementToTeam(int teamId, AchievmentsEnum achEnum)
        {
            var achievmentId = (int)achEnum;

            //On vérifie que la team n'a pas déjà accompli l'ach
            Query query = new Query();
            query.CreateParameter("TEA_ID", teamId);
            DataRowCollection rows = query.GetRowsFromSP(Database.AFP, PS_ALL_ACH_TEA);
            if (rows != null && rows.Count > 0 &&
                rows.Cast<DataRow>().Any(row => (int)row["TEA_ID"] == teamId && (int)row["ACH_ID"] == achievmentId))
            {
                return;
            }

            query = new Query();
            //Link ach and team in db

            query.CreateParameter("ACH_ID", achievmentId);
            query.CreateParameter("TEA_ID", teamId);

            query.ExecuteProcedure(Database.AFP, PI_ACH_TEA);
            //apply team bonus
            var team = TeamProcess.Get(teamId);
            Achievement ach = GetAllAchievments().FirstOrDefault(a => a.Id == achievmentId);
            if (ach != null && ach.EccReward != 0)
                ApplyEccBonus(team, ach.EccReward);
        }

        /// <summary>
        /// Ajout l'achievment et applique le bonus
        /// </summary>
        /// <param name="vehicule"></param>
        /// <param name="achEnum"></param>
        public static void AddAchievementToTeam(Vehicle vehicule, AchievmentsEnum achEnum)
        {
            AddAchievementToTeam(vehicule.TeamId, achEnum);            
        }

        public static List<Achievement> GetAchievements(int teamId)
        {
            List<Achievement> res = new List<Achievement>();
            Query query = new Query();
            query.CreateParameter("TEA_ID", teamId);
            DataRowCollection rows = query.GetRowsFromSP(Database.AFP, PS_ALL_ACH_TEA);
            if (rows != null && rows.Count > 0)
            {
                foreach (DataRow row in rows)
                {
                    res.Add(new Achievement(row));
                }
            }
            return res;
        }

        private static void ApplyEccBonus(Team team, int bonus)
        {
            if (team != null)
            {
                team.Money += bonus;
                TeamProcess.UpdateTeam(team);
            }
        }
    }
}
