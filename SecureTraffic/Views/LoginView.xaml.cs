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

namespace SecureTraffic
{
	public partial class LoginView : ContentPage
	{
		private GoogleViewModel _googleViewModel { get; set; }

		public LoginView()
		{
            try
            {
                InitializeComponent();

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
                        //    var authRequest =
                        //"https://accounts.google.com/o/oauth2/v2/auth"
                        //+ "?response_type=code"
                        //+ "&scope=openid"
                        //+ "&redirect_uri=" + "https://securtraffic-49c23.firebaseapp.com/__/auth/handler"
                        //+ "&client_id=" + "448189929340-egnqj2s3hkvfb0v4qi6hdaugl97nu85m.apps.googleusercontent.com";

                        //Device.OpenUri(new Uri(authRequest));
                        _googleViewModel = BindingContext as GoogleViewModel;

                        var authRequest =
                        "https://accounts.google.com/o/oauth2/v2/auth"
                        + "?response_type=code"
                        + "&scope=openid"
                        + "&redirect_uri=" + "https://securtraffic-49c23.firebaseapp.com/__/auth/handler"
                        + "&client_id=" + "448189929340-egnqj2s3hkvfb0v4qi6hdaugl97nu85m.apps.googleusercontent.com";

                        var webView = new WebView
                        {
                            Source = authRequest,
                            HeightRequest = 1
                        };

                        webView.Navigated += WebViewOnNavigatedGoogle;

                        Content = webView;
                    }
                    };

                DoLoginFacebook.Clicked += async (sender, args) =>
                {
                    bool comprobarPermisos = await ComprobarPermisos();
                    if (comprobarPermisos)
                    {
                        var apiRequest =
                    "https://www.facebook.com/dialog/oauth?client_id="
                    + "1949845268596950"
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
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se ha podido realizar el login", "OK");
                return false;
            }
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
