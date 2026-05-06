using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Route("api/role")]
	[Authorize]
	public class RoleAPIController : BaseApiController
	{
		private readonly ISecurityFacade _securityFacade;

		public RoleAPIController(ISecurityFacade securityFacade)
		{
			_securityFacade = securityFacade;
		}

		[HttpPost, Route("")]
		public IActionResult FetchRoles([FromBody] SearchCriteriaViewModel searchCriteria)
		{
			var results = _securityFacade.GetRolesList(searchCriteria);
			return Json(results);
		}

		[HttpPost, Route("keys")]
		public IActionResult FetchKeysForRole([FromBody] int roleId)
		{
			var results = _securityFacade.GetSecurityKeysForRole(roleId);
			return Json(results);
		}

		[HttpPost, Route("save")]
		public IActionResult SaveRoles([FromBody] List<RoleViewModel> roles)
		{
			var validKeys = new List<SecurityKeyEnum> { SecurityKeyEnum.RoleEdit };
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var results = _securityFacade.SaveRoles(roles);
			return Json(results);
		}

		[HttpPost, Route("saveKeys")]
		public IActionResult SaveSecurityKeys([FromBody] RolePermissionsViewModel rolePermissions)
		{
			var validKeys = new List<SecurityKeyEnum> { SecurityKeyEnum.RoleEdit };
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var results = _securityFacade.SaveRolePermissions(rolePermissions);
			return Json(results);
		}
	}
}
