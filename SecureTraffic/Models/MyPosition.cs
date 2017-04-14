using System;
namespace SecureTraffic
{
	public class MyPosition
	{
		private Coordinate _coordinate;
		private double _speed;
		private string _token;
		private Vehicle _vehicle;
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

		public string Token
		{
			get { return this._token; }
			set { this._token = value; }
		}

		public Vehicle Vehicle
		{
			get { return this._vehicle; }
			set { this._vehicle = value; }
		}
	}
}
