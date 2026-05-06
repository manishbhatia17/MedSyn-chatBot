using System;

namespace MedGyn.MedForce.Data.Models
{
	public class User
	{
		public User() { }

		public virtual int UserId { get; set; }
		public virtual string Email { get; set; }
		public virtual byte[] Password { get; set; }
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual byte[] PasswordSalt { get; set; }
		public virtual bool ForcePasswordReset { get; set; }
		public virtual DateTime? ResetPasswordOn { get; set; }
		public virtual int? RoleId { get; set; }
		public virtual string SalesRepID { get; set; }

		public virtual Role Role { get; set; }
		public virtual bool IsHidden { get; set; }
	}
}
