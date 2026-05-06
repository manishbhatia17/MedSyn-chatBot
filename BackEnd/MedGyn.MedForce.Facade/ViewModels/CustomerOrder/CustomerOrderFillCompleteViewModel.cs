using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderProductFillCompleteViewModel
	{
		public CustomerOrderProductFillCompleteViewModel() { }


		public int CustomerOrderProductFillID { get; set; }
		public int FillOption { get; set; }
		public int NumberOfSameBoxes { get; set; }
		public int NumberOfPackingSlips { get; set; }
		public List<CustomerOrderProductFillViewModel> Products { get; set; }

		public bool GenerateMultiplePackingSlip { get; set; }

	}
}
