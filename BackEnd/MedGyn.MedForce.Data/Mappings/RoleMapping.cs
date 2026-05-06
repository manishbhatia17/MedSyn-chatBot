using MedGyn.MedForce.Data.Models;
using FluentNHibernate.Mapping;

namespace MedGyn.MedForce.Data.Mappings
{
    public class RoleMapping : ClassMap<Role>
    {
        public RoleMapping()
        {
            Table("[Role]");

            Id(x => x.RoleId)
                .Column("RoleId")
                .GeneratedBy.Identity();

            Map(x => x.RoleName);
            Map(x => x.IsDeleted);
            Map(x => x.IsArchived);
        }
    }
}
