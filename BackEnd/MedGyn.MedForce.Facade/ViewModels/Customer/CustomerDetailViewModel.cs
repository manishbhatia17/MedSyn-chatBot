using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerDetailsViewModel
	{
		public CustomerViewModel Customer { get; set; }
		public List<DropdownDisplayViewModel> StateCodes { get; set; }
		public List<DropdownDisplayViewModel> CountryCodes { get; set; }
		public List<DropdownDisplayViewModel> GLSalesNumberCodes { get; set; }
		public List<DropdownDisplayViewModel> GLShippingChargeNumberCodes { get; set; }
		public List<DropdownDisplayViewModel> GLReceivableNumberCodes { get; set; }
		public List<DropdownDisplayViewModel> PaymentTypes { get; set; }
		public List<DropdownDisplayViewModel> PracticeTypeCodes { get; set; }
		public List<DropdownDisplayViewModel> SalesTaxCodes { get; set; }
		public List<DropdownDisplayViewModel> CustomerStatusCodes { get; set; }
		public List<DropdownDisplayViewModel> RepUserIDs { get; set; }
		public List<DropdownDisplayViewModel> ShipCompanyCodes { get; set; }

	}
}
