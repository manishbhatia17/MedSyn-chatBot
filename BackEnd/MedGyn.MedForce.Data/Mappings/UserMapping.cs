using MedGyn.MedForce.Data.Models;
using FluentNHibernate.Mapping;

namespace MedGyn.MedForce.Data.Mappings
{
	public class UserMapping : ClassMap<User>
	{
		public UserMapping()
		{
			Table("[User]");
			Id(x => x.UserId)
				.Column("UserId")
				.GeneratedBy.Identity();

			Map(x => x.FirstName);
			Map(x => x.LastName);
			Map(x => x.Email);
			Map(x => x.IsDeleted);
			Map(x => x.Password);
			Map(x => x.PasswordSalt);
			Map(x => x.ForcePasswordReset);
			Map(x => x.ResetPasswordOn);
			Map(x => x.SalesRepID);
			Map(x => x.IsHidden);

			References(x => x.Role)
				.Column("RoleId");
		}
	}
}
