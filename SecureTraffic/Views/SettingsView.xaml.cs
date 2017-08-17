using System;
using System.Collections.Generic;
using SecureTraffic.Helpers;
using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class SettingsView : ContentPage
	{
		public SettingsView()
		{
            InitializeComponent();

			this.Title = "Configuración";
            
            BindingContext = new SettingsViewModel();

			DoLogout.Clicked += async (sender, e) =>
			{
				Settings.Token = "";
				await Navigation.PushModalAsync(new LoginView());
			};
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {

        }
    }
}
