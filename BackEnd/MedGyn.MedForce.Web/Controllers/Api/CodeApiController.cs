using System.Collections.Generic;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Route("api/code")]
	[Authorize]
	public class CodeAPIController : BaseApiController
	{
		private readonly ICodeFacade _codeFacade;
		private readonly ISecurityFacade _securityFacade;

		public CodeAPIController(ICodeFacade codeFacade, ISecurityFacade securityFacade)
		{
			_codeFacade = codeFacade;
			_securityFacade = securityFacade;
		}

		[HttpPost, Route("")]
		public IActionResult FetchCodeTypes([FromBody] SearchCriteriaViewModel searchCriteria)
		{
			var isAuthorized = _securityFacade.IsAuthorized(new List<SecurityKeyEnum> {SecurityKeyEnum.CodeTypes, SecurityKeyEnum.Codes});
			if (!isAuthorized)
				return StatusCode(403);
			var results = _codeFacade.GetAllCodeTypes(searchCriteria);
			return Json(results);
		}

		[HttpPost, Route("codes")]
		public IActionResult FetchCodes([FromBody] SearchCriteriaViewModel searchCriteria, CodeTypeEnum codeTypeID)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.Codes);
			if (!isAuthorized)
				return StatusCode(403);
			var results = _codeFacade.GetCodesByType(searchCriteria, codeTypeID);
			return Json(results);
		}

		[HttpPost, Route("save")]
		public IActionResult SaveCodeTypes([FromBody] List<CodeTypeViewModel> codeTypes)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.CodeTypes);
			if (!isAuthorized)
				return StatusCode(403);
			var results = _codeFacade.SaveCodeTypes(codeTypes);
			return Json(results);
		}

		[HttpPost, Route("codes/save")]
		public IActionResult SaveCodes([FromBody] List<CodeViewModel> codes, CodeTypeEnum codeTypeID)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.Codes);
			if (!isAuthorized)
				return StatusCode(403);
			var results = _codeFacade.SaveCodes(codes, codeTypeID);
			return Json(results);
		}
	}
}
