using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class PurchaseOrderProductMapping : ClassMap<PurchaseOrderProduct>
	{
		public PurchaseOrderProductMapping()
		{
			Table("[PurchaseOrderProduct]");
			Id(x => x.PurchaseOrderProductID)
				.Column("PurchaseOrderProductID")
				.GeneratedBy.Identity();

			Map(x => x.PurchaseOrderID);
			Map(x => x.ProductID);
			Map(x => x.UnitOfMeasureCodeID);
			Map(x => x.Quantity);
			Map(x => x.Price);
		}
	}
}
