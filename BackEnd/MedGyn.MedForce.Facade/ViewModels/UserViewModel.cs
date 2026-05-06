using System.Collections.Generic;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class UserViewModel
	{
		public UserViewModel()
		{
			Errors = new List<string>();
		}

		public UserViewModel(UserContract user)
		{
			UserId     = user.UserId;
			Email      = user.Email;
			FirstName  = user.FirstName;
			LastName   = user.LastName;
			IsDeleted  = user.IsDeleted;
			RoleId     = user.RoleId ?? 0;
			Errors     = new List<string>();
			SalesRepId = user.SalesRepID;
		}

		public int UserId { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool IsDeleted { get; set; }
		public int RoleId { get; set; }
		public string SalesRepId { get; set; }

		public List<string> Errors { get; set; }
		public bool Success => Errors.Count == 0;

		private const string defaultPasswordHolder = "********";

		public UserContract ToContract()
		{
			return new UserContract()
			{
				UserId     = this.UserId,
				Email      = this.Email,
				FirstName  = this.FirstName,
				LastName   = this.LastName,
				IsDeleted  = this.IsDeleted,
				RoleId     = this.RoleId,
				SalesRepID = this.SalesRepId
			};
		}
	}
}
