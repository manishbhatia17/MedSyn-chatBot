namespace MedGyn.MedForce.Data.Models
{
	public class CustomerOrderShipmentBox
	{
		public virtual int CustomerOrderShipmentBoxID { get; set; }
		public virtual int? CustomerOrderShipmentID { get; set; }
		public virtual int CustomerOrderID { get; set; }
		public virtual int? Weight { get; set; }
		public virtual int? WeightUnitCodeID { get; set; }
		public virtual int? Length { get; set; }
		public virtual int? Width { get; set; }
		public virtual int? Depth { get; set; }
		public virtual int? DimensionUnitCodeID { get; set; }

	}
}