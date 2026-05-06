using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MedGyn.MedForce.Web.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class CodeTypeController : BaseController
	{
		private readonly ISecurityFacade _securityFacade;
		public CodeTypeController(ISecurityFacade securityFacade)
		{
			_securityFacade = securityFacade;
		}

		[Route("{codeTypeID}/codes")]
		public IActionResult Codes(int codeTypeID)
		{
			var isAuthorized = _securityFacade.IsAuthorized(new List<SecurityKeyEnum> {SecurityKeyEnum.CodeTypes, SecurityKeyEnum.Codes});
			if (!isAuthorized)
				return StatusCode(403);
			return View(codeTypeID);
		}
	}
}
