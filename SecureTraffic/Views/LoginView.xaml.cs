using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SecureTraffic.ViewModels;
using Xamarin.Forms;
using static Xamarin.Forms.Device;
using SecureTraffic.Google;
using Xamarin.Auth;
using Newtonsoft.Json;

namespace SecureTraffic
{
	public partial class LoginView : ContentPage
	{
		private GoogleViewModel _googleViewModel { get; set; }
        Account account;
        AccountStore store;
        public LoginView()
		{
            try
            {

                InitializeComponent();

                store = AccountStore.Create();
                account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();

                this.Title = "Login";

                GoRegister.Clicked += async (sender, args) =>
                {
                    await Navigation.PushModalAsync(new NavigationPage(new RegisterView()));
                };

                DoLogin.Clicked += async (sender, args) =>
                {
                    try
                    {
                        bool comprobarPermisos = await ComprobarPermisos();
                        if (comprobarPermisos)
                        {
                            //Loadding.IsRunning = true;
                            bool resgisted = await new UserViewModel().LoginUser(email.Text, password.Text);
                            if (resgisted)
                            {
                                //Loadding.IsRunning = false;
                                await Navigation.PushModalAsync(new NavigationPage(new FastVehicleView()));
                            }
                            else
                            {
                                //Loadding.IsRunning = false;
                                await DisplayAlert("Advertencia", "Tus datos no son correctos", "OK");
                                password.Text = "";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                };

                DoLoginGoogle.Clicked += async (sender, args) =>
                {
                    bool comprobarPermisos = await ComprobarPermisos();
                    if (comprobarPermisos)
                    {
                        string clientId = null;
                        string redirectUri = null;

                        switch (Device.RuntimePlatform)
                        {
                            case Device.iOS:
                                clientId = Constants.iOSClientId;
                                redirectUri = Constants.iOSRedirectUrl;
                                break;

                            case Device.Android:
                                clientId = Constants.AndroidClientId;
                                redirectUri = Constants.AndroidRedirectUrl;
                                break;
                        }

                        try
                        {
                            var authenticator = new OAuth2Authenticator(
                                clientId,
                                null,
                                Constants.Scope,
                                new Uri(Constants.AuthorizeUrl),
                                new Uri(redirectUri),
                                new Uri(Constants.AccessTokenUrl),
                                null,
                                true);

                            authenticator.Completed += OnAuthCompleted;
                            authenticator.Error += OnAuthError;

                            AuthenticationState.Authenticator = authenticator;

                            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
                            presenter.Login(authenticator);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", "No se ha podido hacer login con GOOGLE", "OK");
                        }

                    }
                    };

                DoLoginFacebook.Clicked += async (sender, args) =>
                {
                    bool comprobarPermisos = await ComprobarPermisos();
                    if (comprobarPermisos)
                    {
                        var apiRequest =
                    "https://www.facebook.com/dialog/oauth?client_id="
                    + "173487199933858"//"1949845268596950"
                    + "&display=popup&response_type=token&redirect_uri=https://securtraffic-49c23.firebaseapp.com/__/auth/handler";

                        var webView = new WebView
                        {
                            Source = apiRequest,
                            HeightRequest = 1
                        };

                        webView.Navigated += WebViewOnNavigated;

                        Content = webView;
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoginView: " + ex.Message);
            }
		}

        private async Task<bool> ComprobarPermisos()
        {
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                    var status2 = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                    if (status != PermissionStatus.Granted)
                    {
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                        status = results[Permission.Location];
                    }

                    if (status2 != PermissionStatus.Granted)
                    {
                        var results2 = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                        status2 = results2[Permission.Storage];
                    }

                    if (status == PermissionStatus.Granted && status2 == PermissionStatus.Granted)
                    {
                        return true;
                    }
                    else
                    {
                        await DisplayAlert("Advertencia", "La aplicación no tiene permisos para utilizar el GPS o el Almacenamiento", "OK");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se ha podido realizar el login", "OK");
                return false;
            }
        }
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            User user = null;
            if (e.IsAuthenticated)
            {
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    // Deserialize the data and store it in the account store
                    // The users email address will be used to identify data in SimpleDB
                    string userJson = await response.GetResponseTextAsync();
                    user = JsonConvert.DeserializeObject<User>(userJson);
                }

                bool resgisted = await new UserViewModel().RegisterUser(user.Id.ToString() + "@ssogoogle.com", e.Account.Properties["access_token"]);

                bool loged = await new UserViewModel().LoginUser(user.Id.ToString() + "@ssogoogle.com", e.Account.Properties["access_token"]);
                           
                if (loged && resgisted)
                {
                    //Loadding.IsRunning = false;
                    await Navigation.PushModalAsync(new NavigationPage(new FastVehicleView()));
                }
                else
                {
                    //Loadding.IsRunning = false;
                    await DisplayAlert("Advertencia", "Tus datos no son correctos", "OK");
                    password.Text = "";
                }

                if (resgisted) resgisted = await new UserViewModel().LoginUser(user.Id.ToString() + "@ssogoogle.com", e.Account.Properties["access_token"]);

                if (account != null)
                {
                    store.Delete(account, Constants.AppName);
                }

                await store.SaveAsync(account = e.Account, Constants.AppName);
            }
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            Debug.WriteLine("Authentication error: " + e.Message);
        }
        public async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            Device.OpenUri(new Uri("https://securtraffic.000webhostapp.com/"));
        }
        //public async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        //{
		//	await Navigation.PushModalAsync(new NavigationPage(new TerminosWebView()));
        //}

        private async void WebViewOnNavigatedGoogle(object sender, WebNavigatedEventArgs e)
		{

			var code = ExtractCodeFromUrlGoogle(e.Url);

			if (code != "")
			{

				var accessToken = await _googleViewModel.GetAccessTokenAsync(code);

				//Loadding.IsRunning = true;
				bool resgisted = await new UserViewModel().LoginUserFacebook(accessToken);
                if (resgisted)
                {
                    //Loadding.IsRunning = false;
                    await Navigation.PushModalAsync(new NavigationPage(new FastVehicleView()));
                }
                else
                {
                    //Loadding.IsRunning = false;
                    await DisplayAlert("Alert", "Error al iniciar sesión", "OK"); password.Text = "";
                }
			}
		}

		private string ExtractCodeFromUrlGoogle(string url)
		{
			if (url.Contains("code="))
			{
				var attributes = url.Split('&');
				var code = attributes.FirstOrDefault(s => s.Contains("code=")).Split('=')[1];
				return code;
			}

			return string.Empty;
		}

		private async void WebViewOnNavigated(object sender, WebNavigatedEventArgs e)
		{

			var accessToken = ExtractAccessTokenFromUrl(e.Url);

			if (accessToken != "")
			{
				//Loadding.IsRunning = true;
				bool resgisted = await new UserViewModel().LoginUserFacebook(accessToken);
                if (resgisted)
                {
                    //Loadding.IsRunning = false;
                    await Navigation.PushModalAsync(new NavigationPage(new FastVehicleView()));
                }
                else
                {
                    //Loadding.IsRunning = false;
                    await DisplayAlert("Alert", "Error al iniciar sesión", "OK"); password.Text = "";
                }
			}
		}

		private string ExtractAccessTokenFromUrl(string url)
		{
			if (url.Contains("access_token") && url.Contains("&expires_in="))
			{
				var at = url.Replace("https://securtraffic-49c23.firebaseapp.com/__/auth/handler?#access_token=", "");
				if (OS == TargetPlatform.WinPhone || OS == TargetPlatform.Windows)
				{
					at = url.Replace("http://www.facebook.com/connect/login_success.html?#access_token=", "");
				}

				var accessToken = at.Remove(at.IndexOf("&expires_in="));

				return accessToken;
			}

			return string.Empty;
		}
	}
}
