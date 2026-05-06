using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class PurchaseOrderMapping : ClassMap<PurchaseOrder>
	{
		public PurchaseOrderMapping()
		{
			Table("[PurchaseOrder]");
			Id(x => x.PurchaseOrderID)
				.Column("PurchaseOrderID")
				.GeneratedBy.Identity();

			Map(x => x.PurchaseOrderCustomID);
			Map(x => x.SubmitDate);
			Map(x => x.ApprovalDate);
			Map(x => x.ApprovedBy);
			Map(x => x.VendorID);
			Map(x => x.VendorOrderNumber);
			Map(x => x.ExpectedDate);
			Map(x => x.ShipCompanyType);
			Map(x => x.ShipChoiceCodeID);
			Map(x => x.IsPartialShipAcceptable);
			Map(x => x.ShippingCharge);
			Map(x => x.Notes);
			Map(x => x.UpdatedBy);
			Map(x => x.UpdatedOn);
			Map(x => x.IsDoNotReceive);
			Map(x => x.DoNotReceiveReason);
			Map(x => x.CreatedBy);
			Map(x => x.CreatedOn);
			Map(x => x.ReceivedBy);
			Map(x => x.ReceivedOn);
		}
	}
}
