using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers
{
	public class StatusController : BaseController
	{
		private readonly IStatusFacade _statusFacade;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StatusController(IStatusFacade statusFacade, IHttpContextAccessor httpContextAccessor)
		{
			_statusFacade = statusFacade;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		new public IActionResult Index()
		{
			var model = _statusFacade.GetStatus();

			return View(model);
		}

		[HttpPost]
		public IActionResult Index(StatusViewModel statusViewModel)
		{
			_statusFacade.UpdateDatabase(statusViewModel);
			return View(statusViewModel);
		}
	}
}
