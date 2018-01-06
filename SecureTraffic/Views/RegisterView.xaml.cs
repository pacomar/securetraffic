using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class RegisterView : ContentPage
	{
		public RegisterView()
		{
			InitializeComponent();

            this.Title = "Registro";

            BindingContext = new UserViewModel();

			doRegister.Clicked += async(sender, args) =>
			{
				bool resgisted = await new UserViewModel().RegisterUser(email.Text, password.Text);
				if (resgisted)
				{
                    await DisplayAlert("Genial!", "Registrado correctamente", "OK");
                    await Navigation.PushModalAsync(new NavigationPage(new LoginView()));
				}
				else
				{
					await DisplayAlert("Ups", "Usuario ya registrado", "OK");
					email.Text = "";
					password.Text = "";
				}
			};

			goLogin.Clicked += async(sender, args) =>
			{
				await Navigation.PushModalAsync(new NavigationPage(new LoginView()));
			};
		}
	}
}
