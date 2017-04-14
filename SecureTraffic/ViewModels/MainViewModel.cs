using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using Plugin.Geolocator;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SecureTraffic
{
	public class MainViewModel
	{
		public MainViewModel()
		{
			string rnd = new Random().Next(int.MinValue, int.MaxValue).ToString();
			var tokenGenerator = new Firebase.Xamarin.Token.TokenGenerator("zHGOXaynKRyC7QZqe1GWp30ZhWmhRP4qtnEorl3D");
			var authPayload = new Dictionary<string, object>()
			{
				{"uid", rnd.ToString()}
			};
			App.token = tokenGenerator.CreateToken(authPayload);
		}
	}
}
