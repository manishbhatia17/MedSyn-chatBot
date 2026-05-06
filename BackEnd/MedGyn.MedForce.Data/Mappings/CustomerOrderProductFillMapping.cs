using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class CustomerOrderProductFillMapping : ClassMap<CustomerOrderProductFill>
	{
		public CustomerOrderProductFillMapping()
		{
			Table("[CustomerOrderProductFill]");
			Id(x => x.CustomerOrderProductFillID)
				.Column("CustomerOrderProductFillID")
				.GeneratedBy.Identity();

			Map(x => x.CustomerOrderProductID);
			Map(x => x.QuantityPacked);
			Map(x => x.SerialNumbers);
			Map(x => x.CustomerOrderShipmentBoxID);
		}
	}
}
