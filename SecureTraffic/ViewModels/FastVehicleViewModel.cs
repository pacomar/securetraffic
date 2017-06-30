using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;
using Firebase.Xamarin.Database;
using SecureTraffic.Models;
using System.Net.Http;
using System.Net;
using System.IO;
using Xamarin.Forms;
using Plugin.MediaManager;
using System.Linq;

namespace SecureTraffic
{
    public class FastVehicleViewModel
    {
        private Map _map { get; set; }
        private Position myPosition;
        private int alertDistance = 500;
        private int distancePosibleAlert = 1000;
        private Image imagen { get; set; }

        public FastVehicleViewModel(Map _map,Image imagen)
        {
            string rnd = new Random().Next(int.MinValue, int.MaxValue).ToString();
            var tokenGenerator = new Firebase.Xamarin.Token.TokenGenerator("zHGOXaynKRyC7QZqe1GWp30ZhWmhRP4qtnEorl3D");
            var authPayload = new Dictionary<string, object>()
            {
                {"uid", rnd.ToString()}
            };
            App.token = tokenGenerator.CreateToken(authPayload);

            this._map = _map;
            this.imagen = imagen;

			CenterMap();
        }

        /// <summary>
        /// Funcion que situa nuestra posicion en el mapa y llama a la funcion de actualizar marcadores
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CenterMap()
        {
            bool res = false;

            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
                if (position == null)
                    return res;

                myPosition = new Position(position.Latitude, position.Longitude);

                this._map.MoveToRegion(new MapSpan(new Position(position.Latitude, position.Longitude), 0.05, 0.05));

                res = true;

                UpdateMarkers();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
            }

            return res;
        }

        /// <summary>
        /// Funcion que actualiza los puntos del mapa, comprueba la distancia por coordendas, si esta muy cerca llama a la API de google y lanza aviso si es necesario
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateMarkers()
        {
            VehiclesService _vehServ = new VehiclesService();
            var vehicles = await _vehServ.GetVehicles();
			this._map.Pins.Clear();

            bool alertar = false;
            foreach (var vehicle in vehicles)
            {
                InfoCloseVehicule infoVehicle = new InfoCloseVehicule();

				if (CalculateDistanceLine(myPosition.Latitude, myPosition.Longitude, vehicle.Object.Coordinate.Latitude, vehicle.Object.Coordinate.Longitude) < distancePosibleAlert)
                {
                    infoVehicle = await this.GetInformationCloseVehicle(myPosition.Latitude, myPosition.Longitude, vehicle.Object.Coordinate.Latitude, vehicle.Object.Coordinate.Longitude);
                }

                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(vehicle.Object.Coordinate.Latitude, vehicle.Object.Coordinate.Longitude),
                    Label = vehicle.Object.Vehicle.ToString(),
                };
                this._map.Pins.Add(pin);

                bool lanzaraviso = ComprobarDistanciaYCarretera(infoVehicle);

                if (lanzaraviso)
                {
                    alertar = true;
                    Alertar();
                }
            }
            
            if (!alertar) PararAlertar();

            await Task.Delay(5000);
			CenterMap();

            return true;
        }

        public bool ComprobarDistanciaYCarretera(InfoCloseVehicule infoVehicle)
        {
            if (infoVehicle.distance < alertDistance)
            {
                List<string> listacarreteras = ListaCarreteras();
                bool esunacarretera = listacarreteras.Any(x => !infoVehicle.adressSlowVehicule.Split(',')[0].ToLower().Contains(x.ToLower()));
                bool esdeunicosentido = false;

                if (infoVehicle.adressSlowVehicule.Split(',')[0] == infoVehicle.adressMyVehicule.Split(',')[0] && esunacarretera)
                    {
                        return true;
                    }
            }
            return false;
        }

        public List<string> ListaCarreteras()
        {
            return new List<string>() { "alameda", "calle", "c/", "camino", "glorieta", "kalea", "pasaje", "paseo", "pº", "plaça", "plaza", "plza", "pza", "rambla", "ronda", "rua", "rúa", "sector", "av.", "calle", "travesía", "travesia", "urbanizacion", "urbanización", "avenida", "avda", "avinguda", "barrio", "bº", "calleja", "cami", "camí", "carrera", "cuesta", "edificio", "enparantza", "estrada", "jardines", "jardins", "parque", "passeig", "praza", "plazuela", "placeta", "poblado", "pbdo", "pd.", "travessera", "avinguda", "passatge", "bulevar", "ps.", "poligono", "polígono", "otros" };
        }

        /// <summary>
        /// Funcion que calcula la distancia en linea recta dadas 2 coordenadas (matemáticamente)
        /// </summary>
        /// <param name="fromLong">Longitud origen</param>
        /// <param name="fromLat">Latitud origen</param>
        /// <param name="toLong">Longitud destino</param>
        /// <param name="toLat">Latitud destino</param>
        /// <returns>Resultado en millas de la distancia</returns>
        private double CalculateDistanceLine(double fromLong, double fromLat,
                    double toLong, double toLat)
        {
            double d2r = Math.PI / 180;
            double dLong = (toLong - fromLong) * d2r;
            double dLat = (toLat - fromLat) * d2r;
            double a = Math.Pow(Math.Sin(dLat / 2.0), 2) + Math.Cos(fromLat * d2r)
                    * Math.Cos(toLat * d2r) * Math.Pow(Math.Sin(dLong / 2.0), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = 6367000 * c;
            return Math.Round(d);
        }

        /// <summary>
        /// Funcion que devuelve la informacion avanzada entre dos puntos
        /// </summary>
        /// <param name="fromLong">Longitud origen</param>
        /// <param name="fromLat">Latitud origen</param>
        /// <param name="toLong">Longitud destino</param>
        /// <param name="toLat">Latitud destino</param>
        /// <returns>Devuelve informacion de si estan en la misma carretera, distancia por carretera, tiempo</returns>
        private async Task<InfoCloseVehicule> GetInformationCloseVehicle(double fromLong, double fromLat,
                    double toLong, double toLat)
        {
            InfoCloseVehicule informacionVehiculo = new InfoCloseVehicule();
            var url = HttpWebRequest.Create("https://maps.googleapis.com/maps/api/directions/json?origin=" + fromLong.ToString().Replace(',', '.') + "," + fromLat.ToString().Replace(',', '.') + "&destination=" + toLong.ToString().Replace(',', '.') + "," + toLat.ToString().Replace(',', '.') + "&sensor=false");/*&key=AIzaSyAC25WcCdJIF5uvWLXMgGuYK4Y9sBZpJ34");*/

            url.ContentType = "application/json";
            url.Method = "GET";

            using (HttpWebResponse response = url.GetResponseAsync().Result as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    var content = reader.ReadToEnd();

                    var JSONObject = Newtonsoft.Json.Linq.JObject.Parse(content);

                    string distanciaText = JSONObject["routes"][0]["legs"][0]["distance"]["text"].ToString();
                    string tiempoText = JSONObject["routes"][0]["legs"][0]["duration"]["text"].ToString();
                    string distancia = JSONObject["routes"][0]["legs"][0]["distance"]["value"].ToString();
                    string tiempo = JSONObject["routes"][0]["legs"][0]["duration"]["value"].ToString();
                    string direccionVehiculo = JSONObject["routes"][0]["legs"][0]["end_address"].ToString();
                    string direccionPropia = JSONObject["routes"][0]["legs"][0]["start_address"].ToString();

                    informacionVehiculo.distance = Int32.Parse(distancia);
                    informacionVehiculo.time = Int32.Parse(tiempo);
                    informacionVehiculo.distanceText = distanciaText;
                    informacionVehiculo.timeText = tiempoText;
                    informacionVehiculo.adressSlowVehicule = direccionVehiculo;
                    informacionVehiculo.adressMyVehicule = direccionPropia;
                }
            }
            
            return informacionVehiculo;
        }

        /// <summary>
        /// Funcion que devuelve la configuracion de alertas si no hay devuelve todas a true
        /// </summary>
        /// <returns>Configuracion alertas</returns>
        protected Settings retrieveSettings()
        {
            if (Application.Current.Properties.ContainsKey("sonido") && Application.Current.Properties.ContainsKey("imagen") && Application.Current.Properties.ContainsKey("color"))
            {
                bool sonido = (bool) Application.Current.Properties["sonido"];
                bool imagen = (bool) Application.Current.Properties["imagen"];
                bool color = (bool) Application.Current.Properties["color"];

                return new Settings(sonido, imagen, color);
            }
            return new Settings();
        }

        public void Alertar()
        {

            Settings settings = retrieveSettings();

            if (settings.imagen)
            {
                imagen.IsVisible = true;
            }
            if (settings.sonido)
            {
                CrossMediaManager.Current.Play("https://www.soundjay.com/button/beep-05.mp3");
                CrossMediaManager.Current.Play("https://www.soundjay.com/button/beep-05.mp3");
            }
            if (settings.color)
            {
                //solo version free
            }
        }

        public void PararAlertar()
        {
            Settings settings = retrieveSettings();

            if (settings.sonido)
            {
                CrossMediaManager.Current.Stop();
            }
            if (settings.imagen)
            {
                imagen.IsVisible = false;
            }
            if (settings.color)
            {
                //solo version free
            }
        }
    }
}
