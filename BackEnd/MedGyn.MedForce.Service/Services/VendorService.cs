using System;
using System.Collections.Generic;
using System.Linq;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
	public class VendorService : IVendorService
	{
		private readonly IVendorRepository _vendorRepository;
		private readonly IAuthenticationService _authenticationService;

		public VendorService(IVendorRepository vendorRepository, IAuthenticationService authenticationService)
		{
			_vendorRepository      = vendorRepository;
			_authenticationService = authenticationService;
		}

		public IEnumerable<VendorContract> GetAllVendors() {
			return _vendorRepository.GetAllVendors().Select(v => new VendorContract(v));
		}
		public IEnumerable<VendorContract> GetAllVendors(string search, string sortCol, bool sortAsc)
		{
			return _vendorRepository.GetAllVendors(search, sortCol, sortAsc).Select(v => new VendorContract(v));
		}

		public VendorContract GetVendor(int vendorID)
		{
			var model = _vendorRepository.GetVendor(vendorID);

			var contract = new VendorContract(model);

			return contract;
		}

		public VendorContract SaveVendor(VendorContract vendor)
		{
			var vendorModel = vendor.ToModel();

			vendorModel.UpdatedBy = _authenticationService.GetUserID();
			vendorModel.UpdatedOn = DateTime.UtcNow;

			if (vendorModel.VendorID > 0)
			{
				_vendorRepository.UpdateVendor(vendorModel);
			}
			else
			{
				vendorModel = _vendorRepository.SaveVendor(vendorModel);
			}

			return new VendorContract(vendorModel);
		}

		public bool ValidateVendorCustomID(int vendorID, string vendorCustomID)
		{
			return _vendorRepository.ValidateVendorCustomID(vendorID, vendorCustomID);
		}
	}
}
