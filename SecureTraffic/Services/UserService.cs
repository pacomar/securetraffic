using System.Threading.Tasks;
using Firebase.Xamarin.Auth;

namespace SecureTraffic
{
	public class UserService : ServiceBase
	{
		public UserService()
		{
		}

		public async Task<string> RegisterUser(string email, string password)
		{
			var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAacfnIMLhrzOLUKAE4cZ-GvWJR1v1OiQM"));

			var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

			return auth.FirebaseToken;
		}
	}
}