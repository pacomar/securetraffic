using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class SlowVehicleView : ContentPage
	{
		public SlowVehicleView(Vehicle veh)
		{
			InitializeComponent();

			this.Title = "Slow Vehicle";

			BindingContext = new SlowVehicleViewModel(veh);
		}
	}
}
