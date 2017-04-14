using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;

namespace SecureTraffic
{
	public class FastVehicleViewModel
	{
		private Map _map { get; set; } 
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
				var pin = new Pin
				{
					Type = PinType.Place,
					Position = new Position(vehicle.Object.Latitude, vehicle.Object.Longitude),
					Label = "slow vehicle",
					Address = "custom detail info"
				};
				this._map.Pins.Add(pin);
			}

			return true;
		}
	}
}
