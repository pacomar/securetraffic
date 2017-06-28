using Android.Views;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class SettingsView : ContentPage
	{
		public SettingsView()
		{
            var attributesWindow = new WindowManagerLayoutParams();

			InitializeComponent();

			this.Title = "Configuración";
            
            BindingContext = new SettingsViewModel();
		}

    }
}
