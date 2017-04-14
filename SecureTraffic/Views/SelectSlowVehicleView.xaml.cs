using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class SelectSlowVehicleView : ContentPage
	{
		public SelectSlowVehicleView()
		{
			InitializeComponent();

			GoSlowVehicleBike.Clicked += async(sender, args) =>
			{
				await Navigation.PushAsync(new SlowVehicleView(Vehicle.Bike));
			};

			GoSlowVehicleTractor.Clicked += async(sender, args) =>
			{
				await Navigation.PushAsync(new SlowVehicleView(Vehicle.Tractor));
			};
		}
	}
}
