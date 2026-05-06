using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderShipViewModel
	{
		public CustomerOrderShipViewModel() { }

		public bool OtherBoxesDone { get; set; }
		public CustomerOrderFillViewModel Fill { get; set; }
		public CustomerOrderShipBoxViewModel ShipmentBox { get; set; }
		public CustomerOrderShipmentViewModel Shipment { get; set; }
		public List<DropdownDisplayViewModel> WeightUnitCodes { get; set; }
		public List<DropdownDisplayViewModel> DimensionUnitCodes { get; set; }
		public List<DropdownDisplayViewModel> ShipCompanyCodes { get; set; }
		public Dictionary<string, string> AccountNumbers { get; set; }
		public List<DropdownDisplayViewModel> FedExShipMethodCodes { get; set; }
		public List<DropdownDisplayViewModel> UPSShipMethodCodes { get; set; }
		public List<DropdownDisplayViewModel> OtherShipMethodCodes { get; set; }
		public int FedExCodeID { get; set; }
		public int FedExMedGynCodeID { get; set; }
		public int UPSCodeID { get; set; }
		public int UPSMedGynCodeID { get; set; }
		public int UPSFreightCodeID { get; set; }
		public int BillCountry { get; set; }
	}
}
