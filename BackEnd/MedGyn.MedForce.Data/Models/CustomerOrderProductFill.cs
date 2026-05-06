using System;

namespace MedGyn.MedForce.Data.Models
{
	public class CustomerOrderProductFill
	{
		public virtual int CustomerOrderProductFillID { get; set; }
		public virtual int CustomerOrderProductID { get; set; }
		public virtual int QuantityPacked { get; set; }
		public virtual string SerialNumbers { get; set; }
		public virtual int? CustomerOrderShipmentBoxID { get; set; }
	}
}
