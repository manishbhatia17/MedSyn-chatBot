using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers
{
	public class InvoiceActivityController : BaseController
	{
		private readonly ISecurityFacade _securityFacade;

		public InvoiceActivityController(ISecurityFacade securityFacade)
		{
			_securityFacade = securityFacade;
		}
	}
}
