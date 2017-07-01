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
				await Navigation.PushModalAsync(new NavigationPage(new RegisterView()));
			};

			DoLogin.Clicked += async(sender, args) =>
			{
				bool resgisted = await new UserViewModel().LoginUser(email.Text, password.Text);
				if (resgisted)
				{
					await Navigation.PushModalAsync(new NavigationPage(new FastVehicleView()));
				}
				else
				{
                    await DisplayAlert("Alert", "Tus datos no son correctos", "OK");
					password.Text = "";
				}
			};
		}
	}
}
