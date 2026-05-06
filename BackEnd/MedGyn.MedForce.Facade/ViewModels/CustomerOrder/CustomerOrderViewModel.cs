using System;
using System.Collections.Generic;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderViewModel
	{
		public CustomerOrderViewModel()
		{
			Products = new List<CustomerOrderProductViewModel>();
		}
		public CustomerOrderViewModel(CustomerOrderContract customerOrder) : this()
		{
			CustomerOrderID                   = customerOrder.CustomerOrderID;
			CustomerOrderCustomID             = customerOrder.CustomerOrderCustomID;
			SubmitDate                        = customerOrder.SubmitDate;
			MGApprovedOn                      = customerOrder.MGApprovedOn;
			VPApprovedOn                      = customerOrder.VPApprovedOn;
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
			UpdatedOn                         = customerOrder.UpdatedOn;
			DoNotFillReason                   = customerOrder.DoNotFillReason;
			IntermediaryShippingName          = customerOrder.IntermediaryShippingName;
			IntermediaryShippingAddress       = customerOrder.IntermediaryShippingAddress;
			IntermediaryShippingContactNumber = customerOrder.IntermediaryShippingContactNumber;
			IntermediaryShippingContactName   = customerOrder.IntermediaryShippingContactName;
			IntermediaryShippingContactEmail  = customerOrder.IntermediaryShippingContactEmail;
			AttachmentFileName                = customerOrder.AttachmentFileName;
			ShippingCustomerName			  = customerOrder.ShippingCustomerName;
			FilledOn                          = customerOrder.FilledOn;
			ShippedOn                         = customerOrder.ShippedOn;
		}

		public int CustomerOrderID { get; set; }
		public string CustomerOrderCustomID { get; set; }
		public DateTime? SubmitDate { get; set; }
		public DateTime? MGApprovedOn { get; set; }
		public string MGApprovedBy { get; set; }
		public DateTime? VPApprovedOn { get; set; }
		public string VPApprovedBy { get; set; }
		public int? CustomerID { get; set; }
		public int? CustomerShippingInfoID { get; set; }
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
		public string UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public string DoNotFillReason { get; set; }
		public bool IsDomestic { get; set; }
		public bool IsDomesticDistribution { get; set; }
		public bool IsInternational { get; set; }
		public string IntermediaryShippingName { get; set; }
		public string IntermediaryShippingAddress { get; set; }
		public string IntermediaryShippingContactNumber { get; set; }
		public string IntermediaryShippingContactName { get; set; }
		public string IntermediaryShippingContactEmail { get; set; }
		public string AttachmentFileName { get; set; }
        public string ShippingCustomerName { get; set; }
		public string FilledBy { get; set; }
		public DateTime? FilledOn { get; set; }
		public string ShippedBy { get; set; }
		public DateTime? ShippedOn { get; set; }

        public List<CustomerOrderProductViewModel> Products { get; set; }

		public CustomerOrderContract ToContract()
		{
			return new CustomerOrderContract
			{
				CustomerOrderID                   = CustomerOrderID,
				CustomerOrderCustomID             = CustomerOrderCustomID,
				CustomerID                        = CustomerID.Value,
				CustomerShippingInfoID            = CustomerShippingInfoID.Value,
				PONumber                          = PONumber,
				AttachmentURI                     = AttachmentURI,
				Contact                           = Contact,
				ShipCompanyType                   = ShipCompanyType,
				ShipChoiceCodeID                  = ShipChoiceCodeID,
				IsPartialShipAcceptable           = IsPartialShipAcceptable,
				IsDoNotFill                       = IsDoNotFill,
				ShippingCharge                    = ShippingCharge,
				HandlingCharge                    = HandlingCharge,
				InsuranceCharge                   = InsuranceCharge,
				Instructions                      = Instructions,
				Notes                             = Notes,
				DoNotFillReason                   = DoNotFillReason,
				IntermediaryShippingName          = IntermediaryShippingName,
				IntermediaryShippingAddress       = IntermediaryShippingAddress,
				IntermediaryShippingContactNumber = IntermediaryShippingContactNumber,
				IntermediaryShippingContactName   = IntermediaryShippingContactName,
				IntermediaryShippingContactEmail  = IntermediaryShippingContactEmail,
				AttachmentFileName                = AttachmentFileName,
				ShippingCustomerName			  = ShippingCustomerName
			};
		}
	}
}
