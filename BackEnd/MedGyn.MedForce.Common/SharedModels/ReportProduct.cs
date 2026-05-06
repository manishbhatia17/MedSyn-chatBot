using System;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class ReportProduct
	{
		public ReportProduct() { }

		public ReportProduct(dynamic reportProduct)
		{
			ProductID                  = reportProduct.ProductID;
			ProductCustomID            = reportProduct.ProductCustomID;
			ProductName                = reportProduct.ProductName;
			Quantity                   = reportProduct.Quantity;
			QuantityPacked             = reportProduct.QuantityPacked;
			Price                      = reportProduct.Price;

			string cents = reportProduct.Price4Decimal.ToString().Split('.')[1];

			//if there are more than 2 decimal places, we want to round the price to 2 decimal places when it ends in multiple 0s
			if (cents.Length > 2)
			{
				string extraCents = cents.Substring(2, (cents.Length -2));

				if(Convert.ToDouble(extraCents) == 0)
				{
					Price4Decimal = decimal.Parse(reportProduct.Price4Decimal.ToString("0.00"));
				}
                else
                {
					Price4Decimal = reportProduct.Price4Decimal;
				}
                
			}
			else
			{
				Price4Decimal = reportProduct.Price4Decimal;
			}

		}

		public int ProductID { get; set; }
		public string ProductCustomID { get; set; }
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public int QuantityPacked { get; set; }
		public decimal Price { get; set; } //price with 2 decimal places
		public decimal Price4Decimal { get; set; } //price with more than 2 decimal places
		public decimal Ext => decimal.Parse((Quantity * Price4Decimal).ToString("0.00")); // we want the price to be rounded to 2 decimal places

	}
}
