using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class RoleViewModel
	{
		public RoleViewModel()
		{
			Errors = new List<string>();
		}

		public RoleViewModel(RoleContract role)
		{
			RoleId     = role.RoleId;
			RoleName   = role.RoleName;
			IsDeleted  = role.IsDeleted;
			IsArchived = role.IsArchived;

			Errors = new List<string>();
		}

		public RoleViewModel(dynamic results)
		{
			RoleId = results.RoleId;
			RoleName = results.RoleName;
			IsArchived = results.IsArchived;
			UserCount = results.UserCount;
			KeyCount = results.KeyCount;

			Errors = new List<string>();
		}

		public int RoleId { get; set; }
		public string RoleName { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsArchived { get; set; }
		public int UserCount { get; set; }
		public int KeyCount { get; set; }

		public List<string> Errors { get; set; }
		public bool Success => Errors.Count == 0;

		public RoleContract ToContract()
		{
			return new RoleContract()
			{
				RoleId     = this.RoleId,
				RoleName   = this.RoleName,
				IsDeleted  = this.IsDeleted,
				IsArchived = this.IsArchived
			};
		}
	}
}
