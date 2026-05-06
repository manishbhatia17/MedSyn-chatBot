using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class ProductDetailsViewModel
	{
		public ProductViewModel Product { get; set; }
		public List<DropdownDisplayViewModel> Vendors { get; set; }
		public List<DropdownDisplayViewModel> UMCodes { get; set; }
		public List<DropdownDisplayViewModel> ShipWeightUnitCodes { get; set; }
		public List<DropdownDisplayViewModel> ShipDimensionUnitCodes { get; set; }
	}
}