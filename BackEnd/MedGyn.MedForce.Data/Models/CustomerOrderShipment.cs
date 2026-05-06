using System;

namespace MedGyn.MedForce.Data.Models
{
	public class CustomerOrderShipment
	{
		public virtual int CustomerOrderShipmentID { get; set; }
		public virtual int CustomerOrderID { get; set; }
		public virtual int? ShipCompanyType { get; set; }
		public virtual int? ShipMethodCodeID { get; set; }
		public virtual string ShipAccountNumber { get; set; }
		public virtual bool ShipmentComplete { get; set; }
		public virtual int? FillOption { get; set; }
		public virtual int? NumberOfSameBoxes { get; set; }
		public virtual int? NumberOfPackingSlips { get; set; }
		public virtual decimal? ShippingCharge { get; set; }
		public virtual int CreatedBy { get; set; }
		public virtual DateTime CreatedOn { get; set; }
		public virtual string InvoiceNumber { get; set; }
		public virtual DateTime? InvoiceDate { get; set; }
		public virtual decimal? InvoiceTotal { get; set; }
		public virtual string MasterTrackingNumber { get; set; }
		public virtual bool InvoiceSent { get; set; }
		public virtual int? PeachTreeExportBatchID { get; set; }
		public virtual DateTime? DeliveryDate { get; set; }
	}
}