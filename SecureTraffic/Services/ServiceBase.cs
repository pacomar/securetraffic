using System;
using Firebase.Xamarin.Database;

namespace SecureTraffic
{
	public class ServiceBase
	{
		public static FirebaseClient firebase = new FirebaseClient("https://securetraffic-f6abc.firebaseio.com");

		public ServiceBase()
		{
		}
	}
}
