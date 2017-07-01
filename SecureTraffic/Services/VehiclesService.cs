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
				.Child(App.token)
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
				vehicle.CurrentPosition = position;
				vehicle.Time = position.Time;
			}
			
			var item = await firebase
				.Child("Vehicle")
				.PostAsync(vehicle);

			return item.Key;
		}

		public async Task<IEnumerable<FirebaseObject<MyPosition>>> GetVehicles()
		{
			var items = await firebase
				.Child("Vehicle")
				.OrderBy("Time")
				.LimitToLast(50)
				.OnceAsync<MyPosition>();

			int timestamp = Helper.ConvertToTimestamp(DateTime.Now);
			var aux = items.Where(veh => (timestamp - long.Parse(veh.Object.Time)) < 300);

			return aux;
		}
	}
}
