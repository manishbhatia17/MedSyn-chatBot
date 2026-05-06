using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Facade.Facades
{
	public class SecurityFacade : ISecurityFacade
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly IRoleService _roleService;

		public SecurityFacade(IAuthenticationService authenticationService, IRoleService roleService)
		{
			_authenticationService = authenticationService;
			_roleService = roleService;
		}

		public bool IsAuthorized(SecurityKeyEnum securityKey)
		{
			return IsAuthorized(new List<SecurityKeyEnum> { securityKey });
		}

		public bool IsAuthorized(List<SecurityKeyEnum> securityKeys)
		{
			foreach(var securityKey in securityKeys)
			{
				if (_authenticationService.HasClaim((int)securityKey))
					return true;
			}
			return false;
		}

		public int GetUserId()
		{
			return _authenticationService.GetUserID();
		}

		public RoleViewModel GetRole(int roleId)
		{
			var role = _roleService.GetRole(roleId);
			return new RoleViewModel(role);
		}

		public RoleListViewModel GetRolesList(SearchCriteriaViewModel sc)
		{
			var contractList = _roleService.GetRolesList(sc.Search, sc.SortColumn, sc.SortAsc);
			var roleList = contractList.Select(c => new RoleViewModel(c)).ToList();

			return new RoleListViewModel(sc, roleList);
		}

		public List<RoleSecurityKeyViewModel> GetSecurityKeysForRole(int roleId)
		{
			var keyList = _roleService.GetSecurityKeysForRole(roleId);
			return keyList.Select(k => new RoleSecurityKeyViewModel(k)).ToList();
		}

		public bool SaveRoles(List<RoleViewModel> roles)
		{
			return _roleService.SaveRoles(roles.Select(r => r.ToContract()).ToList());
		}

		public bool SaveRolePermissions(RolePermissionsViewModel rolePermissions)
		{
			return _roleService.SaveRolePermissions(rolePermissions.RoleId, rolePermissions.SecurityKeyContracts());
		}
	}
}
