using System;
using System.Threading.Tasks;

namespace SecureTraffic
{
	public class UserViewModel
	{
		public UserViewModel()
		{
		}

		public async Task<bool> RegisterUser(string email, string password)
		{
			UserService _userServ = new UserService();

			string token = await _userServ.RegisterUser(email, password);

			return token != "";
		}
	}
}
