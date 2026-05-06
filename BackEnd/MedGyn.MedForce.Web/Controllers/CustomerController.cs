using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MedGyn.MedForce.Web.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class CustomerController : BaseController 
	{ 
		private readonly ISecurityFacade _securityFacade;
		public CustomerController(ISecurityFacade securityFacade)
		{
			_securityFacade = securityFacade;
		}

		public override IActionResult Index()
		{
			var validKeys = new List<SecurityKeyEnum> { 
				SecurityKeyEnum.CustomerView,
				SecurityKeyEnum.CustomerEdit,
				SecurityKeyEnum.CustomerSeeAll,
				SecurityKeyEnum.ExportCustomerList};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if(!isAuthorized)
				return StatusCode(403);
			return View();
		}
	}
	
}
