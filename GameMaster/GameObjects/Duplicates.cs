using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace eacc
{
    [DataContract]
    public class Duplicate
    {
        [DataMember]
        public int Square { get; set; }

        [DataMember]
        public List<int> Vehicles { get; set; }

        [DataMember]
        public bool isDifferentTeam { get; set; }

        public Duplicate(IEnumerable<Vehicle> vehicles)
        {
            this.Square = vehicles.First().Square;
            this.Vehicles = vehicles.Select(v => v.Id).ToList();
            this.isDifferentTeam = (vehicles.Select(v => v.TeamId).Distinct().Count() > 1);
        }
    }
}
