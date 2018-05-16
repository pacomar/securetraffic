using System;
using System.Threading.Tasks;
using Firebase.Xamarin.Auth;
using SecureTraffic.Helpers;

namespace SecureTraffic
{
	public class UserViewModel
	{
		private UserService _userService;
		public UserViewModel()
		{
			this._userService = new UserService();
		}

		public async Task<bool> RegisterUser(string email, string password)
		{
			FirebaseAuthLink auth = await this._userService.RegisterUser(email, password);

			return auth != null;
		}

        public async Task<bool> RecoverPassword(string email)
        {
            return await this._userService.RecoverPassword(email);
        }

		public async Task<bool> ResgisterWithGoogle()
		{
			FirebaseAuthLink auth = await this._userService.RegisteGoogleUser();

			return auth != null;
		}

		public async Task<bool> LoginUser(string email, string password)
		{
			bool res = false;

			FirebaseAuthLink auth = await this._userService.LoginUser(email, password);

			if (auth != null)
			{
				App.token = auth.FirebaseToken;
				Settings.Token = App.token;
				res = true;
			}

			return res;
		}

		public async Task<bool> LoginUserGoogle(string token)
		{
			bool res = false;

			FirebaseAuthLink auth = await this._userService.LoginUserGoogle(token);

			if (auth != null)
			{
				App.token = auth.FirebaseToken;
				Settings.Token = App.token;
				res = true;
			}

			return res;
		}

		public async Task<bool> LoginUserFacebook(string token)
		{
			bool res = false;

			FirebaseAuthLink auth = await this._userService.LoginUserFacebook(token);

			if (auth != null)
			{
				App.token = auth.FirebaseToken;
				Settings.Token = App.token;
				res = true;
			}

			return res;
		}
	}
}
