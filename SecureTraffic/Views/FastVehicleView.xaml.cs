using System;
using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class FastVehicleView : ContentPage
	{
		public FastVehicleView()
		{
			InitializeComponent();

			this.Title = "SecurTraffic";

            ToolbarItems.Add(new ToolbarItem("Settings", "prueba.png", () =>
            {
                new SettingsView();
            }));

            BindingContext = new FastVehicleViewModel(MyMap, ImageAlert);
        }

        public async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            var imageSender = (Image)sender;

            switch (imageSender.ClassId)
            {
                case "0":
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Bici));
                    break;
                case "1":
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Tractor));
                    break;
                case "2":
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Obra));
                    break;
                case "3":
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Otro));
                    break;
                default:
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Otro));
                    break;
            }
        }

        public void OnTapGestureRecognizerTappedAlert(object sender, EventArgs args)
        {
            Image imagen = (Image) sender;
            imagen.IsVisible = false;
        }
    }
}
