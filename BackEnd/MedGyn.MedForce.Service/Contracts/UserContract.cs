using System;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class UserContract
	{
		public UserContract()
		{

		}

		public UserContract(User user)
		{
			UserId             = user.UserId;
			Email              = user.Email;
			Password           = user.Password;
			FirstName          = user.FirstName;
			LastName           = user.LastName;
			IsDeleted          = user.IsDeleted;
			PasswordSalt       = user.PasswordSalt;
			ForcePasswordReset = user.ForcePasswordReset;
			ResetPasswordOn    = user.ResetPasswordOn;
			RoleId             = user.RoleId;
			Role               = new RoleContract(user.Role);
			SalesRepID         = user.SalesRepID;
			IsHidden = user.IsHidden;
		}

		public string FullName => $"{FirstName} {LastName}".Trim();

		public int UserId { get; set; }
		public string Email { get; set; }
		public byte[] Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int CompanyEntityId { get; set; }
		public bool IsDeleted { get; set; }
		public byte[] PasswordSalt { get; set; }
		public string PasswordString { get; set; }
		public bool ForcePasswordReset { get; set; }
		public DateTime? ResetPasswordOn { get; set; }
		public int? RoleId { get; set; }
		public int? TeamId { get; set; }
		public DateTime? InvitationIssuedOn { get; set; }
		public int CompanyId { get; set; }
		public RoleContract Role { get; set; }
		public string SalesRepID { get; set; }
		public bool IsHidden { get; set; }

		public User ToModel(User curUser)
		{
			if(curUser == null)
				curUser = new User() { UserId = UserId };

			curUser.Email      = Email;
			curUser.FirstName  = FirstName;
			curUser.LastName   = LastName;
			curUser.IsDeleted  = IsDeleted;
			curUser.RoleId     = RoleId;
			curUser.SalesRepID = SalesRepID;
			curUser.IsHidden = IsHidden;
		

			return curUser;
		}
	}
}
