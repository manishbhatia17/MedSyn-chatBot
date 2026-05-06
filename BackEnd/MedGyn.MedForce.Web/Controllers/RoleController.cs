using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MedGyn.MedForce.Web.Controllers
{
	[Authorize]
	public class RoleController : BaseController
	{
		private readonly ISecurityFacade _securityFacade;

		public RoleController(ISecurityFacade securityFacade)
		{
			_securityFacade = securityFacade;
		}

		public override IActionResult Index()
		{
			var validKeys = new List<SecurityKeyEnum> { SecurityKeyEnum.RoleView, SecurityKeyEnum.RoleEdit };
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if(!isAuthorized)
				return StatusCode(403);
			return View();
		}
	}
}