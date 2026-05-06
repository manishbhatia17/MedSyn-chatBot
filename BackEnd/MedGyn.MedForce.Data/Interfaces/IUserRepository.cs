using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Interfaces
{
	public interface IUserRepository
	{
		List<User> GetAllUsers();
		IList<dynamic> GetUserList(string search, string sortCol, bool sortAsc);
		User GetUser(int userId);
		IList<User> GetUsersWithRoleIn(int[] roleIDs);
		User GetRepForCustomerShippingInfo(int customerShippingInfoID);
		User GetUser(string email);
		List<Role> GetAllRoles();
		Role GetRole(int? roleId);
		Role GetRoleByName(string roleName);
		User SaveUser(User user);
		void ResetPassword(int userId, bool forceReset, byte[] password, byte[] salt, DateTime? resetPasswordOn);
		bool ValidateEmail(int userID, string email);
		void DeleteUser(User user);
	}
}
