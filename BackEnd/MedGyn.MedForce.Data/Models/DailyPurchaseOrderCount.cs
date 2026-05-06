using System;

namespace MedGyn.MedForce.Data.Models
{
	public class DailyPurchaseOrderCount
	{
		public virtual int VendorID { get; set; }
		public virtual DateTime LastCreated { get; set; }
		public virtual int DailyCount { get; set; }
	}
}
