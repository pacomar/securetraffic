using System;
namespace SecureTraffic
{
	public class MyVehicle
	{
		private MyPosition _currentPosition;
		private MyPosition _lastPosition;
		private string _time;

		public MyVehicle()
		{
		}

		public MyPosition CurrentPosition
		{
			get { return this._currentPosition; }
			set { this._currentPosition = value; }
		}

		public MyPosition LastPosition
		{
			get { return this._lastPosition; }
			set { this._lastPosition = value; }
		}

		public string Time
		{
			get { return this._time; }
			set { this._time = value; }
		}
	}
}
