using System;

namespace MedGyn.MedForce.Data.Models
{
	public class CustomerOrder
	{
		public virtual int CustomerOrderID { get; set; }
		public virtual string CustomerOrderCustomID { get; set; }
		public virtual DateTime? SubmitDate { get; set; }
		public virtual DateTime? MGApprovedOn { get; set; }
		public virtual int? MGApprovedBy { get; set; }
		public virtual DateTime? VPApprovedOn { get; set; }
		public virtual int? VPApprovedBy { get; set; }
		public virtual int CustomerID { get; set; }
		public virtual int CustomerShippingInfoID { get; set; }
		public virtual string PONumber { get; set; }
		public virtual string AttachmentURI { get; set; }
		public virtual string Contact { get; set; }
		public virtual int? ShipCompanyType { get; set; }
		public virtual int? ShipChoiceCodeID { get; set; }
		public virtual bool IsPartialShipAcceptable { get; set; }
		public virtual bool IsDoNotFill { get; set; }
		public virtual decimal? ShippingCharge { get; set; }
		public virtual decimal? HandlingCharge { get; set; }
		public virtual decimal? InsuranceCharge { get; set; }
		public virtual string Instructions { get; set; }
		public virtual string Notes { get; set; }
		public virtual int UpdatedBy { get; set; }
		public virtual DateTime UpdatedOn { get; set; }
		public virtual int CreatedBy { get; set; }
		public virtual DateTime CreatedOn { get; set; }
		public virtual string DoNotFillReason { get; set; }
		public virtual bool IsFilling { get; set; }
		public virtual string IntermediaryShippingName { get; set; }
		public virtual string IntermediaryShippingAddress { get; set; }
		public virtual string IntermediaryShippingContactNumber { get; set; }
		public virtual string IntermediaryShippingContactName { get; set; }
		public virtual string IntermediaryShippingContactEmail { get; set; }
		public virtual string AttachmentFileName { get; set; }
		public virtual string ShippingCustomerName { get; set; }
		public virtual int FilledBy { get; set; }
		public virtual DateTime? FilledByOn { get; set; }
		public virtual bool FinancingApproved { get; set; }
		public virtual DateTime FinancingApprovedOn { get; set; }
		public virtual int FinancingApprovedBy { get; set; }
		public virtual int? ShippedBy { get; set; }
		public virtual DateTime? ShippedByOn { get; set; }

		public CustomerOrder()
		{
			//SubmitDate = DateTime.UtcNow;
			UpdatedOn = DateTime.UtcNow;
			CreatedOn = DateTime.UtcNow;
			IsPartialShipAcceptable = true;
			IsDoNotFill = false;
			IsFilling = false;
			FinancingApproved = false;
			ShippingCharge = 0.00M;
			HandlingCharge = 0.00M;
			InsuranceCharge = 0.00M;
			UpdatedBy = 0; // Default value, should be set by the application logic
			CreatedBy = 0; // Default value, should be set by the application logic
		}

		public CustomerOrder(dynamic result)
		{
			CustomerOrderID = result.CustomerOrderID;
			CustomerOrderCustomID = result.CustomerOrderCustomID;
			SubmitDate = result.SubmitDate;
			MGApprovedOn = result.MGApprovedOn;
			MGApprovedBy = result.MGApprovedBy;
			VPApprovedOn = result.VPApprovedOn;
			VPApprovedBy = result.VPApprovedBy;
			CustomerID = result.CustomerID;
			CustomerShippingInfoID = result.CustomerShippingInfoID;
			PONumber = result.PONumber;
			AttachmentURI = result.AttachmentURI;
			Contact = result.Contact;
			ShipCompanyType = result.ShipCompanyType;
			ShipChoiceCodeID = result.ShipChoiceCodeID;
			IsPartialShipAcceptable = result.IsPartialShipAcceptable;
			IsDoNotFill = result.IsDoNotFill;
			ShippingCharge = result.ShippingCharge;
			HandlingCharge = result.HandlingCharge;
			InsuranceCharge = result.InsuranceCharge;
			Instructions = result.Instructions;
			Notes = result.Notes;
			UpdatedBy = result.UpdatedBy;
			UpdatedOn = result.UpdatedOn;
			CreatedBy = result.CreatedBy;
			CreatedOn = result.CreatedOn;
			DoNotFillReason = result.DoNotFillReason;
			IsFilling = result.IsFilling;
			IntermediaryShippingName = result.IntermediaryShippingName ?? string.Empty; // Handle null values
			IntermediaryShippingAddress = result.IntermediaryShippingAddress ?? string.Empty; // Handle null values
			IntermediaryShippingContactNumber = result.IntermediaryShippingContactNumber ?? string.Empty; // Handle null values
			IntermediaryShippingContactName = result.IntermediaryShippingContactName ?? string.Empty; // Handle null values
			IntermediaryShippingContactEmail = result.IntermediaryShippingContactEmail ?? string.Empty; // Handle null values
			AttachmentFileName = result.AttachmentFileName ?? string.Empty; // Handle null values
			ShippingCustomerName = result.ShippingCustomerName ?? string.Empty; // Handle null values
			FilledBy = 0; // Default value, should be set by the application logic
			FilledByOn = null; // Default value, should be set by the application
		}
	}
}
