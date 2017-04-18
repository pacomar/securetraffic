using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;
using Firebase.Xamarin.Database;
using SecureTraffic.Models;
using System.Net.Http;

namespace SecureTraffic
{
    public class FastVehicleViewModel
    {
        private Map _map { get; set; }
        private Position myPosition;
        private int alertDistance = 500;
        private int distancePosibleAlert = 1000;

        public FastVehicleViewModel(Map _map)
        {
            this._map = _map;
            CenterMap();
            UpdateMarkers();
        }

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
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
            }

            return res;
        }

        public async Task<bool> UpdateMarkers()
        {
            VehiclesService _vehServ = new VehiclesService();
            var vehicles = await _vehServ.GetVehicles();

            foreach (var vehicle in vehicles)
            {
                string address = "Too far";
                InfoCloseVehicule infoVehicle = new InfoCloseVehicule();

                if (CalculateDistanceLine(myPosition.Latitude, myPosition.Longitude, vehicle.Object.Latitude, vehicle.Object.Longitude) < distancePosibleAlert)
                {
                    infoVehicle = await this.GetInformationCloseVehicle(myPosition.Latitude, myPosition.Longitude, vehicle.Object.Latitude, vehicle.Object.Longitude);
                    address = infoVehicle.adressSlowVehicule;
                }

                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(vehicle.Object.Latitude, vehicle.Object.Longitude),
                    Label = "slow vehicle",
                    Address = address
                };
                this._map.Pins.Add(pin);

                if (infoVehicle.adressSlowVehicule == infoVehicle.adressMyVehicule && infoVehicle.distance < alertDistance)
                {
                    //LANZAR ALERTAS
                }
            }

            return true;
        }

        /// <summary>
        /// Funcion que calcula la distancia en linea recta dadas 2 coordenadas (matemáticamente)
        /// </summary>
        /// <param name="fromLong">Longitud origen</param>
        /// <param name="fromLat">Latitud origen</param>
        /// <param name="toLong">Longitud destino</param>
        /// <param name="toLat">Latitud destino</param>
        /// <returns>Resultado en m de la distancia</returns>
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
            string url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + fromLong + "," + fromLat + "&destination=" + toLong + ","  + toLat + "&sensor=false";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                // ... Display the result.
                if (result != null)
                {
                    var JSONObject = Newtonsoft.Json.Linq.JObject.Parse(result);

                    string distancia = (string)JSONObject["routes"][0]["legs"][0]["distance"];
                    string tiempo = (string)JSONObject["routes"][0]["legs"][0]["duration"];
                    string direccionVehiculo = (string)JSONObject["routes"][0]["legs"][0]["end_address"];
                    string direccionPropia = (string)JSONObject["routes"][0]["legs"][0]["start_address"];

                    //JSONArray routeArray = json.GetJSONArray("routes");
                    //JSONObject routes = routeArray.GetJSONObject(0);

                    //JSONArray newTempARr = routes.GetJSONArray("legs");
                    //JSONObject newDisTimeOb = newTempARr.GetJSONObject(0);

                    //JSONObject distOb = newDisTimeOb.GetJSONObject("distance");
                    //JSONObject timeOb = newDisTimeOb.GetJSONObject("duration");

                    informacionVehiculo.distance = Int32.Parse(distancia);
                    informacionVehiculo.time = Int32.Parse(tiempo);
                    informacionVehiculo.adressSlowVehicule = direccionVehiculo;
                    informacionVehiculo.adressMyVehicule = direccionPropia;

                    return informacionVehiculo;
                }
            }
            return informacionVehiculo;
        }
    }
}
