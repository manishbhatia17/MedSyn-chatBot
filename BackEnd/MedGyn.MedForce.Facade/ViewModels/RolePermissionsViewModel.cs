using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class RolePermissionsViewModel
	{
		public int RoleId { get; set; }
		public List<RoleSecurityKeyViewModel> roleSecurityKeys { get; set; }

		public List<RoleSecurityKeyContract> SecurityKeyContracts(){
			var contracts = new List<RoleSecurityKeyContract>();
			foreach(var securityKey in this.roleSecurityKeys)
			{
				if(securityKey.IsSelected)
					contracts.Add(new RoleSecurityKeyContract{ RoleId = this.RoleId, SecurityKeyId = securityKey.SecuritKeyId});
			}
			return contracts;
		}

	}
}
