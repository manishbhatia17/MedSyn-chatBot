using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Facade.ViewModels.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MedGyn.MedForce.Web.Controllers.Api
{
    [Authorize]
    [Route("api/onHandActivity")]
    public class OnHandActivityApiController : BaseApiController
	{
		private readonly ISecurityFacade _securityFacade;
        private readonly IProductFacade _productFacade;

		public OnHandActivityApiController(ISecurityFacade securityFacade, IProductFacade productFacade)
		{
			_securityFacade = securityFacade;
            _productFacade = productFacade;
		}

        [HttpPost, Route("")]
        public IActionResult Index([FromBody] SearchCriteriaViewModel searchCriteriaViewModel, DateTime StartDate, DateTime EndDate, string SelectedProduct)
		{
            try
            {
                var validKeys = new List<SecurityKeyEnum>
            {
                SecurityKeyEnum.ProductActivityView};
                var isAuthorized = _securityFacade.IsAuthorized(validKeys);
                if (!isAuthorized)
                    return StatusCode(403);

                OnHandActivityReportViewModel viewModel = _productFacade.GetOnHandActivityReport(StartDate, EndDate, Convert.ToInt32(SelectedProduct));
                OnHandActivityListViewModel onHandActivityListViewModel = new OnHandActivityListViewModel(new SearchCriteriaViewModel(), viewModel.ProductActivities);

                Dictionary<string, object> outputList = new Dictionary<string, object>();
                outputList["results"] = onHandActivityListViewModel.Results.OrderBy(o => o.ActivityDate);
                outputList["start"] = onHandActivityListViewModel.Start;
                outputList["end"] = onHandActivityListViewModel.End;
                outputList["currentPage"] = onHandActivityListViewModel.CurrentPage;
                outputList["totalPages"] = onHandActivityListViewModel.TotalPages;
                outputList["totalResults"] = onHandActivityListViewModel.TotalResults;
                outputList["beginningTotal"] = viewModel.BeginningTotal;
                outputList["endingTotal"] = viewModel.EndingTotal;

                return Json(outputList);
            }
			catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

		}
	}
}
