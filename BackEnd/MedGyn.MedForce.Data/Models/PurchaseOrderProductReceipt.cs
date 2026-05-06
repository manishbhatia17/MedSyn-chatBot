using System;

namespace MedGyn.MedForce.Data.Models
{
	public class PurchaseOrderProductReceipt
	{
		public virtual int PurchaseOrderProductReceiptID { get; set; }
		public virtual int PurchaseOrderProductID { get; set; }
		public virtual int QuantityReceived { get; set; }
		public virtual string SerialNumbers { get; set; }
		public virtual DateTime? ReceiptDate { get; set; }
	}
}
