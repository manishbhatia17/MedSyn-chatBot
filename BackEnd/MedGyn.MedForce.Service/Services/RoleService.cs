using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
	public class RoleService : IRoleService
	{
		private readonly IRoleRepository _roleRepository;

		public RoleService(IRoleRepository roleRepository)
		{
			_roleRepository = roleRepository;
		}

		public List<RoleContract> GetAllRoles()
		{
			var dbRoles = _roleRepository.GetAllRoles();
			return dbRoles.Select(r => new RoleContract(r)).ToList();
		}

		public IList<dynamic> GetRolesList(string search, string sortCol, bool sortAsc)
		{
			return _roleRepository.GetRolesList(search, sortCol, sortAsc);
		}
		public RoleContract GetRole(int? roleId)
		{
			var role = _roleRepository.GetRole(roleId);
			if (role == null)
			{
				return null;
			}

			return new RoleContract(role);
		}
		public IList<dynamic> GetSecurityKeysForRole(int roleId)
		{
			return _roleRepository.GetSecurityKeysForRole(roleId);
		}

		public bool SaveRoles(List<RoleContract> roles)
		{
			return _roleRepository.SaveRoles(roles.Select(r => r.ToModel()).ToList());
		}
		public bool SaveRolePermissions(int roleId, List<RoleSecurityKeyContract> permissions)
		{
			return _roleRepository.SaveRolePermissions(roleId, permissions.Select(r => r.ToModel()).ToList());
		}
	}
}
