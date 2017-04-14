using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
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

		/*public async Task<string> UpdatePositionVehicle(MyPosition position, string key)
		{
			FirebaseObject<MyPosition> aux = new FirebaseObject<MyPosition>(key);
			await firebase
				.Child("Position")
				.PutAsync(position);

			return item.Key;
		}*/

		public async Task<IReadOnlyCollection<FirebaseObject<Coordinate>>> GetVehicles()
		{
			var items = await firebase
				.Child("Coordinate")
				.OnceAsync<Coordinate>();

			return items;
		}
	}
}
