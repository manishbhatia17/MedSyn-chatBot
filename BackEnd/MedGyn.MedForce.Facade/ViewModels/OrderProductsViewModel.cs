using System;
using System.Collections.Generic;
using System.Linq;
using MedGyn.MedForce.Common.SharedModels;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class OrderProductsViewModel
	{
		public int ProductID { get; set; }
		public int? UMCodeID { get; set; }
		public string UnitOfMeasure { get; set; }
		public decimal? PriceDomesticList { get; set; }
		public decimal? PriceDomesticDistribution { get; set; }
		public decimal? PriceDomesticAfaxys { get; set; }
		public decimal? PriceInternationalDistribution { get; set; }
		public decimal? ListPrice
		{
			get
			{
				return POHistory?.FirstOrDefault()?.Price;
			}
		}

		public StockInfo StockInfo { get; set; }
		public PurchaseOrderProductsHistoryViewModel LowestPrice { get; set; }

		public List<PurchaseOrderProductsHistoryViewModel> POHistory { get; set; }
		public List<CustomerOrderProductsViewModelHistory> COHistory { get; set; }
		public List<string> POExpectedDates { get; set; } = new List<string>();
	}

	public class CustomerOrderProductsViewModelHistory
	{
		public CustomerOrderProductsViewModelHistory() { }
		public CustomerOrderProductsViewModelHistory(dynamic data)
		{
			if(data == null)
				return;

			Date     = ((DateTime?)data.SubmitDate)?.ToString("MM/dd/yy") ?? null;
			Quantity = data.Quantity;
			UMCodeID = data.UnitOfMeasureCodeID;
			Price    = data.Price;
		}
		public string Date { get; set; }
		public int Quantity { get; set; }
		public int? UMCodeID { get; set; }
		public string UnitOfMeasure { get; set; }
		public decimal Price { get; set; }
		public decimal ExtPrice => Quantity * Price;
	}

	public class PurchaseOrderProductsHistoryViewModel
	{
		public PurchaseOrderProductsHistoryViewModel() { }
		public PurchaseOrderProductsHistoryViewModel(dynamic data)
		{
			if(data == null)
				return;

			Date       = ((DateTime?)data.SubmitDate)?.ToString("MM/dd/yy") ?? null;
			Quantity   = data.Quantity;
			UMCodeID   = data.UnitOfMeasureCodeID;
			Price      = data.Price;
			VendorID   = data.VendorID;
			VendorName = data.VendorName;
		}
		public string Date { get; set; }
		public int Quantity { get; set; }
		public int? UMCodeID { get; set; }
		public string UnitOfMeasure { get; set; }
		public decimal Price { get; set; }
		public int VendorID { get; set; }
		public string VendorName { get; set; }
		public decimal ExtPrice => Quantity * Price;
	}
}


