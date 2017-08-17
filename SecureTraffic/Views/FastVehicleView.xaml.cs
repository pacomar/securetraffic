using Plugin.Geolocator;
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

			ToolbarItems.Add(new ToolbarItem(){
				Text = "Ajustes",
				Command = new Command(() => LanzarPantallaSettings() )
            });

            var locator = CrossGeolocator.Current;

            if (!locator.IsGeolocationAvailable && !locator.IsGeolocationEnabled) DisplayAlert("Aviso", "Por favor, habilita el GPS", "OK");

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
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Agricola));
                    break;
                case "2":
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Obra));
                    break;
                case "3":
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Persona));
                    break;
                case "4":
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Otro));
                    break;
                default:
                    await Navigation.PushAsync(new SlowVehicleView(Vehicle.Otro));
                    break;
            }
        }

        public void OnTapGestureRecognizerTappedAlert(object sender, EventArgs args)
        {
            Image imagen = (Image)sender;
            imagen.IsVisible = false;
        }

        public async void LanzarPantallaSettings()
        {
			await Navigation.PushAsync(new SettingsView());
        } 
    }
}
