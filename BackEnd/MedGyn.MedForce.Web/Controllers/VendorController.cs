using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MedGyn.MedForce.Web.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class VendorController : BaseController
	{
		private readonly ISecurityFacade _securityFacade;
		public VendorController(ISecurityFacade securityFacade)
		{
			_securityFacade = securityFacade;
		}

		public override IActionResult Index()
		{
			var validKeys = new List<SecurityKeyEnum> { SecurityKeyEnum.VendorView, SecurityKeyEnum.VendorEdit};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			return View();
		}
	}
}
