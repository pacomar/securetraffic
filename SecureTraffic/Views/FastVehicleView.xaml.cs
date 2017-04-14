using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class FastVehicleView : ContentPage
	{
		public FastVehicleView()
		{
			InitializeComponent();

			this.Title = "Fast Vehicle";

			BindingContext = new FastVehicleViewModel(MyMap);
		}
	}
}
