using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class CustomerOrderProductMapping : ClassMap<CustomerOrderProduct>
	{
		public CustomerOrderProductMapping()
		{
			Table("[CustomerOrderProduct]");
			Id(x => x.CustomerOrderProductID)
				.Column("CustomerOrderProductID")
				.GeneratedBy.Identity();

			Map(x => x.CustomerOrderID);
			Map(x => x.ProductID);
			Map(x => x.UnitOfMeasureCodeID);
			Map(x => x.Quantity);
			Map(x => x.Price);
		}
	}
}
