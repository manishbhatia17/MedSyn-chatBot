using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IRoleService
	{
		List<RoleContract> GetAllRoles();
		RoleContract GetRole(int? roleId);
		IList<dynamic> GetRolesList(string search, string sortCol, bool sortAsc);
		IList<dynamic> GetSecurityKeysForRole(int roleId);
		bool SaveRoles(List<RoleContract> roles);
		bool SaveRolePermissions(int roleId, List<RoleSecurityKeyContract> permissions);
	}
}
