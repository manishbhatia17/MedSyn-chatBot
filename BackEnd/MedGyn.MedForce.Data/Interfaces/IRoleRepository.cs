using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Interfaces
{
	public interface IRoleRepository
	{
		IEnumerable<Role> GetAllRoles();
		IList<dynamic> GetRolesList(string search, string sortCol, bool sortAsc);
		Role GetRole(int? roleId);
		IList<dynamic> GetSecurityKeysForRole(int roleId);
		bool SaveRoles(List<Role> roles);
		bool SaveRolePermissions(int roleId, List<RoleSecurityKey> permissions);
	}
}
