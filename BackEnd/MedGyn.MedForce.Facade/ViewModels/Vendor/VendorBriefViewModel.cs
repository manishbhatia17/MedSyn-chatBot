using System.Collections.Generic;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class VendorBriefViewModel
	{
		public VendorBriefViewModel() { }
		public VendorBriefViewModel(VendorContract vendor, IDictionary<int, CodeContract> countryCodes, IDictionary<int, CodeContract> vendorCodes)
		{
			VendorID           = vendor.VendorID;
			VendorName         = vendor.VendorName;
			VendorCustomID     = vendor.VendorCustomID;
			City               = vendor.City;
			CountryCodeID      = countryCodes[vendor.CountryCodeID]?.CodeDescription;
			VendorStatusCodeID = vendorCodes[vendor.VendorStatusCodeID]?.CodeDescription;
		}
		public int VendorID { get; set; }
		public string VendorName { get; set; }
		public string VendorCustomID { get; set; }
		public string City { get; set; }
		public string CountryCodeID { get; set; }
		public string VendorStatusCodeID { get; set; }
	}
}