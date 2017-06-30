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
					await Navigation.PushAsync(new LoginView(email.Text));
				}
			};

			goLogin.Clicked += async(sender, args) =>
			{
				await Navigation.PushAsync(new LoginView());
			};
		}
	}
}
