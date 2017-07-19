using System;
using System.Threading.Tasks;
using Firebase.Xamarin.Auth;

namespace SecureTraffic
{
	public class UserService : ServiceBase
	{
		private FirebaseAuthProvider _authProvider;

		public UserService()
		{
            this._authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAacfnIMLhrzOLUKAE4cZ-GvWJR1v1OiQM"));
		}

		public async Task<FirebaseAuthLink> RegisterUser(string email, string password)
		{
			FirebaseAuthLink auth = null;
			try
			{
				auth = await this._authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error al registrar un usuario: "+ex.Message);
			}
			return auth;
		}

		public async Task<FirebaseAuthLink> RegisteGoogleUser()
		{
			FirebaseAuthLink auth = null;
			try
			{
				auth = await this._authProvider.CreateUserWithEmailAndPasswordAsync("","");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error al registrar un usuario: "+ex.Message);
			}
			return auth;
		}

		public async Task<FirebaseAuthLink> LoginUser(string email, string password)
		{
			FirebaseAuthLink auth = null;
			try
			{
				auth = await this._authProvider.SignInWithEmailAndPasswordAsync(email, password);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error al loguear a un usuario: "+ex.Message);
			}
			return auth;
		}

		public async Task<FirebaseAuthLink> LoginUserGoogle()
		{
			FirebaseAuthLink auth = null;
			try
			{
				auth = await this._authProvider.SignInAnonymouslyAsync();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error al loguear a un usuario: " + ex.Message);
			}
			return auth;
		}

		public async Task<FirebaseAuthLink> LoginUserFacebook()
		{
			FirebaseAuthLink auth = null;
			try
			{
				auth = await this._authProvider.SignInAnonymouslyAsync();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error al loguear a un usuario: " + ex.Message);
			}
			return auth;
		}
	}
}