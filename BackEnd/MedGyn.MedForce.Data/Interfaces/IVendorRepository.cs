using MedGyn.MedForce.Data.Models;
using System.Collections.Generic;

namespace MedGyn.MedForce.Data.Interfaces
{
	public interface IVendorRepository
	{
		IEnumerable<Vendor> GetAllVendors();
		IEnumerable<Vendor> GetAllVendors(string search, string sortCol, bool sortAsc);
		Vendor GetVendor(int vendorID);
		void UpdateVendor(Vendor vendorModel);
		Vendor SaveVendor(Vendor vendorModel);
		bool ValidateVendorCustomID(int vendorID, string vendorCustomID);

	}
}
