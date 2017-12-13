using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using SecureTraffic.iOS.Services;
using Xamarin.Forms;
using SecureTraffic.Messages;

namespace SecureTraffic.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			// define useragent android like
			string userAgent = "Mozilla/5.0 (Linux; Android 5.1.1; Nexus 5 Build/LMY48B; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/43.0.2357.65 Mobile Safari/537.36";

			// set default useragent
			NSDictionary dictionary = NSDictionary.FromObjectAndKey(NSObject.FromObject(userAgent), NSObject.FromObject("UserAgent"));
			NSUserDefaults.StandardUserDefaults.RegisterDefaults(dictionary);

            global::Xamarin.Forms.Forms.Init();

            Xamarin.FormsMaps.Init();

            WireUpLongRunningTask();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        #region Methods
        iOSLongRunningTaskExample longRunningTaskExample;

        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", async message =>
            {
                longRunningTaskExample = new iOSLongRunningTaskExample();
                await longRunningTaskExample.Start();
            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message =>
            {
                longRunningTaskExample.Stop();
            });
        }
        #endregion
    }
}
