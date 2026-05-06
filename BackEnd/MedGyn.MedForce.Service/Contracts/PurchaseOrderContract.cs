using System;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class PurchaseOrderContract
	{
		public PurchaseOrderContract() { }
		public PurchaseOrderContract(PurchaseOrder purchaseOrder)
		{
			if(purchaseOrder == null)
				return;

			PurchaseOrderID         = purchaseOrder.PurchaseOrderID;
			PurchaseOrderCustomID   = purchaseOrder.PurchaseOrderCustomID;
			SubmitDate              = purchaseOrder.SubmitDate;
			ApprovalDate            = purchaseOrder.ApprovalDate;
			ApprovedBy              = purchaseOrder.ApprovedBy;
			VendorID                = purchaseOrder.VendorID;
			VendorOrderNumber       = purchaseOrder.VendorOrderNumber;
			ExpectedDate            = purchaseOrder.ExpectedDate;
			ShipCompanyType         = purchaseOrder.ShipCompanyType;
			ShipChoiceCodeID        = purchaseOrder.ShipChoiceCodeID;
			IsPartialShipAcceptable = purchaseOrder.IsPartialShipAcceptable;
			ShippingCharge          = purchaseOrder.ShippingCharge;
			Notes                   = purchaseOrder.Notes;
			UpdatedBy               = purchaseOrder.UpdatedBy;
			UpdatedOn               = purchaseOrder.UpdatedOn;
			IsDoNotReceive          = purchaseOrder.IsDoNotReceive;
			DoNotReceiveReason      = purchaseOrder.DoNotReceiveReason;
			ReceivedBy              = purchaseOrder.ReceivedBy;
			ReceivedOn = purchaseOrder.ReceivedOn;
			Quantity = -1;
		}

		public PurchaseOrderContract(PurchaseOrder purchaseOrder, int quantity)
		{
			if (purchaseOrder == null)
				return;

			PurchaseOrderID = purchaseOrder.PurchaseOrderID;
			PurchaseOrderCustomID = purchaseOrder.PurchaseOrderCustomID;
			SubmitDate = purchaseOrder.SubmitDate;
			ApprovalDate = purchaseOrder.ApprovalDate;
			ApprovedBy = purchaseOrder.ApprovedBy;
			VendorID = purchaseOrder.VendorID;
			VendorOrderNumber = purchaseOrder.VendorOrderNumber;
			ExpectedDate = purchaseOrder.ExpectedDate;
			ShipCompanyType = purchaseOrder.ShipCompanyType;
			ShipChoiceCodeID = purchaseOrder.ShipChoiceCodeID;
			IsPartialShipAcceptable = purchaseOrder.IsPartialShipAcceptable;
			ShippingCharge = purchaseOrder.ShippingCharge;
			Notes = purchaseOrder.Notes;
			UpdatedBy = purchaseOrder.UpdatedBy;
			UpdatedOn = purchaseOrder.UpdatedOn;
			IsDoNotReceive = purchaseOrder.IsDoNotReceive;
			DoNotReceiveReason = purchaseOrder.DoNotReceiveReason;
			ReceivedBy = purchaseOrder.ReceivedBy;
			ReceivedOn = purchaseOrder.ReceivedOn;
			Quantity = quantity;
		}

		public int PurchaseOrderID { get; set; }
		public string PurchaseOrderCustomID { get; set; }
		public DateTime? SubmitDate { get; set; }
		public DateTime? ApprovalDate { get; set; }
		public int? ApprovedBy { get; set; }
		public int VendorID { get; set; }
		public string VendorOrderNumber { get; set; }
		public DateTime? ExpectedDate { get; set; }
		public int? ShipCompanyType { get; set; }
		public int? ShipChoiceCodeID { get; set; }
		public bool IsPartialShipAcceptable { get; set; }
		public decimal? ShippingCharge { get; set; }
		public string Notes { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsDoNotReceive { get; set;}
		public string DoNotReceiveReason { get; set; }
		public int? ReceivedBy { get; set; }
		public DateTime? ReceivedOn { get; set; }
		public int Quantity { get; set; }

		public PurchaseOrder ToModel(PurchaseOrder currPo)
		{
			if(currPo == null)
			{
				currPo = new PurchaseOrder();
			}
			currPo.PurchaseOrderID         = PurchaseOrderID;
			currPo.PurchaseOrderCustomID   = PurchaseOrderCustomID;
			currPo.SubmitDate              = SubmitDate;
			currPo.VendorID                = VendorID;
			currPo.VendorOrderNumber       = VendorOrderNumber;
			currPo.ExpectedDate            = ExpectedDate;
			currPo.ShipCompanyType         = ShipCompanyType;
			currPo.ShipChoiceCodeID        = ShipChoiceCodeID;
			currPo.IsPartialShipAcceptable = IsPartialShipAcceptable;
			currPo.ShippingCharge          = ShippingCharge;
			currPo.Notes                   = Notes;
			currPo.UpdatedBy               = UpdatedBy;
			currPo.UpdatedOn               = UpdatedOn;
			currPo.IsDoNotReceive          = IsDoNotReceive;
			currPo.DoNotReceiveReason      = DoNotReceiveReason;
			currPo.ReceivedBy              = ReceivedBy;
			currPo.ReceivedOn              = ReceivedOn;

			return currPo;
		}
	}
}
