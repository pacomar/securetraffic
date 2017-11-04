using System;

using Xamarin.Forms;

namespace SecureTraffic
{
	public class LoginView : ContentPage
	{
		public LoginView()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}

