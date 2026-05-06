using MedGyn.MedForce.Service.Contracts;
using System.Collections.Generic;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IVendorService
	{
		IEnumerable<VendorContract> GetAllVendors();
		IEnumerable<VendorContract> GetAllVendors(string search, string sortCol, bool sortAsc);
		VendorContract GetVendor(int vendorId);
		VendorContract SaveVendor(VendorContract vendorContract);
		bool ValidateVendorCustomID(int vendorID, string vendorCustomID);
	}
}
