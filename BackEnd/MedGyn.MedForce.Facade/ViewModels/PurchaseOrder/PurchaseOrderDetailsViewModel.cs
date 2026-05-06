using System.Collections.Generic;
using MedGyn.MedForce.Common.Configurations;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class PurchaseOrderDetailsViewModel
	{
		public PurchaseOrderDetailsViewModel() { }
		public PurchaseOrderDetailsViewModel(AppSettings appSettings)
		{
			ShipCompanies = new List<DropdownDisplayViewModel>()
			{
				new DropdownDisplayViewModel((int)ShippingCompanyEnum.FedEx, "FedEx"),
				new DropdownDisplayViewModel((int)ShippingCompanyEnum.UPS, "UPS"),
			};

			ShippingAccounts = new Dictionary<string, string>() {
				{ $"{(int)ShippingCompanyEnum.FedEx}", appSettings.FedExAccountNumber },
				{ $"{(int)ShippingCompanyEnum.UPS}", appSettings.UPSAccountNumber },
			};
		}

		public PurchaseOrderViewModel PurchaseOrder { get; set; }
		public List<DropdownDisplayViewModel> Vendors { get; set; }
		public List<DropdownDisplayViewModel> ShipCompanies { get; set; }
		public List<DropdownDisplayViewModel> FedExShipMethodCodes { get; set; }
		public List<DropdownDisplayViewModel> UPSShipMethodCodes { get; set; }

		// Dotnet 3 isn't intelligent enough to serialize a Dict<int, string>
		public Dictionary<string, string> UMCodes { get; set; }
		public Dictionary<string, string> ShippingAccounts { get; set; }
		public Dictionary<string, decimal> VendorTaxes { get; set; }

	}
}
