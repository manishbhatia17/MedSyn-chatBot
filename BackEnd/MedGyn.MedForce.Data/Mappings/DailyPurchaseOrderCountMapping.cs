using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class DailyPurchaseOrderCountMapping : ClassMap<DailyPurchaseOrderCount>
	{
		public DailyPurchaseOrderCountMapping()
		{
			Table("[DailyPurchaseOrderCount]");
			Id(x => x.VendorID).GeneratedBy.Assigned();

			Map(x => x.LastCreated);
			Map(x => x.DailyCount);
		}
	}
}
