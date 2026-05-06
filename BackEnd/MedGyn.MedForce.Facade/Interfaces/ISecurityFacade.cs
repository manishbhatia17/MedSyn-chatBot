using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MedGyn.MedForce.Facade.ViewModels;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface ISecurityFacade
	{
		bool IsAuthorized(SecurityKeyEnum securityKey);
		bool IsAuthorized(List<SecurityKeyEnum> securityKeys);
		int GetUserId();
		RoleViewModel GetRole(int userId);
		RoleListViewModel GetRolesList(SearchCriteriaViewModel sc);
		List<RoleSecurityKeyViewModel> GetSecurityKeysForRole(int roleId);
		bool SaveRoles(List<RoleViewModel> roles);
		bool SaveRolePermissions(RolePermissionsViewModel permissionsViewModel);
	}
}
