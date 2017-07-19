using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Plugin.Geolocator.Abstractions;

namespace SecureTraffic
{
	public class VehiclesService : ServiceBase
	{
		public VehiclesService()
		{
		}

		public async Task<string> SetPositionVehicle(MyPosition position)
		{
			var vehicle = await firebase
				.Child("Vehicle")
				.Child(App.guid.ToString())
				//.WithAuth(App.token)
				.OnceSingleAsync<MyVehicle>();

			if (vehicle != null)
			{
				MyPosition aux = vehicle.CurrentPosition;
				vehicle.Time = position.Time;
				vehicle.CurrentPosition = position;
				vehicle.LastPosition = aux;
			}
			else
			{
				vehicle = new MyVehicle();
				vehicle.CurrentPosition = position;
				vehicle.LastPosition = null;
				vehicle.Time = position.Time;
			}
			
			await firebase
				.Child("Vehicle")
				.Child(App.guid.ToString())
				//.WithAuth(App.token)
				.PutAsync(vehicle);

			return App.guid.ToString();
		}

		public async Task<IEnumerable<FirebaseObject<MyVehicle>>> GetVehicles()
		{
			var items = await firebase
				.Child("Vehicle")
				//.WithAuth(App.token)
				.OnceAsync<MyVehicle>();

			int timestamp = Helper.ConvertToTimestamp(DateTime.Now);
			var aux = items.Where(veh => (
				(timestamp - long.Parse(veh.Object.Time)) < 300) &&
              	veh.Key != App.guid.ToString());

			return aux;
		}
	}
}
