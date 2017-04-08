using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class MainView : ContentPage
	{
		public MainView()
		{
			InitializeComponent();

			BindingContext = new MainViewModel();
		}
	}
}
