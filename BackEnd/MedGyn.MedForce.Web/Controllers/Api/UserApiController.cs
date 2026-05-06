using System.Collections.Generic;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Route("api/user")]
	public class UserAPIController : BaseApiController
	{
		private readonly ISecurityFacade _securityFacade;
		private readonly IUserFacade _userFacade;

		public UserAPIController(ISecurityFacade securityFacade, IUserFacade userfacade)
		{
			_securityFacade = securityFacade;
			_userFacade = userfacade;
		}


		[HttpPost, Route("")]
		public IActionResult FetchUsers([FromBody] SearchCriteriaViewModel searchCriteria)
		{
			var validKeys = new List<SecurityKeyEnum> { SecurityKeyEnum.UserView, SecurityKeyEnum.UserEdit };
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var userList = _userFacade.GetCustomerListViewModel(searchCriteria);
			return Json(userList);
		}

		[HttpGet, Route("roles")]
		public IActionResult FetchRoles()
		{
			var validKeys = new List<SecurityKeyEnum> { SecurityKeyEnum.UserView, SecurityKeyEnum.UserEdit };
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var roles = _userFacade.GetActiveRoles();
			return Json(roles);
		}


		[HttpPost("saveUser")]
		public IActionResult SaveUser([FromBody] UserDisplayViewModel user)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.UserEdit);
			if (!isAuthorized)
				return StatusCode(403);
			var result = _userFacade.SaveUser(user);

			return Json(result);
		}

		[HttpDelete, Route("")]
		public IActionResult DeleteUser([FromBody] UserDisplayViewModel user)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.UserEdit);
			if (!isAuthorized)
				return StatusCode(403);
			var result = _userFacade.DeleteUser(user);

			return Json(result);
		}

	}
}