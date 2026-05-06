using System;

namespace MedGyn.MedForce.Data.Models
{
	public class DailyCustomerOrderCount
	{
		public virtual int CustomerID { get; set; }
		public virtual DateTime LastCreated { get; set; }
		public virtual int DailyCount { get; set; }
	}
}
