using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class MainView : ContentPage
	{
		public MainView()
		{
			InitializeComponent();

            this.Title = "Secure Traffic";

			BindingContext = new MainViewModel();

			GoSelectSlowVehicle.Clicked += async (sender, args) =>
			{
				await Navigation.PushAsync(new SelectSlowVehicleView());
			};

			GoFastVehicle.Clicked += async(sender, args) =>
			{
				await Navigation.PushAsync(new FastVehicleView());
			};
		}
	}
}
