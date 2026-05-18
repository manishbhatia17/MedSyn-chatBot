using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
    public class StateMapping : ClassMap<State>
    {
        public StateMapping()
        {
            Table("State");

            Id(x => x.Id)
                .Column("Id")
                .GeneratedBy.Identity();

            Map(x => x.Name);

            References(x => x.Territory)
                .Column("TerritoryId");
        }
    }
}