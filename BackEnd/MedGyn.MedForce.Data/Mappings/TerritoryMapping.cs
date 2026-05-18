using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
    public class TerritoryMapping : ClassMap<Territory>
    {
        public TerritoryMapping()
        {
            Table("Territory");

            Id(x => x.Id)
                .GeneratedBy.Identity();

            Map(x => x.Code);

            Map(x => x.Name);

            Map(x => x.Type);

            HasMany(x => x.States)
                .KeyColumn("TerritoryId")
                .Inverse()
                .Cascade.None();

            HasMany(x => x.Countries)
                .KeyColumn("TerritoryId")
                .Inverse()
                .Cascade.None();
        }
    }
}