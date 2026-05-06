using System;
using System.Collections.Generic;
using System.Diagnostics;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class PurchaseOrderViewModel
	{
		public PurchaseOrderViewModel()
		{
			Products = new List<PurchaseOrderProductViewModel>();
		}
		public PurchaseOrderViewModel(PurchaseOrderContract purchaseOrder) : this()
		{
			PurchaseOrderID         = purchaseOrder.PurchaseOrderID;
			PurchaseOrderCustomID   = purchaseOrder.PurchaseOrderCustomID;
			SubmitDate              = purchaseOrder.SubmitDate;
			ApprovalDate            = purchaseOrder.ApprovalDate;
			VendorID                = purchaseOrder.VendorID;
			VendorOrderNumber       = purchaseOrder.VendorOrderNumber;
			ExpectedDate            = purchaseOrder.ExpectedDate;
			ShipCompanyType         = purchaseOrder.ShipCompanyType;
			ShipChoiceCodeID        = purchaseOrder.ShipChoiceCodeID;
			IsPartialShipAcceptable = purchaseOrder.IsPartialShipAcceptable;
			ShippingCharge          = purchaseOrder.ShippingCharge;
			Notes                   = purchaseOrder.Notes;
			UpdatedOn               = purchaseOrder.UpdatedOn;
			IsDoNotReceive          = purchaseOrder.IsDoNotReceive;
			DoNotReceiveReason      = purchaseOrder.DoNotReceiveReason;
			ReceivedOn			    = purchaseOrder.ReceivedOn;
		}

		public int PurchaseOrderID { get; set; }
		public string PurchaseOrderCustomID { get; set; }
		public DateTime? SubmitDate { get; set; }
		public DateTime? ApprovalDate { get; set; }
		public int? VendorID { get; set; }
		public string VendorOrderNumber { get; set; }
		public DateTime? ExpectedDate { get; set; }
		public int? ShipCompanyType { get; set; }
		public int? ShipChoiceCodeID { get; set; }
		public bool IsPartialShipAcceptable { get; set; }
		public decimal? ShippingCharge { get; set; }
		public string Notes { get; set; }
		public string ApprovedBy { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsDoNotReceive { get; set; }
		public string DoNotReceiveReason { get; set; }
		public bool IsDomestic { get; set; }
		public bool IsDomesticDistribution { get; set; }
		public bool IsInternational { get; set; }
		public string ReceivedBy { get; set; }
		public DateTime? ReceivedOn { get; set; }


		public List<PurchaseOrderProductViewModel> Products { get; set; }

		public PurchaseOrderContract ToContract()
		{
			return new PurchaseOrderContract
			{
				PurchaseOrderID         = PurchaseOrderID,
				PurchaseOrderCustomID   = PurchaseOrderCustomID,
				SubmitDate              = SubmitDate.HasValue ? SubmitDate.Value.Date : SubmitDate,
				ApprovalDate            = ApprovalDate.HasValue ? ApprovalDate.Value.Date : ApprovalDate,
				VendorID                = VendorID.Value,
				VendorOrderNumber       = VendorOrderNumber,
				ExpectedDate            = ExpectedDate.HasValue ? ExpectedDate.Value.Date : ExpectedDate,
				ShipCompanyType         = ShipCompanyType,
				ShipChoiceCodeID        = ShipChoiceCodeID,
				IsPartialShipAcceptable = IsPartialShipAcceptable,
				ShippingCharge          = ShippingCharge,
				Notes                   = Notes,
				IsDoNotReceive          = IsDoNotReceive,
				DoNotReceiveReason      = DoNotReceiveReason,
			};
		}
	}
}
