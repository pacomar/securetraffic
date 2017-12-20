using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureTraffic.Google
{
    public static class Constants
    {
        public static string AppName = "OAuthNativeFlow";

        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string iOSClientId = "276934282312-94kcko5pq8b6esk6fvvmvai49qnatl3k.apps.googleusercontent.com";
        public static string AndroidClientId = "276934282312-nclvl8p0be2da7nta7t2pk1bcdsgvh8v.apps.googleusercontent.com";

        // These values do not need changing
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string iOSRedirectUrl = "com.googleusercontent.apps.276934282312-94kcko5pq8b6esk6fvvmvai49qnatl3k:/oauth2redirect";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.276934282312-nclvl8p0be2da7nta7t2pk1bcdsgvh8v:/oauth2redirect";
    }
}
