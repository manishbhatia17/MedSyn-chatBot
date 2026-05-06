using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CustomerOrderProductContract
	{
		public CustomerOrderProductContract() { }
		public CustomerOrderProductContract(dynamic customerOrderProduct) {
			CustomerOrderProductID = customerOrderProduct.CustomerOrderProductID;
			CustomerOrderID        = customerOrderProduct.CustomerOrderID;
			ProductID              = customerOrderProduct.ProductID;
			ProductName            = customerOrderProduct.ProductName;
			ProductCustomID        = customerOrderProduct.ProductCustomID;
			UnitOfMeasureCodeID    = customerOrderProduct.UnitOfMeasureCodeID;
			Quantity               = customerOrderProduct.Quantity;
			Price                  = customerOrderProduct.Price;
		}

		public int CustomerOrderProductID { get; set; }
		public int CustomerOrderID { get; set; }
		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public string ProductCustomID { get; set; }
		public int UnitOfMeasureCodeID { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }

		public CustomerOrderProduct ToModel()
		{
			var customerOrderProductID = CustomerOrderProductID < 0 ? 0 : CustomerOrderProductID;
			return new CustomerOrderProduct()
			{
				CustomerOrderProductID = customerOrderProductID,
				CustomerOrderID        = CustomerOrderID,
				ProductID              = ProductID,
				UnitOfMeasureCodeID    = UnitOfMeasureCodeID,
				Quantity               = Quantity,
				Price                  = Price,
			};
		}
	}
}
