using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class PurchaseOrderProductContract
	{
		public PurchaseOrderProductContract() { }
		public PurchaseOrderProductContract(dynamic purchaseOrderProduct)
		{
			PurchaseOrderProductID = purchaseOrderProduct.PurchaseOrderProductID;
			PurchaseOrderID        = purchaseOrderProduct.PurchaseOrderID;
			ProductID              = purchaseOrderProduct.ProductID;
			ProductName            = purchaseOrderProduct.ProductName;
			UnitOfMeasureCodeID    = purchaseOrderProduct.UnitOfMeasureCodeID;
			Quantity               = purchaseOrderProduct.Quantity;
			Price                  = purchaseOrderProduct.Price;
		}

		public int PurchaseOrderProductID { get; set; }
		public int PurchaseOrderID { get; set; }
		public int ProductID { get; set; }
		public int UnitOfMeasureCodeID { get; set; }
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }

		public PurchaseOrderProduct ToModel()
		{
			return new PurchaseOrderProduct
			{
				PurchaseOrderProductID = PurchaseOrderProductID,
				PurchaseOrderID        = PurchaseOrderID,
				ProductID              = ProductID,
				UnitOfMeasureCodeID    = UnitOfMeasureCodeID,
				Quantity               = Quantity,
				Price                  = Price,
			};
		}
	}
}
