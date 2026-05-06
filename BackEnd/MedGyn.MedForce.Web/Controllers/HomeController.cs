using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers
{
	public class HomeController : BaseController
	{
		public override IActionResult Index()
		{
			return RedirectToAction("Login", "Account");
		}
	}
}
