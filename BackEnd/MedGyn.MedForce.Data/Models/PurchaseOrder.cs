using System;

namespace MedGyn.MedForce.Data.Models
{
	public class PurchaseOrder
	{
		public virtual int PurchaseOrderID { get; set; }
		public virtual string PurchaseOrderCustomID { get; set; }
		public virtual DateTime? SubmitDate { get; set; }
		public virtual DateTime? ApprovalDate { get; set; }
		public virtual int? ApprovedBy { get; set; }
		public virtual int VendorID { get; set; }
		public virtual string VendorOrderNumber { get; set; }
		public virtual DateTime? ExpectedDate { get; set; }
		public virtual int? ShipCompanyType { get; set; }
		public virtual int? ShipChoiceCodeID { get; set; }
		public virtual bool IsPartialShipAcceptable { get; set; }
		public virtual decimal? ShippingCharge { get; set; }
		public virtual string Notes { get; set; }
		public virtual int UpdatedBy { get; set; }
		public virtual DateTime UpdatedOn { get; set; }
		public virtual bool IsDoNotReceive { get; set; }
		public virtual string DoNotReceiveReason { get; set; }
		public virtual int CreatedBy { get; set; }
		public virtual DateTime CreatedOn { get; set; }
		public virtual int? ReceivedBy { get; set; }
		public virtual DateTime? ReceivedOn { get; set; }
	}
}
