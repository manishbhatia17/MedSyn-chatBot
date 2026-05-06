using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MedGyn.MedForce.Web.Controllers
{
	[Authorize]
	public class UserController : BaseController
	{
		private readonly ISecurityFacade _securityFacade;
		private readonly IUserFacade _userFacade;

		public UserController(ISecurityFacade securityFacade, IUserFacade userfacade)
		{
			_securityFacade = securityFacade;
			_userFacade     = userfacade;
		}

		public override IActionResult Index()
		{
			var validKeys    = new List<SecurityKeyEnum> {SecurityKeyEnum.UserView, SecurityKeyEnum.UserEdit};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			return View();
		}

		[HttpGet]
		public IActionResult Edit(int userId)
		{
			var editModel = _userFacade.GetUser(userId);
			return View(editModel);
		}

		[HttpPost]
		public IActionResult AddUser(UserViewModel model)
		{
			if (model.UserId == 0) // new User (create)
			{

			}
			else // existing user (update)
			{

			}
			return Json("Saved");
		}
	}
}