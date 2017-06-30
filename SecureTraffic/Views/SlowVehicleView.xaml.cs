using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class SlowVehicleView : ContentPage
	{
		public SlowVehicleView(Vehicle veh)
		{
            //var attributesWindow = new WindowManagerLayoutParams();

			InitializeComponent();

			this.Title = "Vehículo lento: " + veh;

			BindingContext = new SlowVehicleViewModel(veh);
		}

    }
}
