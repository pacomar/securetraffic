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
            this._authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBqQNY_fYsywexh4-NMniGdxT_6lNp6Pgs"));
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

        public async Task<bool> RecoverPassword(string email)
        {
            try
            {
                await this._authProvider.SendPasswordResetEmailAsync(email);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al recuperar contraseña: " + ex.Message);
            }
            return true;
        }

		public async Task<FirebaseAuthLink> RegisteGoogleUser()
		{
			FirebaseAuthLink auth = null;
			try
			{
				auth = await this._authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google,"448189929340-egnqj2s3hkvfb0v4qi6hdaugl97nu85m.apps.googleusercontent.com");
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

		public async Task<FirebaseAuthLink> LoginUserGoogle(string token)
		{
			FirebaseAuthLink auth = null;
			try
			{
				auth = await this._authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google,token);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error al loguear a un usuario: " + ex.Message);
			}
			return auth;
		}

		public async Task<FirebaseAuthLink> LoginUserFacebook(string token)
		{
			FirebaseAuthLink auth = null;
			try
			{
				auth = await this._authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook,token);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error al loguear a un usuario: " + ex.Message);
			}
			return auth;
		}
	}
}