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
				await new UserViewModel().RegisterUser(email.Text, password.Text);
			};

			goLogin.Clicked += async(sender, args) =>
			{
				await Navigation.PushAsync(new LoginView());
			};
		}
	}
}
