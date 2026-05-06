using System;
using System.Collections.Generic;
using System.Linq;
using MedGyn.MedForce.Common.SharedModels;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class PurchaseOrderReceiveViewModel
	{
		public PurchaseOrderReceiveViewModel() { }
		public PurchaseOrderReceiveViewModel(string updater, PurchaseOrderReceiveInfo pori)
		{
			PurchaseOrderID         = pori.PurchaseOrderID;
			PurchaseOrderCustomID   = pori.PurchaseOrderCustomID;
			SubmitDate              = pori.SubmitDate;
			ApprovalDate            = pori.ApprovalDate;
			ApprovedBy              = pori.ApprovedBy;
			VendorID                = pori.VendorID;
			VendorOrderNumber       = pori.VendorOrderNumber;
			FromCity                = pori.FromCity;
			FromState               = pori.FromState;
			FromCountry             = pori.FromCountry;
			ExpectedDate            = pori.ExpectedDate;
			ShipCompany             = pori.ShipCompany.HasValue ? Enum.GetName(typeof(ShippingCompanyEnum), pori.ShipCompany) : null;
			ShipMethod              = pori.ShipMethod;
			IsPartialShipAcceptable = pori.IsPartialShipAcceptable;
			ShippingCharge          = pori.ShippingCharge;
			Notes                   = pori.Notes;
			UpdatedBy               = updater;
			UpdatedOn               = pori.UpdatedOn;
		}

		public virtual int PurchaseOrderID { get; set; }
		public virtual string PurchaseOrderCustomID { get; set; }
		public virtual DateTime? SubmitDate { get; set; }
		public virtual DateTime? ApprovalDate { get; set; }
		public virtual int? ApprovedBy { get; set; }
		public virtual int VendorID { get; set; }
		public virtual string VendorOrderNumber { get; set; }
		public virtual string FromCity { get; set; }
		public virtual string FromState { get; set; }
		public virtual string FromCountry { get; set; }
		public virtual DateTime? ExpectedDate { get; set; }
		public virtual string ShipCompany { get; set; }
		public virtual string ShipMethod { get; set; }
		public virtual bool IsPartialShipAcceptable { get; set; }
		public virtual decimal? ShippingCharge { get; set; }
		public virtual string Notes { get; set; }
		public virtual string UpdatedBy { get; set; }
		public virtual DateTime UpdatedOn { get; set; }

		public IList<PurchaseOrderProductReceiptViewModel> Products { get; set; }
	}
}
