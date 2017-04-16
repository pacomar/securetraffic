using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureTraffic.Models
{
    public class InfoCloseVehicule
    {
        public int distancia { get; set; } = int.MaxValue;

        public int tiempo { get; set; } = int.MaxValue;

        public string direccionVehiculoLento { get; set; } = "";

        public string direccionVehiculoPropio { get; set; } = "";
    }
}
