using System.Collections.Generic;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.ViewModels;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface IUserFacade
	{
		bool IsAuthorized(string url, int userId);
		UserViewModel GetUser(int userId);
		UserListViewModel GetCustomerListViewModel(SearchCriteriaViewModel sc);
		List<RoleViewModel> GetActiveRoles();
		SaveResults SaveUser(UserDisplayViewModel user);
		SaveResults DeleteUser(UserDisplayViewModel user);
	}
}
