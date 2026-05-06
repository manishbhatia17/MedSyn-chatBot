using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers
{
	public abstract class BaseController : Controller
	{
		public virtual IActionResult Index()
		{
			return View();
		}

		[Route("details/{id}")]
		public virtual IActionResult Details(int id)
		{
			return View(id);
		}
	}
}
