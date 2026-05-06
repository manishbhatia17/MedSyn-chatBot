using System.Collections.Generic;
using MedGyn.MedForce.Common.Configurations;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderDetailsViewModel
	{
		public CustomerOrderDetailsViewModel() { }
		public CustomerOrderDetailsViewModel(AppSettings appSettings)
		{
		}

		public CustomerOrderViewModel CustomerOrder { get; set; }
		public List<DropdownDisplayViewModel> Customers { get; set; }
		public List<DropdownDisplayViewModel> Products { get; set; }
		public List<DropdownDisplayViewModel> ShipCompanyCodes { get; set; }
		public List<DropdownDisplayViewModel> FedExShipMethodCodes { get; set; }
		public List<DropdownDisplayViewModel> UPSShipMethodCodes { get; set; }
		public List<DropdownDisplayViewModel> OtherShipMethodCodes { get; set; }

		// Dotnet 3 isn't intelligent enough to serialize a Dict<int, string>
		public Dictionary<string, string> UMCodes { get; set; }

		//Dictonary stores all tax rates for customers so it can be applied when dropdown is changed
		public Dictionary<string, decimal> CustomerTaxes { get; set; }
		//Dictonary stores all credit card fees for customers so it can be applied when dropdown is changed
		public Dictionary<string, decimal> CreditCardFee { get; set; }
		
	}
}
