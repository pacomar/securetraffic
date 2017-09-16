using System;
using System.Collections.Generic;
using SecureTraffic.Helpers;
using Xamarin.Forms;
using SecureTraffic.Models;

namespace SecureTraffic
{
	public partial class SettingsView : ContentPage
	{
		public SettingsView()
		{
            InitializeComponent();

			this.Title = "Configuración";

			DoLogout.Clicked += async (sender, e) =>
			{
				Settings.Token = "";
				await Navigation.PushModalAsync(new NavigationPage(new LoginView()));
			};

            SettingsModel settingsalertas = retrieveSettings();

                SwitchSonido.IsToggled = settingsalertas.sonido;
                SwitchImagen.IsToggled = settingsalertas.imagen;
                SwitchColor.IsToggled = settingsalertas.color;

        }

        private void Handle_ToggledSonido(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            SettingsModel settingsalertas = retrieveSettings();

            settingsalertas.sonido = SwitchSonido.IsToggled;

            SaveSettings(settingsalertas);
        }

        private void Handle_ToggledImagen(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            SettingsModel settingsalertas = retrieveSettings();

            settingsalertas.imagen = SwitchImagen.IsToggled;

            SaveSettings(settingsalertas);
        }

        private void Handle_ToggledColor(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            SettingsModel settingsalertas = retrieveSettings();

            settingsalertas.color = SwitchColor.IsToggled;

            SaveSettings(settingsalertas);
        }

        /// <summary>
        /// Funcion que devuelve la configuracion de alertas si no hay devuelve todas a true
        /// </summary>
        /// <returns>Configuracion alertas</returns>
        protected SettingsModel retrieveSettings()
        {
            if (Application.Current.Properties.ContainsKey("sonido") && Application.Current.Properties.ContainsKey("imagen") && Application.Current.Properties.ContainsKey("color"))
            {
                bool sonido = (bool)Application.Current.Properties["sonido"];
                bool imagen = (bool)Application.Current.Properties["imagen"];
                bool color = (bool)Application.Current.Properties["color"];

                return new SettingsModel(sonido, imagen, color);
            }
            return new SettingsModel();
        }

        /// <summary>
        /// Funcion que guarda las propiedades de las alertas
        /// </summary>
        /// <param name="settings"></param>
        protected void SaveSettings(SettingsModel settings)
        {
            Application.Current.Properties.Remove("sonido");
            Application.Current.Properties.Remove("imagen");
            Application.Current.Properties.Remove("color");

            Application.Current.Properties.Add("sonido", settings.sonido);
            Application.Current.Properties.Add("imagen", settings.imagen);
            Application.Current.Properties.Add("color", settings.color);

            Application.Current.SavePropertiesAsync();
        }
    }
}
