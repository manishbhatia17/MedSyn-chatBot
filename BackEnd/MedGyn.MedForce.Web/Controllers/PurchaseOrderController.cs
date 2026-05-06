using MedGyn.MedForce.Facade.Interfaces;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MedGyn.MedForce.Web.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class PurchaseOrderController : BaseController
	{
		private readonly IPurchaseOrderFacade _purchaseOrderFacade;
		private readonly ISecurityFacade _securityFacade;
		public PurchaseOrderController(IPurchaseOrderFacade purchaseOrderFacade, ISecurityFacade securityFacade)
		{
			_purchaseOrderFacade = purchaseOrderFacade;
			_securityFacade = securityFacade;
		}

		public override IActionResult Index()
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderView,
				SecurityKeyEnum.PurchaseOrderEdit,
				SecurityKeyEnum.PurchaseOrderDomesticView,
				SecurityKeyEnum.PurchaseOrderDomesticEdit,
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionView,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalView,
				SecurityKeyEnum.PurchaseOrderInternationalEdit,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,
				SecurityKeyEnum.PurchaseOrderReceive};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			return View();
		}

		[Route("Receive/{id}")]
		public virtual IActionResult Receive(int id)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PurchaseOrderReceive);
			if (!isAuthorized)
				return StatusCode(403);
			return View(id);
		}
	}
}