using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace eacc
{
    [DataContract]
    public class Team
    {
        public Team(DataRow row)
        {
            this.Id = (int)row["TEA_ID"];
            this.Name = (string)row["TEA_NAME_CH"];
            this.Color = (string)row["TEA_COLOR_CH"];
            this.Score = (int)row["TEA_SCORE_NU"];
            this.Money = (int)row["TEA_MONEY_NU"];
            this.SecretKey = (string)row["TEA_SECRET_CH"];
        }


        public Team()
        {
            this.VehicleList = new List<Vehicle>();
            this.AchievmentList = new List<Achievement>();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public int Money { get; set; }
        
        public string SecretKey { get; set; }

        public List<Vehicle> VehicleList { get; set; }

        public List<Achievement> AchievmentList { get; set; }

        [DataMember(Name = "Achievment")]
        public List<int> AchievmentIdList { get { return AchievmentList.Select(a => a.Id).ToList(); }  }
        public string MagicHeader { get; set; }
    }
}
