using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
    public class RegionMapping : ClassMap<Region>
    {
        public RegionMapping()
        {
            Table("Region");

            Id(x => x.Id)
                .Column("Id")
                .GeneratedBy.Identity();

            Map(x => x.Name);
        }
    }
}