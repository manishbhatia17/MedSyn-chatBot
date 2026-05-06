using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class DailyCustomerOrderCountMapping : ClassMap<DailyCustomerOrderCount>
	{
		public DailyCustomerOrderCountMapping()
		{
			Table("[DailyCustomerOrderCount]");
			Id(x => x.CustomerID).GeneratedBy.Assigned();

			Map(x => x.LastCreated);
			Map(x => x.DailyCount);
		}
	}
}
