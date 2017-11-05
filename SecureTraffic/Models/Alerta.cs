using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureTraffic.Models
{
    public class Alerta
    {
        public String identificador { get; set; }

        public Vehicle vehiculo { get; set; }

        public String distanciaTexto { get; set; }

        public int contador { get; set; }

        public int distancia { get; set; }

        public Alerta(String identificador, Vehicle vehiculo, String distanciaTexto, int contador, int distancia)
        {
            this.identificador = this.identificador;
            this.vehiculo = this.vehiculo;
            this.distanciaTexto = this.distanciaTexto;
            this.contador = contador;

            this.distancia = distancia;
        }
    }
}
