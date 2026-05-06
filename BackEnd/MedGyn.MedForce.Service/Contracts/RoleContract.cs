using System.Collections.Generic;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class RoleContract
	{
		public RoleContract()
		{

		}

		public RoleContract(Role role)
		{
			if(role == null) {
				return;
			}

			RoleId = role.RoleId;
			RoleName = role.RoleName;
			IsDeleted = role.IsDeleted;
			IsArchived = role.IsArchived;
		}

		public int RoleId { get; set; }
		public string RoleName { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsArchived { get; set; }

		public Role ToModel()
		{
			return new Role()
			{
				RoleId = this.RoleId,
				RoleName = this.RoleName,
				IsDeleted = this.IsDeleted,
				IsArchived = this.IsArchived
			};
		}
	}
}
