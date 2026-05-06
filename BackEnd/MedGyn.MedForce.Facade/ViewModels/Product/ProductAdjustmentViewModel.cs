using System;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class ProductInventoryAdjustmentViewModel
	{
		public ProductInventoryAdjustmentViewModel() { }

		public ProductInventoryAdjustmentViewModel(ProductInventoryAdjustmentContract productAdjustment)
		{
			if (productAdjustment == null)
			{
				return;
			}

			ProductID       = productAdjustment.ProductID;
			Quantity        = productAdjustment.Quantity;
			ReasonCodeID    = productAdjustment.ReasonCodeID;
			ReasonCodeOther = productAdjustment.ReasonCodeOther;
			AdjustmentDate  = productAdjustment.AdjustmentDate;
		}

		public int ProductID { get; set; }
		public int Quantity { get; set; }
		public int ReasonCodeID { get; set; }
		public string ReasonCodeOther { get; set; }
		public string AdjustedBy { get; set; }
		public DateTime AdjustmentDate { get; set; }

		public ProductInventoryAdjustmentContract ToContract()
		{
			return new ProductInventoryAdjustmentContract
			{
				ProductID       = ProductID,
				Quantity        = Quantity,
				ReasonCodeID    = ReasonCodeID,
				ReasonCodeOther = ReasonCodeOther
			};
		}
	}
}
