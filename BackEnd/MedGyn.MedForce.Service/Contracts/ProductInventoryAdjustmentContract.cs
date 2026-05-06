using System;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class ProductInventoryAdjustmentContract
	{
		public ProductInventoryAdjustmentContract() { }

		public ProductInventoryAdjustmentContract(ProductInventoryAdjustment ProductInventoryAdjustment)
		{
			ProductInventoryAdjustmentID = ProductInventoryAdjustment.ProductInventoryAdjustmentID;
			ProductID                    = ProductInventoryAdjustment.ProductID;
			Quantity                     = ProductInventoryAdjustment.Quantity;
			ReasonCodeID                 = ProductInventoryAdjustment.ReasonCodeID;
			ReasonCodeOther              = ProductInventoryAdjustment.ReasonCodeOther;
			AdjustedBy                   = ProductInventoryAdjustment.AdjustedBy;
			AdjustmentDate               = ProductInventoryAdjustment.AdjustmentDate;
		}

		public int ProductInventoryAdjustmentID { get; set; }
		public int ProductID { get; set; }
		public int Quantity { get; set; }
		public int ReasonCodeID { get; set; }
		public string ReasonCodeOther { get; set; }
		public int AdjustedBy { get; set; }
		public DateTime AdjustmentDate { get; set; }

		public ProductInventoryAdjustment ToModel()
		{
			return new ProductInventoryAdjustment()
			{
				ProductInventoryAdjustmentID = ProductInventoryAdjustmentID,
				ProductID                    = ProductID,
				Quantity                     = Quantity,
				ReasonCodeID                 = ReasonCodeID,
				ReasonCodeOther              = ReasonCodeOther,
				AdjustedBy                   = AdjustedBy,
				AdjustmentDate               = AdjustmentDate,
			};
		}
	}
}
