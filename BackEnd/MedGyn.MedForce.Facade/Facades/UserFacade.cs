using System.Collections.Generic;
using System.Linq;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Facade.Facades
{
	public class UserFacade : IUserFacade
	{
		private readonly IUserService _userService;

		public UserFacade(IUserService userService)
		{
			_userService = userService;
		}

		public bool IsAuthorized(string url, int userId)
		{
			return true;
		}

		public UserViewModel GetUser(int userId)
		{
			var user = _userService.GetUser(userId);
			return new UserViewModel(user);
		}

		public UserListViewModel GetCustomerListViewModel(SearchCriteriaViewModel sc)
		{
			var users = _userService.GetUserList(sc.Search, sc.SortColumn, sc.SortAsc);

			var userViewModels = users.Select(c => new UserDisplayViewModel(c)).ToList();

			return new UserListViewModel (sc, userViewModels);
		}

		public List<RoleViewModel> GetActiveRoles()
		{
			return _userService.GetAllRoles().Where(r => !(r.IsArchived || r.IsDeleted)).Select(r => new RoleViewModel(r)).ToList();
		}

		public SaveResults SaveUser(UserDisplayViewModel user)
		{
			var isValidEmail = _userService.ValidateEmail(user.UserId, user.Email);
			if(!isValidEmail)
				return new SaveResults("DUP_ID");

			var contract = _userService.SaveUser(user.ToContract());
			return new SaveResults();
		}

		public SaveResults DeleteUser(UserDisplayViewModel user)
		{
			var isValidEmail = _userService.ValidateEmail(user.UserId, user.Email);
			if (!isValidEmail)
				return new SaveResults("DUP_ID");

			var contract = _userService.DeleteUser(user.ToContract());
			return new SaveResults();
		}
	}
}
