using SecureTraffic.Helpers;
using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class App : Application
	{
		public static string token = "";

		public App()
		{
			InitializeComponent();
			if (Settings.Token != "")
			{
				token = Settings.Token;
				MainPage = new NavigationPage(new FastVehicleView());
			}
			else
			{
				MainPage = new NavigationPage(new LoginView());	
			}
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
