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
using System.Text.RegularExpressions;

namespace SecureTraffic
{
    public class FastVehicleViewModel
    {
        private Map _map { get; set; }
        private Position myPosition;
        private Position myLastPosition;
        private int alertDistance = 1000;
        private int distancePosibleAlert = 1200;
        private Xamarin.Forms.Image imagen { get; set; }
        public Label distanceLabel { get; set; }

        private Xamarin.Forms.Image imagen2 { get; set; }
        public Label distanceLabel2 { get; set; }

        private Xamarin.Forms.Image imagen3 { get; set; }
        public Label distanceLabel3 { get; set; }

        private List<Alerta> alertas = new List<Alerta>();

        public FastVehicleViewModel(Map _map, Xamarin.Forms.Image imagen, Label distanceLabel, Xamarin.Forms.Image imagen2, Label distanceLabel2, Xamarin.Forms.Image imagen3, Label distanceLabel3)
        {
            try
            {
                string rnd = new Random().Next(int.MinValue, int.MaxValue).ToString();

                this._map = _map;
                this.imagen = imagen;
                this.distanceLabel = distanceLabel;

                this.imagen2 = imagen2;
                this.distanceLabel2 = distanceLabel2;

                this.imagen3 = imagen3;
                this.distanceLabel3 = distanceLabel3;

                //centrar el mapa en españa - madrid
                this._map.MoveToRegion(new MapSpan(new Position(40.4893538, -3.6827461), 0.05, 0.05));

                CenterMap();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FastVehicleViewModel: " + ex.Message);
            }
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
                while (position == null) position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                try { myLastPosition = myPosition; } catch { }
                myPosition = new Position(position.Latitude, position.Longitude);

                this._map.MoveToRegion(new MapSpan(new Position(position.Latitude, position.Longitude), 0.05, 0.05));

                res = true;

                //detectar si vamos a menos de 60 para avisar como vehiculo lento
                if (position.Speed < 16.667)
                {
                    AvisarSoyLento(position);
                }

                UpdateMarkers();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex.Message);
                CenterMap();
            }

            return res;
        }

        public async void AvisarSoyLento(Plugin.Geolocator.Abstractions.Position position)
        {
            try
            {
                VehiclesService _vehServ = new VehiclesService();

                //TODO map e to My position
                MyPosition aux = new MyPosition()
                {
                    Coordinate = new Coordinate(position.Latitude, position.Longitude),
                    Speed = position.Speed,
                    Vehicle = Vehicle.Otro,
                    Time = Helper.ConvertToTimestamp(DateTime.Now).ToString()
                };
                this._map.MoveToRegion(new MapSpan(new Position(position.Latitude, position.Longitude), 0.05, 0.05));
                await _vehServ.SetPositionVehicle(aux);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AvisarSoyLento: " + ex.Message);
            }
        }

        /// <summary>
        /// Funcion que actualiza los puntos del mapa, comprueba la distancia por coordendas, si esta muy cerca llama a la API de google y lanza aviso si es necesario
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateMarkers()
        {
            try
            {

                VehiclesService _vehServ = new VehiclesService();
                var vehicles = await _vehServ.GetVehicles();
                this._map.Pins.Clear();
                
                List<Alerta> alertasbuffer = new List<Alerta>();

                foreach (var vehicle in vehicles)
                {
                    InfoCloseVehicule infoVehicle = new InfoCloseVehicule();

                    if (CalculateDistanceLine(myPosition.Latitude, myPosition.Longitude, vehicle.Object.CurrentPosition.Coordinate.Latitude, vehicle.Object.CurrentPosition.Coordinate.Longitude) < distancePosibleAlert)
                    {
                        infoVehicle = await this.GetInformationCloseVehicle(myPosition.Latitude, myPosition.Longitude, vehicle.Object.CurrentPosition.Coordinate.Latitude, vehicle.Object.CurrentPosition.Coordinate.Longitude);
                    }

                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = new Position(vehicle.Object.CurrentPosition.Coordinate.Latitude, vehicle.Object.CurrentPosition.Coordinate.Longitude),
                        Label = vehicle.Object.CurrentPosition.Vehicle.ToString(),
                    };
                    this._map.Pins.Add(pin);


                    bool lanzaraviso = ComprobarDistanciaYCarretera(infoVehicle, new Coordinate(myPosition.Latitude, myPosition.Longitude), new Coordinate(myLastPosition.Latitude, myLastPosition.Longitude), vehicle.Object.CurrentPosition.Coordinate, vehicle.Object.LastPosition.Coordinate);

                    if (lanzaraviso)
                    {
                        alertasbuffer.Add(new Alerta(vehicle.Key, vehicle.Object.CurrentPosition.Vehicle, infoVehicle.distanceText, 0, TextoANumero(infoVehicle.distanceText)));
                    }
                }
                //alertasbuffer.Add(new Alerta("1", new Vehicle(), "300 m", 0, 300));
                //alertasbuffer.Add(new Alerta("2", new Vehicle(), "350 m", 0, 350));
                //alertasbuffer.Add(new Alerta("3", new Vehicle(), "360 m", 0, 360));
                //alertasbuffer.Add(new Alerta("4", new Vehicle(), "300 m", 0, 300));
                //alertasbuffer.Add(new Alerta("5", new Vehicle(), "300 m", 0, 300));
                //alertasbuffer.Add(new Alerta("6", new Vehicle(), "300 m", 0, 300));
                alertas = CompararAlertasGuardadasVsNuevas(alertasbuffer);

                if (alertas.Count > 0) Alertar(alertas);
                else PararAlertar();

                await Task.Delay(5000);

                CenterMap();

                return true;
            }
            catch (Exception ex)
            {
                CenterMap();
                Debug.WriteLine("UpdateMarkers: " + ex.Message);
                return true;
            }

        }

        private int TextoANumero(string distanceText)
        {
            try
            {
                string buffer = Regex.Replace(distanceText, "[^0-9]+", string.Empty);

                return int.Parse(buffer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TextoANumero: " + ex.Message);
                return 500;
            }
        }

        private List<Alerta> CompararAlertasGuardadasVsNuevas(List<Alerta> alertasbuffer)
        {
            if (alertasbuffer.Count > 0)
            {
                foreach (Alerta posiblealerta in alertasbuffer)
                {
                    foreach (Alerta posiblealertaguardada in alertas)
                    {

                        if (posiblealerta.identificador.Equals(posiblealertaguardada.identificador))
                        {
                            posiblealerta.contador = posiblealertaguardada.contador++;
                        }

                    }
                }
            }

            return alertasbuffer;
        }

        public bool ComprobarDistanciaYCarretera(InfoCloseVehicule infoVehicle, Coordinate x1, Coordinate x2, Coordinate y1, Coordinate y2)
        {
            try
            {
                if (infoVehicle.distance < alertDistance)
                {
                    List<string> listacalles = ListaCalles();
                    List<string> listacontrolarsentidoautopistas = ListaAutopistas();
                    bool esunacarretera = !listacalles.Any(x => infoVehicle.adressSlowVehicule.Split(',')[0].ToLower().Contains(x.ToLower()));
                    bool esdeunicosentido = listacontrolarsentidoautopistas.Any(x => infoVehicle.adressSlowVehicule.Split(',')[0].ToLower().Contains(x.ToLower()));

                    if (infoVehicle.adressSlowVehicule.Split(',')[0] == infoVehicle.adressMyVehicule.Split(',')[0] && esunacarretera)
                    {
                        if (esdeunicosentido)
                        {
                            if (EsMismoSentido(x1, x2, y1, y2)) return true;
                        }
                        else return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ComprobarDistanciaYCarretera: " + ex.Message);
                return false;
            }

        }

        public List<string> ListaCalles()
        {
            return new List<string>() { "alameda", "calle", "c/", "camino", "glorieta", "kalea", "pasaje", "paseo", "pº", "plaça", "plaza", "plza", "pza", "rambla", "ronda", "rua", "rúa", "sector", "av.", "calle", "travesía", "travesia", "urbanizacion", "urbanización", "avenida", "avda", "avinguda", "barrio", "bº", "calleja", "cami", "camí", "carrera", "cuesta", "edificio", "enparantza", "estrada", "jardines", "jardins", "parque", "passeig", "praza", "plazuela", "placeta", "poblado", "pbdo", "pd.", "travessera", "avinguda", "passatge", "bulevar", "ps.", "poligono", "polígono", "otros" };
        }

        public List<string> ListaAutopistas()
        {
            return new List<string>() { "a-", "r-", "ap-", "autopista", "autovia", "autovía", "peaje", "acceso" };
        }

        public bool EsMismoSentido(Coordinate x1, Coordinate x2, Coordinate y1, Coordinate y2)
        {
            try
            {
                Sentidos sentidolatitudx = Sentidos.Incierto;
                Sentidos sentidolongitudx = Sentidos.Incierto;
                Sentidos sentidolatitudy = Sentidos.Incierto;
                Sentidos sentidolongitudy = Sentidos.Incierto;

                sentidolatitudx = CalcularSentidoLatitud(x1.Latitude, x2.Latitude);
                sentidolongitudx = CalcularSentidoLongitud(x1.Longitude, x2.Longitude);
                sentidolatitudy = CalcularSentidoLatitud(y1.Latitude, y2.Latitude);
                sentidolongitudy = CalcularSentidoLongitud(y1.Longitude, y2.Longitude);

                if (sentidolatitudx == sentidolatitudy || sentidolongitudx == sentidolongitudy) return true;

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EsMismoSentido: " + ex.Message);
                return false;
            }
        }

        public Sentidos CalcularSentidoLongitud(double cordenada1, double cordenada2)
        {
            try
            {
                if (cordenada1 > cordenada2) return Sentidos.Oeste;
                else if (cordenada1 < cordenada2) return Sentidos.Este;

                return Sentidos.Incierto;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalcularSentidoLongitud: " + ex.Message);
                return Sentidos.Incierto;
            }
        }

        public Sentidos CalcularSentidoLatitud(double cordenada1, double cordenada2)
        {
            try
            {
                if (cordenada1 > cordenada2) return Sentidos.Sur;
                else if (cordenada1 < cordenada2) return Sentidos.Norte;

                return Sentidos.Incierto;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalcularSentidoLatitud: " + ex.Message);
                return Sentidos.Incierto;
            }
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
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine("CalculateDistanceLine: " + ex.Message);
                return double.MaxValue;
            }

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
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine("GetInformationCloseVehicle: " + ex.Message);
                return new InfoCloseVehicule();
            }
        }

        /// <summary>
        /// Funcion que devuelve la configuracion de alertas si no hay devuelve todas a true
        /// </summary>
        /// <returns>Configuracion alertas</returns>
        protected SettingsModel retrieveSettings()
        {
            try
            {
                if (Application.Current.Properties.ContainsKey("sonido") && Application.Current.Properties.ContainsKey("imagen") && Application.Current.Properties.ContainsKey("color"))
                {
                    bool sonido = (bool)Application.Current.Properties["sonido"];
                    bool imagen = (bool)Application.Current.Properties["imagen"];
                    bool color = (bool)Application.Current.Properties["color"];

                    return new SettingsModel(sonido, imagen, color);
                }
                return new SettingsModel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("retrieveSettings: " + ex.Message);
                return new SettingsModel();
            }
        }

        public void Alertar(List<Alerta> alertasActivas)
        {
            try
            {
                SettingsModel settings = retrieveSettings();

                List<Alerta> SortedList = alertasActivas.OrderBy(o => o.distancia).Where(o => o.contador < 10).ToList();

                for (int i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (SortedList.Count > 0)
                            {
                                if (settings.imagen)
                                {
                                    switch (SortedList.ToArray()[0].vehiculo)
                                    {
                                        case Vehicle.Agricola:
                                            imagen.Source = "agricola.png";
                                            break;
                                        case Vehicle.Bici:
                                            imagen.Source = "bici.png";
                                            break;
                                        case Vehicle.Obra:
                                            imagen.Source = "obra.png";
                                            break;
                                        case Vehicle.Otro:
                                            imagen.Source = "otro.png";
                                            break;
                                        case Vehicle.Persona:
                                            imagen.Source = "persona.png";
                                            break;
                                    }

                                    imagen.IsVisible = true;
                                    distanceLabel.IsVisible = true;
                                    distanceLabel.Text = SortedList.ToArray()[0].distanciaTexto;
                                }
                                if (settings.sonido)
                                {
                                    CrossMediaManager.Current.Play("https://www.soundjay.com/button/beep-05.mp3");
                                    CrossMediaManager.Current.Play("https://www.soundjay.com/button/beep-05.mp3");
                                }
                            }
                            else
                            {
                                imagen.IsVisible = false;
                                distanceLabel.IsVisible = false;
                            }
                            break;

                        case 1:
                            if (SortedList.Count > 1)
                            {
                                if (settings.imagen)
                                {
                                    switch (SortedList.ToArray()[1].vehiculo)
                                    {
                                        case Vehicle.Agricola:
                                            imagen2.Source = "agricola.png";
                                            break;
                                        case Vehicle.Bici:
                                            imagen2.Source = "bici.png";
                                            break;
                                        case Vehicle.Obra:
                                            imagen2.Source = "obra.png";
                                            break;
                                        case Vehicle.Otro:
                                            imagen2.Source = "otro.png";
                                            break;
                                        case Vehicle.Persona:
                                            imagen2.Source = "persona.png";
                                            break;
                                    }

                                    imagen2.IsVisible = true;
                                    distanceLabel2.IsVisible = true;
                                    distanceLabel2.Text = SortedList.ToArray()[1].distanciaTexto;
                                }
                                if (settings.sonido)
                                {
                                    CrossMediaManager.Current.Play("https://www.soundjay.com/button/beep-05.mp3");
                                    CrossMediaManager.Current.Play("https://www.soundjay.com/button/beep-05.mp3");
                                }
                            }
                            else
                            {
                                imagen2.IsVisible = false;
                                distanceLabel2.IsVisible = false;
                            }
                            break;
                        case 2:

                            if (SortedList.Count > 2)
                            {
                                if (settings.imagen)
                                {
                                    switch (SortedList.ToArray()[2].vehiculo)
                                    {
                                        case Vehicle.Agricola:
                                            imagen3.Source = "agricola.png";
                                            break;
                                        case Vehicle.Bici:
                                            imagen3.Source = "bici.png";
                                            break;
                                        case Vehicle.Obra:
                                            imagen3.Source = "obra.png";
                                            break;
                                        case Vehicle.Otro:
                                            imagen3.Source = "otro.png";
                                            break;
                                        case Vehicle.Persona:
                                            imagen3.Source = "persona.png";
                                            break;
                                    }

                                    imagen3.IsVisible = true;
                                    distanceLabel3.IsVisible = true;
                                    distanceLabel3.Text = SortedList.ToArray()[2].distanciaTexto;
                                }
                                if (settings.sonido)
                                {
                                    CrossMediaManager.Current.Play("https://www.soundjay.com/button/beep-05.mp3");
                                    CrossMediaManager.Current.Play("https://www.soundjay.com/button/beep-05.mp3");
                                }
                            }
                            else
                            {
                                imagen3.IsVisible = false;
                                distanceLabel3.IsVisible = false;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Alertar: " + ex.Message);
            }
        }

        public void PararAlertar()
        {
            try
            {
                SettingsModel settings = retrieveSettings();

                if (settings.sonido)
                {
                    CrossMediaManager.Current.Stop();
                }
                if (settings.imagen)
                {
                    imagen.IsVisible = false;
                    distanceLabel.IsVisible = false;

                    imagen2.IsVisible = false;
                    distanceLabel2.IsVisible = false;

                    imagen3.IsVisible = false;
                    distanceLabel3.IsVisible = false;
                }
                if (settings.color)
                {
                    //ya no hay version free
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PararAlertar: " + ex.Message);
            }
        }
    }
}
