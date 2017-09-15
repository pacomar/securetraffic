using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading;

namespace SecureTraffic
{
	public class SlowVehicleViewModel : INotifyPropertyChanged
	{
		private string _position;
		private string _time;
		private string _heading;
		private string _speed;
		private string _accuracy;
		private string _altitude;
		private string _altitudeAccuracy;
		private Vehicle _vehicle;
		private Map _map { get; set; }

		public SlowVehicleViewModel(Vehicle veh, Map map)
		{
			_vehicle = veh;
			_map = map;
			StartListening();
		}

		public string Position
		{
			get { return _position; }
		}
		public string Time
		{
			get { return _time; }
		}
		public string Heading
		{
			get { return _heading; }
		}
		public string Speed
		{
			get { return _speed; }
		}
		public string Accuracy
		{
			get { return _accuracy; }
		}
		public string Altitude
		{
			get { return _altitude; }
		}
		public string _AltitudeAccuracy
		{
			get { return _altitudeAccuracy; }
		}

		private void RaisePropertyChanged(string propertyName)
		{
			var handle = PropertyChanged;
			if (handle != null)
				handle(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		async Task StartListening()
		{
			try
			{
				var locator = CrossGeolocator.Current;
				var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
                this._position = "Lat: " + position.Latitude.ToString() + " Long: " + position.Longitude.ToString();
				this._time = position.Timestamp.ToString();
				this._heading = position.Heading.ToString();
				this._speed = position.Speed.ToString();
				this._accuracy = position.Accuracy.ToString();
				this._altitude = position.Altitude.ToString();
				this._altitudeAccuracy = position.AltitudeAccuracy.ToString();


				RaisePropertyChanged("Position");
				RaisePropertyChanged("Time");
				RaisePropertyChanged("Heading");
				RaisePropertyChanged("Speed");
				RaisePropertyChanged("Accuracy");
				RaisePropertyChanged("Altitude");
				RaisePropertyChanged("AltitudeAccuracy");

				await locator.StartListeningAsync(500, 1, false);

				locator.PositionChanged += async (sender, e) => {
					position = e.Position;

					VehiclesService _vehServ = new VehiclesService();

					//TODO map e to My position
					MyPosition aux = new MyPosition()
					{
						Coordinate = new Coordinate(e.Position.Latitude, e.Position.Longitude),
						Speed = e.Position.Speed,
						Vehicle = _vehicle,
						Time = Helper.ConvertToTimestamp(DateTime.Now).ToString()
					};
                    this._map.MoveToRegion(new MapSpan(new Position(position.Latitude, position.Longitude), 0.05, 0.05));
					await _vehServ.SetPositionVehicle(aux);


					this._position = "Lat: " + position.Latitude.ToString() + " Long: " + position.Longitude.ToString();
					this._time = position.Timestamp.ToString();
					this._heading = position.Heading.ToString();
					this._speed = position.Speed.ToString();
					this._accuracy = position.Accuracy.ToString();
					this._altitude = position.Altitude.ToString();
					this._altitudeAccuracy = position.AltitudeAccuracy.ToString();


					RaisePropertyChanged("Position");
					RaisePropertyChanged("Time");
					RaisePropertyChanged("Heading");
					RaisePropertyChanged("Speed");
					RaisePropertyChanged("Accuracy");
					RaisePropertyChanged("Altitude");
					RaisePropertyChanged("AltitudeAccuracy");

                    await Task.Delay(5000);
                    StartListening();
                };
			}
			catch(Exception ex)
			{
			  Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex.Message);
              await Task.Delay(5000);
              StartListening();
            }
		}
    }
}