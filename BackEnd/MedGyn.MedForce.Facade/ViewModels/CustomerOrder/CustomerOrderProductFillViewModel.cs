using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderProductFillViewModel
	{
		public CustomerOrderProductFillViewModel() { }
		public CustomerOrderProductFillViewModel(dynamic data)
		{
			CustomerOrderProductFillID = data.CustomerOrderProductFillID ?? 0;
			CustomerOrderProductID     = data.CustomerOrderProductID;
			ProductCustomID            = data.ProductCustomID;
			ProductName                = data.ProductName;
			UnitOfMeasure              = data.UnitOfMeasure;
			OrderQuantity              = data.OrderQuantity;
			QuantityToShip             = data.OrderQuantity - (data.QuantityAlreadyPacked ?? 0) + (data.QuantityPacked ?? 0);
			QuantityPacked             = data.QuantityPacked;
			SerialNumbers              = data.SerialNumbers;
		}

		public int CustomerOrderProductFillID { get; set; }
		public int CustomerOrderProductID { get; set; }
		public string ProductCustomID { get; set; }
		public string ProductName { get; set; }
		public string UnitOfMeasure { get; set; }
		public int OrderQuantity { get; set; }
		public int QuantityToShip { get; set; }
		public int? QuantityPacked { get; set; }
		public string SerialNumbers { get; set; }

		public CustomerOrderProductFillContract ToContract()
		{
			return new CustomerOrderProductFillContract()
			{
				CustomerOrderProductFillID = CustomerOrderProductFillID,
				CustomerOrderProductID     = CustomerOrderProductID,
				QuantityPacked             = QuantityPacked ?? 0,
				SerialNumbers              = SerialNumbers,
			};
		}
	}
}
