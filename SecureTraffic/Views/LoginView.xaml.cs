using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class LoginView : ContentPage
	{
		public LoginView(string emailPlace = "")
		{
			InitializeComponent();

            this.Title = "Login";

			email.Text = emailPlace;
		
			GoRegister.Clicked += async(sender, args) =>
			{
				await Navigation.PushAsync(new RegisterView());
			};
		}
	}
}
