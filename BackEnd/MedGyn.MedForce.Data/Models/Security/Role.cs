using System.Collections.Generic;

namespace MedGyn.MedForce.Data.Models
{
	public class Role
	{
		public virtual int RoleId { get; set; }
		public virtual string RoleName { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual bool IsArchived { get; set; }
	}
}
