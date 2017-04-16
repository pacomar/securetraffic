using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureTraffic.Models
{
    public class InfoCloseVehicule
    {
        public int distance { get; set; } = int.MaxValue;

        public int time { get; set; } = int.MaxValue;

        public string adressSlowVehicule { get; set; } = "";

        public string adressMyVehicule { get; set; } = "";
    }
}
