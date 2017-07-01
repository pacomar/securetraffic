using System;
namespace SecureTraffic
{
	public class MyPosition
	{
		private Coordinate _coordinate;
		private double _speed;
		private Vehicle _vehicle;
		private string _time;
		public MyPosition()
		{
		}

		public Coordinate Coordinate
		{
			get { return this._coordinate; }
			set { this._coordinate = value; }
		}

		public double Speed
		{
			get { return this._speed; }
			set { this._speed = value; }
		}

		public Vehicle Vehicle
		{
			get { return this._vehicle; }
			set { this._vehicle = value; }
		}

		public string Time
		{
			get { return this._time; }
			set { this._time = value; }
		}
	}
}
