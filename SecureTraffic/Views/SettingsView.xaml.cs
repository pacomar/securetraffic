using System;
using System.Collections.Generic;

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
		}

    }
}
