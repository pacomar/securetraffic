using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureTraffic.Models
{
    public class SettingsModel
    {
        public bool sonido { get; set; } = true;

        public bool imagen { get; set; } = true;

        public bool color { get; set; } = true;

        public SettingsModel(bool sonido, bool imagen, bool color)
        {
            this.sonido = sonido;
            this.imagen = imagen;
            this.color = color;
        }

        public SettingsModel()
        {
            this.sonido = true;
            this.imagen = true;
            this.color = true;
        }
    }
}
