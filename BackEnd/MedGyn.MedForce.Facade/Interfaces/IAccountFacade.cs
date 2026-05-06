using System.Threading.Tasks;
using MedGyn.MedForce.Facade.ViewModels;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface IAccountFacade
	{
		bool IsAuthorized(string url, int userId);
		LoginViewModel Login(LoginViewModel model);
		void Logout();
		ResetPasswordViewModel ResetPassword(ResetPasswordViewModel model);
		ChangePasswordViewModel CheckChangePassword(string email);
		Task<ChangePasswordViewModel> ChangePassword(ChangePasswordViewModel model);
	}
}
