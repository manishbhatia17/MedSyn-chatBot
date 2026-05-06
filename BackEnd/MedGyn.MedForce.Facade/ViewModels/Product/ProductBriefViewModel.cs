using System.Collections.Generic;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class ProductBriefViewModel
	{
		public ProductBriefViewModel() {}
		public ProductBriefViewModel(
			ProductContract product,
			IDictionary<int, CodeContract> umCodes,
			IDictionary<int, string> vendors,
			IDictionary<int, StockInfo> stockInfo
		)
		{
			ProductID       = product.ProductID;
			ProductName     = product.ProductName;
			ProductCustomID = product.ProductCustomID;
			ReorderPoint    = product.ReorderPoint;
			ReorderQuantity = product.ReorderQuantity;
			Cost            = product.Cost;
			IsDiscontinued  = product.IsDiscontinued;

			if (product.UnitOfMeasureCodeID.HasValue)
				UnitOfMeasureCodeID = umCodes[product.UnitOfMeasureCodeID.Value]?.CodeDescription;

			if (product.PrimaryVendorID.HasValue)
			{
				PrimaryVendorID = vendors[product.PrimaryVendorID.Value];
				PriVendorID     = product.PrimaryVendorID.Value;
			}

			if (stockInfo.TryGetValue(product.ProductID, out var si))
			{
				NetQuantity = si.NetQuantity;
				OnHandQuantity = si.OnHand;
				CommittedQuantity = si.UnfilledCOs;
				POQuantity = si.PendingPOs;
			}
		}

		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public string ProductCustomID { get; set; }
		public string UnitOfMeasureCodeID { get; set; }
		public int? OnHandQuantity { get; set; }
		public int? CommittedQuantity { get; set; }
		public int? POQuantity { get; set; } = 0;
		public int? NetQuantity { get; set; }
		public int? ReorderPoint { get; set; }
		public int? ReorderQuantity { get; set; }
		public string PrimaryVendorID { get; set; } // This is for display
		public int PriVendorID { get; set; }
		public decimal? Cost { get; set; }
		public bool IsDiscontinued { get; set; }
	}
}