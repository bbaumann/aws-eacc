using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace eacc
{
    [DataContract]
    public class Achievement
    {
        public Achievement(DataRow row)
        {
            Id = (int)row["ACH_ID"];
            Name = (string)row["ACH_NAME_CH"];
            Description = (string)row["ACH_DESCR_CH"];
            Image = (string)row["ACH_IMG_CH"];
            PosReward = (int)row["ACH_POS_RWRD_NU"];
            EccReward = (int)row["ACH_ECC_RWRD_NU"];
        }

        public Achievement()
        {
            TeamList = new List<Team>();
        }


        public List<Team> TeamList { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember(Name = "Text")]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Image { get; set; }
        public int PosReward{ get; set; }
        public int EccReward{ get; set; }

    }
}