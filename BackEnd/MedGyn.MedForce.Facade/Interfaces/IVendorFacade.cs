using System.Collections.Generic;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.ViewModels;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface IVendorFacade
	{
		VendorListViewModel GetVendorListViewModel(SearchCriteriaViewModel sc);
		VendorDetailsViewModel GetVendorDetails(int vendorID);
		SaveResults SaveVendor(VendorViewModel vendor);
		IEnumerable<DropdownDisplayViewModel> GetProductsForVendor(int vendorID);
	}
}
