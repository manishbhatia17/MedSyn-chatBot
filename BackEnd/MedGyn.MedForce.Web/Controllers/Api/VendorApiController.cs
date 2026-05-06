using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Route("api/vendor")]
	[Authorize]
	public class VendorAPIController : BaseApiController
	{
		private readonly ISecurityFacade _securityFacade;
		private readonly IVendorFacade _vendorFacade;

		public VendorAPIController(ISecurityFacade securityFacade, IVendorFacade vendorFacade)
		{
			_securityFacade = securityFacade;
			_vendorFacade = vendorFacade;
		}

		[HttpPost, Route("")]
		public IActionResult FetchVendors([FromBody] SearchCriteriaViewModel searchCriteria)
		{
			var validKeys = new List<SecurityKeyEnum> { SecurityKeyEnum.VendorView, SecurityKeyEnum.VendorEdit };
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var vendorListModel = _vendorFacade.GetVendorListViewModel(searchCriteria);
			return Json(vendorListModel);
		}

		[HttpGet, Route("{vendorID}")]
		public IActionResult GetVendorDetails(int vendorID)
		{
			var validKeys = new List<SecurityKeyEnum> { SecurityKeyEnum.VendorView, SecurityKeyEnum.VendorEdit };
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var model = _vendorFacade.GetVendorDetails(vendorID);
			return Json(model);
		}

		[HttpGet, Route("{vendorID}/products")]
		public IActionResult GetVendorProducts(int vendorID)
		{
			var list = _vendorFacade.GetProductsForVendor(vendorID);
			return Json(list);
		}

		[HttpPost, Route("save")]
		public IActionResult SaveVendor([FromBody] VendorViewModel vendor)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.VendorEdit);
			if (!isAuthorized)
				return StatusCode(403);
			var results = _vendorFacade.SaveVendor(vendor);
			return Json(results);
		}
	}
}
