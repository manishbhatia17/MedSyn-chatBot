using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedGyn.MedForce.Facade.Interfaces;
using System.Collections.Generic;
using System.Net.Mime;

namespace MedGyn.MedForce.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ProductShippedController : BaseController
    {
        private readonly ISecurityFacade _securityFacade;
        private readonly ICustomerOrderFacade _customerOrderFacade;
        public ProductShippedController(ISecurityFacade securityFacade, ICustomerOrderFacade customerOrderFacade)
        {
            _securityFacade = securityFacade;
            _customerOrderFacade = customerOrderFacade;
        }

		public override IActionResult Index()
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ProductShippedSeeAllNoTotals,
            SecurityKeyEnum.ProductShippedSeeAllWithTotals,
            SecurityKeyEnum.ProductShippedView,
            SecurityKeyEnum.ProductShippedViewTotals};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			return View();
		}
	}
}
