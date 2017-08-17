using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class LoginView : ContentPage
	{
		public LoginView()
		{
			InitializeComponent();

            this.Title = "Login";
		
			GoRegister.Clicked += async(sender, args) =>
			{
				await Navigation.PushModalAsync(new NavigationPage(new RegisterView()));
			};

			DoLogin.Clicked += async(sender, args) =>
			{
				//Loadding.IsRunning = true;
				bool resgisted = await new UserViewModel().LoginUser(email.Text, password.Text);
				if (resgisted)
				{
					//Loadding.IsRunning = false;
					await Navigation.PushModalAsync(new NavigationPage(new FastVehicleView()));
				}
				else
				{
					//Loadding.IsRunning = false;
                    await DisplayAlert("Alert", "Tus datos no son correctos", "OK");
					password.Text = "";
				}
			};

			DoLoginGoogle.Clicked += async(sender, args) =>
			{
				//Loadding.IsRunning = true;
				bool resgisted = await new UserViewModel().LoginUserGoogle();
				if (resgisted)
				{
					//Loadding.IsRunning = false;
					await Navigation.PushModalAsync(new NavigationPage(new FastVehicleView()));
				}
				else
				{
					//Loadding.IsRunning = false;
                    await DisplayAlert("Alert", "Error al iniciar sesión", "OK");	password.Text = "";
				}
			};

			DoLoginFacebook.Clicked += async(sender, args) =>
			{
				//Loadding.IsRunning = true;
				bool resgisted = await new UserViewModel().LoginUserFacebook();
				if (resgisted)
				{
					//Loadding.IsRunning = false;
					await Navigation.PushModalAsync(new NavigationPage(new FastVehicleView()));
				}
				else
				{
					//Loadding.IsRunning = false;
                    await DisplayAlert("Alert", "Error al iniciar sesión", "OK"); password.Text = "";
				}
			};
		}
	}
}
