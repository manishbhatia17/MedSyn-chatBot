using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class PurchaseOrderProductReceiptViewModel
	{
		public PurchaseOrderProductReceiptViewModel() { }
		public PurchaseOrderProductReceiptViewModel(PurchaseOrderProductReceiptContract purchaseOrderLineReceipt)
		{
			PurchaseOrderProductReceiptID = purchaseOrderLineReceipt.PurchaseOrderProductReceiptID;
			PurchaseOrderProductID        = purchaseOrderLineReceipt.PurchaseOrderProductID;
			QuantityReceived              = purchaseOrderLineReceipt.QuantityReceived;
			SerialNumbers                 = purchaseOrderLineReceipt.SerialNumbers;
		}

		public PurchaseOrderProductReceiptViewModel(dynamic data)
		{
			PurchaseOrderProductReceiptID = data.PurchaseOrderProductReceiptID ?? -1;
			PurchaseOrderProductID        = data.PurchaseOrderProductID;
			ProductCustomID               = data.ProductCustomID;
			ProductName                   = data.ProductName;
			UnitOfMeasure                 = data.UnitOfMeasure;
			OrderQuantity                 = data.OrderQuantity;
			QuantityToReceive             = OrderQuantity - (data.QuantityAlreadyReceived ?? 0);
		}

		public int PurchaseOrderProductReceiptID { get; set; }
		public int PurchaseOrderProductID { get; set; }
		public string ProductCustomID { get; set; }
		public string ProductName { get; set; }
		public string UnitOfMeasure { get; set; }
		public int OrderQuantity { get; set; }
		public int QuantityToReceive { get; set; }
		public int QuantityReceived { get; set; }
		public string SerialNumbers { get; set; }

		public PurchaseOrderProductReceiptContract ToContract()
		{
			return new PurchaseOrderProductReceiptContract
			{
				PurchaseOrderProductReceiptID = PurchaseOrderProductReceiptID,
				PurchaseOrderProductID        = PurchaseOrderProductID,
				QuantityReceived              = QuantityReceived,
				SerialNumbers                 = SerialNumbers,
			};
		}
	}
}
