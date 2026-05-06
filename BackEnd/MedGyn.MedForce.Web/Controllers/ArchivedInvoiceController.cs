using System.Collections.Generic;
using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ArchivedInvoiceController : BaseController
    {
        private readonly ISecurityFacade _securityFacade;
        private readonly ICustomerOrderFacade _customerOrderFacade;
        public ArchivedInvoiceController(ISecurityFacade securityFacade, ICustomerOrderFacade customerOrderFacade)
        {
            _securityFacade = securityFacade;
            _customerOrderFacade = customerOrderFacade;
        }

		public override IActionResult Index()
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ArchiveSeeAll,
				SecurityKeyEnum.ArchiveView,
				SecurityKeyEnum.ArchiveSeeAllNoTotals,
				SecurityKeyEnum.ArchiveViewWithTotals};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			return View();
		}
	}
}
