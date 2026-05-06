using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class CustomerOrderShipmentMapping : ClassMap<CustomerOrderShipment>
	{
		public CustomerOrderShipmentMapping()
		{
			Table("[CustomerOrderShipment]");
			Id(x => x.CustomerOrderShipmentID)
				.Column("CustomerOrderShipmentID")
				.GeneratedBy.Identity();
				
			Map(x => x.CustomerOrderID);
			Map(x => x.ShipCompanyType);
			Map(x => x.ShipMethodCodeID);
			Map(x => x.ShipAccountNumber);
			Map(x => x.ShipmentComplete);
			Map(x => x.FillOption);
			Map(x => x.NumberOfSameBoxes);
			Map(x => x.NumberOfPackingSlips);
			Map(x => x.ShippingCharge);
			Map(x => x.CreatedBy);
			Map(x => x.CreatedOn);
			Map(x => x.InvoiceNumber);
			Map(x => x.InvoiceDate);
			Map(x => x.InvoiceTotal);
			Map(x => x.MasterTrackingNumber);
			Map(x => x.InvoiceSent);
			Map(x => x.PeachTreeExportBatchID);
			Map(x => x.DeliveryDate);
		}
	}
}

