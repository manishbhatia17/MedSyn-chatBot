using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
    public class CountryMapping : ClassMap<Country>
    {
        public CountryMapping()
        {
            Table("Country");

            Id(x => x.Id)
                .Column("Id")
                .GeneratedBy.Identity();

            Map(x => x.Name);

            References(x => x.Territory)
                .Column("TerritoryId");

            References(x => x.Region)
                .Column("RegionId");
        }
    }
}