using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MedGyn.MedForce.Web.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class ProductController : BaseController 
	{
		private readonly ISecurityFacade _securityFacade;
		public ProductController(ISecurityFacade securityFacade)
		{
			_securityFacade = securityFacade;
		}

		public override IActionResult Index()
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ProductView,
				SecurityKeyEnum.ProductEdit,
				SecurityKeyEnum.ExportProductList,
				SecurityKeyEnum.InventoryAdjust,
				SecurityKeyEnum.PurchaseOrderEdit};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			return View();
		}

		[Route("PriceAdjustments")]
		public IActionResult PriceAdjustments()
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ProductView,
				SecurityKeyEnum.ProductEdit,
				SecurityKeyEnum.ExportProductList,
				SecurityKeyEnum.InventoryAdjust,
				SecurityKeyEnum.PurchaseOrderEdit};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			return View();
		}
	}
}