using System;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CustomerOrderContract
	{

		public CustomerOrderContract() { }
		public CustomerOrderContract(CustomerOrder customerOrder)
		{
			CustomerOrderID                   = customerOrder.CustomerOrderID;
			CustomerOrderCustomID             = customerOrder.CustomerOrderCustomID;
			SubmitDate                        = customerOrder.SubmitDate;
			MGApprovedOn                      = customerOrder.MGApprovedOn;
			MGApprovedBy                      = customerOrder.MGApprovedBy;
			VPApprovedOn                      = customerOrder.VPApprovedOn;
			VPApprovedBy                      = customerOrder.VPApprovedBy;
			CustomerID                        = customerOrder.CustomerID;
			CustomerShippingInfoID            = customerOrder.CustomerShippingInfoID;
			PONumber                          = customerOrder.PONumber;
			AttachmentURI                     = customerOrder.AttachmentURI;
			Contact                           = customerOrder.Contact;
			ShipCompanyType                   = customerOrder.ShipCompanyType;
			ShipChoiceCodeID                  = customerOrder.ShipChoiceCodeID;
			IsPartialShipAcceptable           = customerOrder.IsPartialShipAcceptable;
			IsDoNotFill                       = customerOrder.IsDoNotFill;
			ShippingCharge                    = customerOrder.ShippingCharge;
			HandlingCharge                    = customerOrder.HandlingCharge;
			InsuranceCharge                   = customerOrder.InsuranceCharge;
			Instructions                      = customerOrder.Instructions;
			Notes                             = customerOrder.Notes;
			UpdatedBy                         = customerOrder.UpdatedBy;
			UpdatedOn                         = customerOrder.UpdatedOn;
			DoNotFillReason                   = customerOrder.DoNotFillReason;
			IntermediaryShippingName          = customerOrder.IntermediaryShippingName;
			IntermediaryShippingAddress       = customerOrder.IntermediaryShippingAddress;
			IntermediaryShippingContactNumber = customerOrder.IntermediaryShippingContactNumber;
			IntermediaryShippingContactName   = customerOrder.IntermediaryShippingContactName;
			IntermediaryShippingContactEmail  = customerOrder.IntermediaryShippingContactEmail;
			AttachmentFileName                = customerOrder.AttachmentFileName;
			ShippingCustomerName			  = customerOrder.ShippingCustomerName;
			FilledBy						  = customerOrder.FilledBy;
			FilledOn						  = customerOrder.FilledByOn;
			ShippedBy						  = customerOrder.ShippedBy;
			ShippedOn						  = customerOrder.ShippedByOn;
		}

		public int CustomerOrderID { get; set; }
		public string CustomerOrderCustomID { get; set; }
		public DateTime? SubmitDate { get; set; }
		public DateTime? MGApprovedOn { get; set; }
		public int? MGApprovedBy { get; set; }
		public DateTime? VPApprovedOn { get; set; }
		public int? VPApprovedBy { get; set; }
		public int CustomerID { get; set; }
		public int CustomerShippingInfoID { get; set; }
		public string PONumber { get; set; }
		public string AttachmentURI { get; set; }
		public string Contact { get; set; }
		public int? ShipCompanyType { get; set; }
		public int? ShipChoiceCodeID { get; set; }
		public bool IsPartialShipAcceptable { get; set; }
		public bool IsDoNotFill { get; set; }
		public decimal? ShippingCharge { get; set; }
		public decimal? HandlingCharge { get; set; }
		public decimal? InsuranceCharge { get; set; }
		public string Instructions { get; set; }
		public string Notes { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public string DoNotFillReason { get; set; }
		public string IntermediaryShippingName { get; set; }
		public string IntermediaryShippingAddress { get; set; }
		public string IntermediaryShippingContactNumber { get; set; }
		public string IntermediaryShippingContactName { get; set; }
		public string IntermediaryShippingContactEmail { get; set; }
		public string AttachmentFileName { get; set; }
		public string ShippingCustomerName { get; set; }
		public bool IsFilling { get; set; }
		public int FilledBy { get; set; }
		public DateTime? FilledOn { get; set; }
		public int? ShippedBy { get; set; }
		public DateTime? ShippedOn { get; set; }

		public CustomerOrder ToModel(CustomerOrder curModel)
		{
			if(curModel == null)
				curModel = new CustomerOrder() { CustomerOrderID = CustomerOrderID };

			curModel.CustomerOrderCustomID             = CustomerOrderCustomID;
			curModel.CustomerID                        = CustomerID;
			curModel.CustomerShippingInfoID            = CustomerShippingInfoID;
			curModel.PONumber                          = PONumber;
			curModel.AttachmentURI                     = AttachmentURI;
			curModel.Contact                           = Contact;
			curModel.ShipCompanyType                   = ShipCompanyType;
			curModel.ShipChoiceCodeID                  = ShipChoiceCodeID;
			curModel.IsPartialShipAcceptable           = IsPartialShipAcceptable;
			curModel.IsDoNotFill                       = IsDoNotFill;
			curModel.ShippingCharge                    = ShippingCharge;
			curModel.HandlingCharge                    = HandlingCharge;
			curModel.InsuranceCharge                   = InsuranceCharge;
			curModel.Instructions                      = Instructions;
			curModel.Notes                             = Notes;
			curModel.DoNotFillReason                   = DoNotFillReason;
			curModel.IntermediaryShippingName          = IntermediaryShippingName;
			curModel.IntermediaryShippingAddress       = IntermediaryShippingAddress;
			curModel.IntermediaryShippingContactNumber = IntermediaryShippingContactNumber;
			curModel.IntermediaryShippingContactName   = IntermediaryShippingContactName;
			curModel.IntermediaryShippingContactEmail  = IntermediaryShippingContactEmail;
			curModel.AttachmentFileName                = AttachmentFileName;
			curModel.ShippingCustomerName			   = ShippingCustomerName;
			curModel.IsFilling = IsFilling;
			curModel.FilledBy = FilledBy;
			curModel.FilledByOn = FilledOn;
			curModel.ShippedBy = ShippedBy;
			curModel.ShippedByOn = ShippedOn;

			return curModel;
		}
	}
}
