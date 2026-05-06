using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class PurchaseOrderProductReceiptContract
	{
		public PurchaseOrderProductReceiptContract() { }
		public PurchaseOrderProductReceiptContract(PurchaseOrderProductReceipt purchaseOrderLineReceipt)
		{
			PurchaseOrderProductReceiptID = purchaseOrderLineReceipt.PurchaseOrderProductReceiptID;
			PurchaseOrderProductID = purchaseOrderLineReceipt.PurchaseOrderProductID;
			QuantityReceived = purchaseOrderLineReceipt.QuantityReceived;
			SerialNumbers = purchaseOrderLineReceipt.SerialNumbers;
		}

		public int PurchaseOrderProductReceiptID { get; set; }
		public int PurchaseOrderProductID { get; set; }
		public int QuantityReceived { get; set; }
		public string SerialNumbers { get; set; }

		public PurchaseOrderProductReceipt ToModel()
		{
			return new PurchaseOrderProductReceipt
			{
				PurchaseOrderProductReceiptID = PurchaseOrderProductReceiptID,
				PurchaseOrderProductID = PurchaseOrderProductID,
				QuantityReceived = QuantityReceived,
				SerialNumbers = SerialNumbers,
			};
		}
	}
}
