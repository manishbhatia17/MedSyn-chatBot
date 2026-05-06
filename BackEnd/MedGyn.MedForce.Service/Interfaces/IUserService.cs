using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IUserService
	{
		List<UserContract> GetAllUsers();
		ValueTask<List<UserContract>> GetAllUsersAsync();

        IList<dynamic> GetUserList(string search, string sortCol, bool sortAsc);
		IList<UserContract> GetUsersWithKey(SecurityKeyEnum[] securityKeys);
		Dictionary<int, UserContract> GetUsersDictionary();
		UserContract GetUser(int userId);
		UserContract GetUser(string email);
		List<RoleContract> GetAllRoles();
		UserContract SaveUser(UserContract user);
		void ResetPassword(UserContract user);
		bool ValidateEmail(int userID, string email);
		UserContract DeleteUser(UserContract updatedUser);                                                                                                                                                                     
	}
}
