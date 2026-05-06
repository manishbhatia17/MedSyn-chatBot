using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class RoleSecurityKeyViewModel
	{		
		public RoleSecurityKeyViewModel(){}

		public int SecuritKeyId { get; set; }
		public string SecurityKeyName { get; set; }
		public string SecurityKeyDescription { get; set; }
		public bool IsSelected { get; set; }
		
		public RoleSecurityKeyViewModel(dynamic results)
		{
			SecuritKeyId = results.SecurityKeyId;
			SecurityKeyName = results.SecurityKeyName;
			SecurityKeyDescription = results.SecurityKeyDescription;
			IsSelected = results.IsSelected == 1;
		}
	}
}
