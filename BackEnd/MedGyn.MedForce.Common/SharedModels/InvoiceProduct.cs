namespace MedGyn.MedForce.Common.SharedModels
{
	public class InvoiceProduct
	{
		public InvoiceProduct() { }

		public InvoiceProduct(dynamic invoiceProduct)
		{
			ProductID                  = invoiceProduct.ProductID;
			ProductCustomID            = invoiceProduct.ProductCustomID;
			CustomerOrderShipmentBoxID = invoiceProduct.CustomerOrderShipmentBoxID;
			ProductName                = invoiceProduct.ProductName;
			Quantity                   = invoiceProduct.Quantity;
			QuantityPacked             = invoiceProduct.QuantityPacked;
			SerialNumbers              = invoiceProduct.SerialNumbers;
			Price                      = invoiceProduct.Price;

		}

		public int ProductID { get; set; }
		public string ProductCustomID { get; set; }
		public int CustomerOrderShipmentBoxID { get; set; }
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public int? BackorderQuantity { get; set; }
		public int QuantityPacked { get; set; }
		public string SerialNumbers { get; set; }
		public decimal Price { get; set; }
		public decimal Ext => QuantityPacked * Price;

	}
}
