using MedGyn.MedForce.Data.Models;
using FluentNHibernate.Mapping;

namespace MedGyn.MedForce.Data.Mappings
{
	public class ProductInventoryAdjustmentMapping : ClassMap<ProductInventoryAdjustment>
	{
		public ProductInventoryAdjustmentMapping()
		{
			Table("[ProductInventoryAdjustment]");
			Id(x => x.ProductInventoryAdjustmentID)
				.Column("ProductInventoryAdjustmentID")
				.GeneratedBy.Identity();

			Map(x => x.ProductID);
			Map(x => x.Quantity);
			Map(x => x.ReasonCodeID);
			Map(x => x.ReasonCodeOther);
			Map(x => x.AdjustedBy);
			Map(x => x.AdjustmentDate);
		}
	}
}
