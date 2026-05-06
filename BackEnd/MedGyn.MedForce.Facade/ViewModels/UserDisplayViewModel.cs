using System.Collections.Generic;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class UserDisplayViewModel
	{
		public UserDisplayViewModel(){}

		public UserDisplayViewModel(dynamic results)
		{
			UserId     = results.UserId;
			Email      = results.Email;
			FirstName  = results.FirstName;
			LastName   = results.LastName;
			RoleId     = results.RoleId ?? 0;
			RoleName   = results.RoleName;
			SalesRepId = results.SalesRepID;
			IsDeleted  = results.IsDeleted;
			IsHidden = results.IsHidden;
		}

		public int UserId { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int RoleId { get; set; }
		public string RoleName { get; set; }
		public string SalesRepId { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsHidden { get; set; }


		public UserContract ToContract()
		{
			return new UserContract()
			{
				UserId     = UserId,
				Email      = Email,
				FirstName  = FirstName,
				LastName   = LastName,
				RoleId     = RoleId,
				SalesRepID = SalesRepId,
				IsDeleted  = IsDeleted,
				IsHidden = IsHidden
			};
		}
	}
}
