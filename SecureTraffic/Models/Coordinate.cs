namespace SecureTraffic
{
	public class Coordinate
	{
		private double _latitude;
		private double _longitude;

		public Coordinate(double lati, double longi)
		{
			_latitude = lati;
			_longitude = longi;
		}

		public double Latitude{
			get { return _latitude; }
			set { _latitude = value; }
		}

		public double Longitude
		{
			get { return _longitude; }
			set { _longitude = value; }
		}

		public override string ToString(){
			string res = "";

			res = _latitude+", "+_longitude;

			return res;
		}
	}
}
