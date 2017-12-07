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

            this.Title = "SECURTRAFFIC";

            ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Ajustes",
                Command = new Command(() => LanzarPantallaSettings())
            });

            GoSlow.Clicked += async (sender, args) =>
            {
                GoSlow.IsVisible = false;
                ImageBici.IsVisible = true;
                ImageAgricola.IsVisible = true;
                ImageObra.IsVisible = true;
                ImagePersona.IsVisible = true;
                ImageOtro.IsVisible = true;
            };

            var locator = CrossGeolocator.Current;

            if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled){
                GoSlow.IsVisible = false;
                ImageBici.IsVisible = false;
                ImageAgricola.IsVisible = false;
                ImageObra.IsVisible = false;
                ImagePersona.IsVisible = false;
                ImageOtro.IsVisible = false;
                Device.BeginInvokeOnMainThread(async () => {
                    await DisplayAlert("Aviso", "Habilita el GPS para poder usar la aplicación.", "OK");
                });
            }else{                
                BindingContext = new FastVehicleViewModel(MyMap, ImageAlert, TextDistance, ImageAlert2, TextDistance2, ImageAlert3, TextDistance3);
            }

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
            //Image imagen = (Image)sender;
            //imagen.IsVisible = false;
            ImageAlert.IsVisible = false;
            TextDistance.IsVisible = false;

            ImageAlert2.IsVisible = false;
            TextDistance2.IsVisible = false;

            ImageAlert3.IsVisible = false;
            TextDistance3.IsVisible = false;
        }

        public async void LanzarPantallaSettings()
        {
            await Navigation.PushAsync(new SettingsView());
        }
    }
}
