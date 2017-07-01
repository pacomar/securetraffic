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

			BindingContext = new UserViewModel();

			doRegister.Clicked += async(sender, args) =>
			{
				bool resgisted = await new UserViewModel().RegisterUser(email.Text, password.Text);
				if (resgisted)
				{
					await Navigation.PushModalAsync(new NavigationPage(new LoginView(email.Text)));
				}
				else
				{
					await DisplayAlert("Alert", "Los datos son ivalidos", "OK");
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
