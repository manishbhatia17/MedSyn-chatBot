using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class CustomerOrderMapping : ClassMap<CustomerOrder>
	{
		public CustomerOrderMapping()
		{
			Table("[CustomerOrder]");
			Id(x => x.CustomerOrderID)
				.Column("CustomerOrderID")
				.GeneratedBy.Identity();

			Map(x => x.CustomerOrderCustomID);
			Map(x => x.SubmitDate);
			Map(x => x.MGApprovedOn);
			Map(x => x.MGApprovedBy);
			Map(x => x.VPApprovedOn);
			Map(x => x.VPApprovedBy);
			Map(x => x.CustomerID);
			Map(x => x.CustomerShippingInfoID);
			Map(x => x.PONumber);
			Map(x => x.Contact);
			Map(x => x.ShipCompanyType);
			Map(x => x.ShipChoiceCodeID);
			Map(x => x.IsPartialShipAcceptable);
			Map(x => x.IsDoNotFill);
			Map(x => x.ShippingCharge);
			Map(x => x.HandlingCharge);
			Map(x => x.InsuranceCharge);
			Map(x => x.Instructions);
			Map(x => x.Notes);
			Map(x => x.UpdatedBy);
			Map(x => x.UpdatedOn);
			Map(x => x.CreatedBy);
			Map(x => x.CreatedOn);
			Map(x => x.AttachmentURI);
			Map(x => x.DoNotFillReason);
			Map(x => x.IsFilling);
			Map(x => x.IntermediaryShippingName);
			Map(x => x.IntermediaryShippingAddress);
			Map(x => x.IntermediaryShippingContactNumber);
			Map(x => x.IntermediaryShippingContactName);
			Map(x => x.IntermediaryShippingContactEmail);
			Map(x => x.AttachmentFileName);
			Map(x => x.ShippingCustomerName);
			Map(x => x.FilledBy);
			Map(x => x.FilledByOn);
			Map(x => x.ShippedBy);
			Map(x => x.ShippedByOn);
		}
	}
}
