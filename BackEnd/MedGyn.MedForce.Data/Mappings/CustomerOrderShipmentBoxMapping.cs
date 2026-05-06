using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class CustomerOrderShipmentBoxMapping : ClassMap<CustomerOrderShipmentBox>
	{
		public CustomerOrderShipmentBoxMapping()
		{
			Table("[CustomerOrderShipmentBox]");
			Id(x => x.CustomerOrderShipmentBoxID)
				.Column("CustomerOrderShipmentBoxID")
				.GeneratedBy.Identity();
				
			Map(x => x.CustomerOrderShipmentID);
			Map(x => x.CustomerOrderID);
			Map(x => x.Weight);
			Map(x => x.WeightUnitCodeID);
			Map(x => x.Length);
			Map(x => x.Width);
			Map(x => x.Depth);
			Map(x => x.DimensionUnitCodeID);
		}
	}
}

