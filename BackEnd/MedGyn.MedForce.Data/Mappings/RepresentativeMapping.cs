using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
    public class RepresentativeMapping : ClassMap<Representative>
    {
        public RepresentativeMapping()
        {
            Table("Representative");

            Id(x => x.Id)
                .Column("Id")
                .GeneratedBy.Identity();

            Map(x => x.Name);

            Map(x => x.Phone);

            Map(x => x.Email);
        }
    }
}