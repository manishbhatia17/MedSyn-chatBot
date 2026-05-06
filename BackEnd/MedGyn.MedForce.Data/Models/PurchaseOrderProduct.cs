namespace MedGyn.MedForce.Data.Models
{
	public class PurchaseOrderProduct
	{
		public virtual int PurchaseOrderProductID { get; set; }
		public virtual int PurchaseOrderID { get; set; }
		public virtual int ProductID { get; set; }
		public virtual int UnitOfMeasureCodeID { get; set; }
		public virtual int Quantity { get; set; }
		public virtual decimal Price { get; set; }
	}
}
