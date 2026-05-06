using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace MedGyn.MedForce.Web.wwwroot.js.App.Controllers
{
	public class OnHandActivityController : BaseController
	{
		private readonly ISecurityFacade _securityFacade;
		
		public OnHandActivityController(ISecurityFacade securityFacade)
		{
			_securityFacade = securityFacade;
		}
	}
}
