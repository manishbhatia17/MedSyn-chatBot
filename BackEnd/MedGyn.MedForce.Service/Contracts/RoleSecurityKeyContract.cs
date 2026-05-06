using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class RoleSecurityKeyContract
	{
		public RoleSecurityKeyContract(){ }

		public RoleSecurityKeyContract(RoleSecurityKey rsk)
		{
			RoleSecurityKeyId = rsk.RoleSecurityKeyId;
			RoleId = rsk.RoleId;
			SecurityKeyId = rsk.SecurityKeyId;
		}

		public int RoleSecurityKeyId { get; set; }
		public int RoleId { get; set; }
		public int SecurityKeyId { get; set; }

		public RoleSecurityKey ToModel()
		{
			return new RoleSecurityKey()
			{
				RoleSecurityKeyId = this.RoleSecurityKeyId,
				RoleId = this.RoleId,
				SecurityKeyId = this.SecurityKeyId
			};
		}
	}
}
