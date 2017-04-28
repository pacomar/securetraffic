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
			var item = await firebase
				.Child("Position")
				.PostAsync(position);

			return item.Key;
		}

		public async Task<IEnumerable<FirebaseObject<MyPosition>>> GetVehicles()
		{
			var items = await firebase
				.Child("Position")
				//.OrderBy("Time")
				//.LimitToLast(50)
				.OnceAsync<MyPosition>();

			var aux1 = items.GroupBy(pos => pos.Object.Token);
			var aux2 = aux1.Select(pos => pos.OrderByDescending(posAux => posAux.Object.Time));
			var aux3 = aux2.Select(pos => pos.FirstOrDefault());
			int timestamp = Helper.ConvertToTimestamp(DateTime.Now);
			var aux4 = aux3.Where(pos => (timestamp - long.Parse(pos.Object.Time)) < 300);

			return aux4;
		}
	}
}
