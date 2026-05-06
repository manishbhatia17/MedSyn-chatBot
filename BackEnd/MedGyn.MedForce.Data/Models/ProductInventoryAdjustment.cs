using System;

namespace MedGyn.MedForce.Data.Models
{
	public class ProductInventoryAdjustment
	{
		public virtual int ProductInventoryAdjustmentID { get; set; }
		public virtual int ProductID { get; set; }
		public virtual int Quantity { get; set; }
		public virtual int ReasonCodeID { get; set; }
		public virtual string ReasonCodeOther { get; set; }
		public virtual int AdjustedBy { get; set; }
		public virtual DateTime AdjustmentDate { get; set; }
	}
}