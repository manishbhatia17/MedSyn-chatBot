using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class PurchaseOrderProductViewModel
	{
		public PurchaseOrderProductViewModel() { }
		public PurchaseOrderProductViewModel(PurchaseOrderProductContract purchaseOrderProduct)
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
		public string ProductName { get; set; }
		public int ProductID { get; set; }
		public int UnitOfMeasureCodeID { get; set; }
		public string UnitOfMeasure { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public bool MarkedForDelete { get; set; }

		public PurchaseOrderProductContract ToContract(int purchaseOrderID)
		{
			return new PurchaseOrderProductContract
			{
				PurchaseOrderProductID = PurchaseOrderProductID,
				PurchaseOrderID        = purchaseOrderID,
				ProductID              = ProductID,
				UnitOfMeasureCodeID    = UnitOfMeasureCodeID,
				Quantity               = Quantity,
				Price                  = Price,
			};
		}
	}
}
