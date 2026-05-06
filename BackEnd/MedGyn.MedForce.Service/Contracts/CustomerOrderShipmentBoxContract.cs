using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CustomerOrderShipmentBoxContract
	{
		public CustomerOrderShipmentBoxContract(){}
		public CustomerOrderShipmentBoxContract(CustomerOrderShipmentBox box)
		{
			CustomerOrderShipmentBoxID = box.CustomerOrderShipmentBoxID;
			CustomerOrderShipmentID    = box.CustomerOrderShipmentID;
			CustomerOrderID            = box.CustomerOrderID;
			Weight                     = box.Weight;
			WeightUnitCodeID           = box.WeightUnitCodeID;
			Length                     = box.Length;
			Width                      = box.Width;
			Depth                      = box.Depth;
			DimensionUnitCodeID        = box.DimensionUnitCodeID;
		}

		public int CustomerOrderShipmentBoxID { get; set; }
		public int? CustomerOrderShipmentID { get; set; }
		public int CustomerOrderID { get; set; }
		public int? Weight { get; set; }
		public int? WeightUnitCodeID { get; set; }
		public int? Length { get; set; }
		public int? Width { get; set; }
		public int? Depth { get; set; }
		public int? DimensionUnitCodeID { get; set; }

		public bool HasShippingInfoFilled => Weight > 0
			&& Length > 0
			&& Width > 0
			&& Depth > 0
			&& WeightUnitCodeID.HasValue
			&& DimensionUnitCodeID.HasValue;

		public CustomerOrderShipmentBox ToModel()
		{
			return new CustomerOrderShipmentBox()
			{
				CustomerOrderShipmentBoxID = CustomerOrderShipmentBoxID,
				CustomerOrderShipmentID    = CustomerOrderShipmentID,
				CustomerOrderID            = CustomerOrderID,
				Weight                     = Weight,
				WeightUnitCodeID           = WeightUnitCodeID,
				Length                     = Length,
				Width                      = Width,
				Depth                      = Depth,
				DimensionUnitCodeID        = DimensionUnitCodeID,
			};
		}
	}
}