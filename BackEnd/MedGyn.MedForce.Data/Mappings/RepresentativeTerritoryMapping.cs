using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
    public class RepresentativeTerritoryMapping : ClassMap<RepresentativeTerritory>
    {
        public RepresentativeTerritoryMapping()
        {
            Table("RepresentativeTerritory");

            Id(x => x.Id)
                .Column("Id")
                .GeneratedBy.Identity();

            References(x => x.Representative)
                .Column("RepresentativeId");

            References(x => x.Territory)
                .Column("TerritoryId");
        }
    }
}