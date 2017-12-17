
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using SecureTraffic.Messages;
using SecureTraffic.Droid.Services;
using Plugin.Permissions;

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
            WireUpLongRunningTask();

            LoadApplication(new App());
		}


        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", message =>
            {
                var intent = new Intent(this, typeof(LongRunningTaskService));
                StartService(intent);
            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message =>
            {
                var intent = new Intent(this, typeof(LongRunningTaskService));
                StopService(intent);
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

	}
}
