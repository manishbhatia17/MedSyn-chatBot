using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderShipBoxViewModel
	{
		public CustomerOrderShipBoxViewModel() { }
		public CustomerOrderShipBoxViewModel(CustomerOrderShipmentBoxContract box)
		{
			CustomerOrderShipmentBoxID = box.CustomerOrderShipmentBoxID;
			Weight                     = box.Weight;
			WeightUnitCodeID           = box.WeightUnitCodeID;
			Length                     = box.Length;
			Width                      = box.Width;
			Depth                      = box.Depth;
			DimensionUnitCodeID        = box.DimensionUnitCodeID;
		}

		public int CustomerOrderShipmentBoxID { get; set; }
		public int? Weight { get; set; }
		public int? WeightUnitCodeID { get; set; }
		public int? Length { get; set; }
		public int? Width { get; set; }
		public int? Depth { get; set; }
		public int? DimensionUnitCodeID { get; set; }
		public bool IsFormPrefilled { get; set; } = false;

		public CustomerOrderShipmentBoxContract ToContract()
		{
			return new CustomerOrderShipmentBoxContract()
			{
				CustomerOrderShipmentBoxID = CustomerOrderShipmentBoxID,
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
