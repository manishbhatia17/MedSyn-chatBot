using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class VendorDetailsViewModel
	{
		public VendorViewModel Vendor { get; set; }
		public List<DropdownDisplayViewModel> StateCodes { get; set; }
		public List<DropdownDisplayViewModel> CountryCodes { get; set; }
		public List<DropdownDisplayViewModel> GLPurchaseAccountNumberCodes { get; set; }
		public List<DropdownDisplayViewModel> GLFreightChargeAccountNumberCodes { get; set; }
		public List<DropdownDisplayViewModel> GLAccountsPayableNumberCodes { get; set; }
		public List<DropdownDisplayViewModel> VendorStatusCodes { get; set; }
	}
}
