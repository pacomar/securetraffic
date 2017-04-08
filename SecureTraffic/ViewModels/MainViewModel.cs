using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using Plugin.Geolocator;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace SecureTraffic
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private Coordinate _position;
		public ICommand GetPosition { get; private set; }
		private bool canDownload = true;

		public MainViewModel()
		{
			_position = new Coordinate(0.0, 0.0);
			GetPosition = new Command(async () => await GetPositionAsync(), () => canDownload);
		}

		public string TextPosition
		{
			get { return _position.ToString(); }
		}

		async Task GetPositionAsync()
		{
			CanInitiateNewDownload(false);
			_position = new Coordinate(0.0, 0.0);

			try
			{	
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;

				var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
				if(position == null)
					return;

				_position = new Coordinate(position.Latitude, position.Longitude);
				RaisePropertyChanged("TextPosition");
			}
			catch(Exception ex)
			{
			  Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
			}

			CanInitiateNewDownload(true);
		}

		void CanInitiateNewDownload(bool value)
		{
			canDownload = value;
			((Command)GetPosition).ChangeCanExecute();   
		}

		private void RaisePropertyChanged(string propertyName)
		{
			var handle = PropertyChanged;
			if (handle != null)
				handle(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
