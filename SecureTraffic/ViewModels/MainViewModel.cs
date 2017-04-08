using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SecureTraffic
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private Coordinate _position;
		private string _textPosition;
		private Command _getPosition;

		public MainViewModel()
		{
			_position = new Coordinate(0.0, 0.0);
		}

		public string TextPosition
		{
			get { return _textPosition; }
			set
			{
				_textPosition = value;
			}
		}

		public ICommand HelloCommand
		{
			get { return _getPosition = _getPosition ?? new Command(GetPositionExecute); }
		}

		private void GetPositionExecute()
		{
			_position = ;
			RaisePropertyChanged("TextPosition");
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
