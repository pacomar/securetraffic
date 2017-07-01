using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Xamarin.Forms;

namespace SecureTraffic.Droid
{
	[Activity(Label = "SecureTraffic.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

            //LocationManager locationManager = (LocationManager)Forms.Context.GetSystemService(Context.LocationService);

            //if (locationManager.IsProviderEnabled(LocationManager.GpsProvider) == false)
            //{
            //    Intent gpsSettingIntent = new Intent(Settings.ActionLocationSourceSettings);
            //    Forms.Context.StartActivity(gpsSettingIntent);
            //}

            //IOS
            //if (CLLocationManager.Status == CLAuthorizationStatus.Denied)
            //{
            //    if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            //    {
            //        NSString settingsString = UIApplication.OpenSettingsUrlString;
            //        NSUrl url = new NSUrl(settingsString);
            //        UIApplication.SharedApplication.OpenUrl(url);
            //    }
            //}

            LoadApplication(new App());
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

	}
}
