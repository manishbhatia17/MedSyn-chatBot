namespace MedGyn.MedForce.Data.Models
{
	public class CustomerOrderProduct
	{
		public virtual int CustomerOrderProductID { get; set; }
		public virtual int CustomerOrderID { get; set; }
		public virtual int ProductID { get; set; }
		public virtual int UnitOfMeasureCodeID { get; set; }
		public virtual int Quantity { get; set; }
		public virtual decimal Price { get; set; }
	}
}
