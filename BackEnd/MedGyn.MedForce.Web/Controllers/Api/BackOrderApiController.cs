using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MedGyn.MedForce.Web.Controllers.Api
{
    [Authorize]
    [Route("api/backOrder")]
    public class BackOrderApiController : BaseApiController
    {
        private readonly ISecurityFacade _securityFacade;
        private readonly ICustomerOrderFacade _customerOrderFacade;

        public BackOrderApiController(ICustomerOrderFacade customerOrderFacade, ISecurityFacade securityFacade)
        {
            _customerOrderFacade = customerOrderFacade;
            _securityFacade = securityFacade;
        }

		[HttpPost, Route("")]
		public async Task<IActionResult> fetchBackOrders([FromBody] SearchCriteriaViewModel searchCriteria, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.BackorderView,
				SecurityKeyEnum.BackorderSeeAll,
				SecurityKeyEnum.BackorderViewWithTotals,
			SecurityKeyEnum.BackorderSeeAllNoTotals};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var backOrderListModel = await _customerOrderFacade.GetBackOrderListViewModel(searchCriteria, status, dateOption);

			double total_amount = 0;
			int total_quantity = 0;
			foreach (var item in backOrderListModel.AllItems)
			{

				total_amount += item.Quantity * Decimal.ToDouble((decimal)item.Price);
				total_quantity += item.Quantity;
			}

			if (_securityFacade.IsAuthorized(validKeys))
			{
				backOrderListModel.TotalAmount = total_amount;

			}
			else
			{
				backOrderListModel.TotalAmount = -1;

			}
			backOrderListModel.TotalQty = total_quantity;

			return Json(backOrderListModel);
		}

		[HttpPost, Route("export/excel")]
		public async Task<IActionResult> ExportProductListExcel([FromBody] SearchCriteriaViewModel sc, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.ExportProductList);
			if (!isAuthorized)
				return StatusCode(403);

			var cd = new ContentDisposition()
			{
				FileName = "BackOrders.xlsx"
			};

			return File(await _customerOrderFacade.ExportProductListExcel(sc,1, status, dateOption), "application/excel");
		}
	}
}
