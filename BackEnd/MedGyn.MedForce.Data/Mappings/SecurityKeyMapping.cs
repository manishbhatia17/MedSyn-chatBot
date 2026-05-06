using MedGyn.MedForce.Data.Models;
using FluentNHibernate.Mapping;

namespace MedGyn.MedForce.Data.Mappings
{
    public class SecurityKeyMapping : ClassMap<SecurityKey>
    {
        public SecurityKeyMapping()
        {
            Table("[SecurityKey]");

            Id(x => x.SecurityKeyId)
                .Column("SecurityKeyId")
                .GeneratedBy.Identity();

            Map(x => x.SecurityKeyName);
            Map(x => x.IsDeleted);

            HasManyToMany(x => x.Roles)
                .Cascade.All()
                .Inverse()
                .Table("RoleSecurityKey")
                .ParentKeyColumn("SecurityKeyId")
                .ChildKeyColumn("RoleId");
        }
    }
}
