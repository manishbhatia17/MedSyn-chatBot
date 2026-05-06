using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class ProductAdjustmentDetailViewModel
	{

		public string ProductName { get; set; }
		public string ProductCustomID { get; set; }
		public int OnHand { get; set; }
		public List<DropdownDisplayViewModel> ReasonCodes { get; set; }
		public List<ProductInventoryAdjustmentViewModel> HistoryList { get; set; }
	}
}
