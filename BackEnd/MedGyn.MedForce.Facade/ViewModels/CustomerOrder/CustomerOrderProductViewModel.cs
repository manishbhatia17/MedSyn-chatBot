using MedGyn.MedForce.Service.Contracts;
using System;
using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderProductViewModel
	{
		public CustomerOrderProductViewModel() { }
		public CustomerOrderProductViewModel(CustomerOrderProductContract customerOrderProduct)
		{
			ProductName            = customerOrderProduct.ProductName;
			ProductCustomID        = customerOrderProduct.ProductCustomID;
			CustomerOrderProductID = customerOrderProduct.CustomerOrderProductID;
			CustomerOrderID        = customerOrderProduct.CustomerOrderID;
			ProductID              = customerOrderProduct.ProductID;
			UnitOfMeasureCodeID    = customerOrderProduct.UnitOfMeasureCodeID;
			Quantity               = customerOrderProduct.Quantity;
			Price                  = customerOrderProduct.Price.ToString();
			ProductID = customerOrderProduct.ProductID;
			originalProductID = customerOrderProduct.ProductID;

		}

		public int CustomerOrderProductID { get; set; }
		public int CustomerOrderID { get; set; }
		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public string ProductCustomID { get; set; }
		public int UnitOfMeasureCodeID { get; set; }
		public string UnitOfMeasure { get; set; }
		public int Quantity { get; set; }
		public string Price { get; set; }
		public bool MarkedForDelete { get; set; }
		public string ShippedBy { get; set; }
		public string ShippedByDate { get; set; }
		public string FilledBy { get; set; }
		public string FilledByDate { get; set; }
		public List<string> POExpectedDates { get; set; } = new List<string>();

		//used for details view model to track changes and ensure we can revert duplicates
		public int originalProductID { get; set; }

		public CustomerOrderProductContract ToContract(int customerOrderID)
		{
			return new CustomerOrderProductContract
			{
				CustomerOrderProductID = CustomerOrderProductID,
				CustomerOrderID        = customerOrderID,
				ProductID              = ProductID,
				UnitOfMeasureCodeID    = UnitOfMeasureCodeID,
				Quantity               = Quantity,
				Price                  = Decimal.Parse(Price),
				
			};
		}
	}
}
