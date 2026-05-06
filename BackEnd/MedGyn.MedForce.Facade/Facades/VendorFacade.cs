using System.Collections.Generic;
using System.Linq;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Facade.Facades
{
	public class VendorFacade : IVendorFacade
	{
		private readonly IVendorService _vendorService;
		private readonly ICodeService _codeService;
		private readonly IProductService _productService;
		private readonly IUserService _userService;

		public VendorFacade(
			IVendorService vendorService,
			ICodeService codeService,
			IProductService productService,
			IUserService userService
		)
		{
			_vendorService  = vendorService;
			_codeService    = codeService;
			_productService = productService;
			_userService    = userService;
		}

		public VendorListViewModel GetVendorListViewModel(SearchCriteriaViewModel sc)
		{
			var vendors      = _vendorService.GetAllVendors(sc.Search, sc.SortColumn, sc.SortAsc);
			var countryCodes = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var statusCodes = _codeService.GetCodeLookupByType(CodeTypeEnum.VendorStatus);

			var vendorViewModels = vendors.Select(v => new VendorBriefViewModel(v, countryCodes, statusCodes)).ToList();

			return new VendorListViewModel(sc, vendorViewModels);
		}
		public VendorDetailsViewModel GetVendorDetails(int vendorID)
		{
			var ret = new VendorDetailsViewModel()
			{
				StateCodes                        = _codeService.GetCodesByType(CodeTypeEnum.States).ToDropdownList(),
				CountryCodes                      = _codeService.GetCodesByType(CodeTypeEnum.Countries).ToDropdownList(),
				GLPurchaseAccountNumberCodes      = _codeService.GetCodesByType(CodeTypeEnum.VendorPurchasesGL).ToDropdownList(),
				GLFreightChargeAccountNumberCodes = _codeService.GetCodesByType(CodeTypeEnum.VendorFreightChargesGL).ToDropdownList(),
				GLAccountsPayableNumberCodes      = _codeService.GetCodesByType(CodeTypeEnum.VendorAccountsPayableGL).ToDropdownList(),
				VendorStatusCodes                 = _codeService.GetCodesByType(CodeTypeEnum.VendorStatus).ToDropdownList()
			};

			if (vendorID == 0)
			{
				ret.Vendor = new VendorViewModel
				{
					CountryCodeID = ret.CountryCodes.FirstOrDefault(x => x.AltID == "USA")?.Value,
					VendorStatusCodeID = ret.VendorStatusCodes.FirstOrDefault(x => x.AltID == "Active")?.Value ?? 0
				};
			}
			else
			{
				var vendor   = _vendorService.GetVendor(vendorID);
				var products = _productService.GetProductsForVendor(vendorID);
				var user     = _userService.GetUser(vendor.UpdatedBy);

				ret.Vendor = new VendorViewModel(vendor)
				{
					Products  = products.Select(p => $"{p.ProductCustomID} - {p.ProductName} {p.Description}").ToList(),
					UpdatedBy = user.FullName,
				};
			}

			return ret;
		}

		public IEnumerable<DropdownDisplayViewModel> GetProductsForVendor(int vendorID)
		{
			var products = _productService.GetProductsForVendor(vendorID);
			return products.Select(x => new DropdownDisplayViewModel(x.ProductID, $"{x.ProductCustomID} {x.ProductName}", visible: !x.IsDiscontinued)).ToList();

		}

		public SaveResults SaveVendor(VendorViewModel vendor)
		{
			var isValidCustomID = _vendorService.ValidateVendorCustomID(vendor.VendorID, vendor.VendorCustomID);
			if(!isValidCustomID)
				return new SaveResults("DUP_ID");
			var vendorContract = vendor.ToContract();

			var savedVendor = _vendorService.SaveVendor(vendorContract);

			return new SaveResults();
		}
	}
}
