using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers.Api
{
    [Route("api/ProductShipped")]
    [ApiController]
    public class ProductShippedApiController : BaseApiController
    {
		private readonly ISecurityFacade _securityFacade;
		private readonly ICustomerOrderFacade _customerOrderFacade;

		public ProductShippedApiController(ICustomerOrderFacade customerOrderFacade, ISecurityFacade securityFacade)
		{
			_customerOrderFacade = customerOrderFacade;
			_securityFacade = securityFacade;
		}

		[HttpPost, Route("")]
		public async Task<IActionResult> FetchProductShippeds([FromBody] SearchCriteriaViewModel searchCriteria, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ProductShippedView,
				SecurityKeyEnum.ProductShippedViewTotals,
				SecurityKeyEnum.ProductShippedSeeAllNoTotals,
				SecurityKeyEnum.ProductShippedSeeAllWithTotals
			};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);

			try
			{
				var results = await _customerOrderFacade.GetFilteredProductShippedViewModel(searchCriteria);
				double total_amount = 0;
				int total_quantity = 0;
				foreach (var item in results)
				{
					IDictionary<string, object> propertyValues = item;
					var quantity = "Quantity";
					var price = "Price";
					total_amount += (int)propertyValues[quantity] * Decimal.ToDouble((decimal)propertyValues[price]);
					total_quantity += (int)propertyValues[quantity];
				}
				var archivedInvoiceListModel = await _customerOrderFacade.ConvertFilteredListForDisplay(searchCriteria, results);
				Dictionary<string, object> outputList = new Dictionary<string, object>();
				outputList["results"] = archivedInvoiceListModel.Results;
				outputList["start"] = archivedInvoiceListModel.Start;
				outputList["end"] = archivedInvoiceListModel.End;
				outputList["currentPage"] = archivedInvoiceListModel.CurrentPage;
				outputList["totalPages"] = archivedInvoiceListModel.TotalPages;
				outputList["totalResults"] = archivedInvoiceListModel.TotalResults;
			
				if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> { SecurityKeyEnum.ProductShippedViewTotals }))
				{
					outputList["totalAmount"] = total_amount;

				}
				else
				{
					outputList["totalAmount"] = null;

				}
				outputList["totalQuantity"] = total_quantity;
				return Json(outputList);
			}
			catch (Exception ex)
			{
                return StatusCode(500, ex.Message);
            }
		}

		[HttpPost, Route("export/excel")]
		public async Task<IActionResult> ExportProductListExcel([FromBody] SearchCriteriaViewModel sc, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
		{
			try
			{
				var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.ExportProductList);
				if (!isAuthorized)
					return StatusCode(403);

				var cd = new ContentDisposition()
				{
					FileName = "ProductsShipped.xlsx"
				};

				Response.Headers.Add("Content-Disposition", cd.ToString());
				return File(await _customerOrderFacade.ExportProductListExcel(sc, 2, status, dateOption), "application/excel");
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}