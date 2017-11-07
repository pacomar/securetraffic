using System;
using Firebase.Xamarin.Database;

namespace SecureTraffic
{
	public class ServiceBase
	{
		public static FirebaseClient firebase = new FirebaseClient("https://securtraffic-49c23.firebaseio.com");

		public ServiceBase()
		{
		}
	}
}
