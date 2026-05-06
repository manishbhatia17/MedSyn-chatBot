using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using MedGyn.MedForce.Facade.DTOs;
using System.Text.RegularExpressions;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Route("api/product")]
	[Authorize]
	public class ProductAPIController : BaseApiController
	{
		private readonly IProductFacade _productFacade;
		private readonly ISecurityFacade _securityFacade;
		public ProductAPIController(IProductFacade productFacade, ISecurityFacade securityFacade)
		{
			_productFacade = productFacade;
			_securityFacade = securityFacade;
		}

		[HttpPost, Route("")]
		public IActionResult FetchProducts([FromBody] SearchCriteriaViewModel searchCriteria, bool? showDiscontinued)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ProductView,
				SecurityKeyEnum.ProductEdit,
				SecurityKeyEnum.ExportProductList,
				SecurityKeyEnum.InventoryAdjust};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var productListModel = _productFacade.GetProductListViewModel(searchCriteria, showDiscontinued ?? false);
			return Json(productListModel);
		}

		[HttpPost, Route("price")]
		public IActionResult GetProductsPrice([FromBody] SearchCriteriaViewModel searchCriteria, bool? showDiscontinued)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PriceAdjustmentEdit};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);

			var productListModel = _productFacade.GetProductsPriceListViewModel(searchCriteria, showDiscontinued ?? false);
			return Json(productListModel);
		}

		[HttpPost, Route("price/save")]
		public async Task<IActionResult> SaveProductPriceAdjustments([FromBody] List<ProductPriceViewModel> productPriceViewModels)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PriceAdjustmentEdit};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);

			var result = await _productFacade.SaveProductPriceAdjustmentsAsync(productPriceViewModels);
			return Json(result);
		}

		[HttpGet, Route("{productID}")]
		public IActionResult GetProductDetails(int productID)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ProductView,
				SecurityKeyEnum.ProductEdit,
				SecurityKeyEnum.CostVisible};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var model = _productFacade.GetProductDetails(productID);
			return Json(model);
		}

		[HttpGet, Route("{productID}/adjustments")]
		public IActionResult GetProductInventoryAdjustments(int productID)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.InventoryAdjust);
			if (!isAuthorized)
				return StatusCode(403);
			var productAdjustmentsModel = _productFacade.GetProductInventoryAdjustments(productID);
			return Json(productAdjustmentsModel);
		}

		[HttpPost, Route("save")]
		public async Task<IActionResult> SaveProduct([FromBody] ProductViewModel product)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.ProductEdit);
			if (!isAuthorized)
				return StatusCode(403);
			//TODO: remove async, only return SaveResults
			var res = await _productFacade.SaveProduct(product);
			return Json(res.result);
		}

		[HttpPost, Route("saveInv")]
		public async Task<IActionResult> SaveProductAdjustment([FromBody] ProductInventoryAdjustmentViewModel productAdjustment)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.InventoryAdjust);
			if (!isAuthorized)
				return StatusCode(403);
			var productAdjustmentModel = await _productFacade.SaveProductAdjustment(productAdjustment);
			return Json(productAdjustmentModel);
		}

		[HttpPost, Route("export/excel")]
		public IActionResult ExportProductListExcel([FromBody] SearchCriteriaViewModel sc)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.ExportProductList);
			if (!isAuthorized)
				return StatusCode(403);

			var cd = new ContentDisposition()
			{
				FileName = "ProductList.xlsx"
			};

			Response.Headers.Add("Content-Disposition", cd.ToString());
			return File(_productFacade.ExportProductListExcel(sc), "application/excel");
		}

		[HttpPost, Route("export/price/excel")]
		public IActionResult ExportProductPriceListExcel([FromBody] SearchCriteriaViewModel sc)
		{
			var isAuthorized = _securityFacade.IsAuthorized(new List<SecurityKeyEnum>() { SecurityKeyEnum.PriceAdjustmentEdit });
			if (!isAuthorized)
				return StatusCode(403);

			var cd = new ContentDisposition()
			{
				FileName = "ProductPrice.xlsx"
			};

			Response.Headers.Add("Content-Disposition", cd.ToString());
			return File(_productFacade.ExportProductPriceListExcel(sc), "application/excel");
		}

		[HttpPost, Route("import/price/excel")]
		public async Task<IActionResult> ImportProductPriceListExcel([FromBody] CustomerOrderUploadDTO OrderUpload)
		{
			var isAuthorized = _securityFacade.IsAuthorized(new List<SecurityKeyEnum>() { SecurityKeyEnum.PriceAdjustmentEdit });
			if (!isAuthorized)
				return StatusCode(403);

			//TODO: read csv into List<ProductPriceViewModel>
			var regex = new Regex(@"^data:.*;base64");
			string file = regex.IsMatch(OrderUpload.File ?? "") ? OrderUpload.File : null;

			if (!string.IsNullOrWhiteSpace(file))
			{
				var split = file.Split(';');
				var fileType = split[0].Substring("data:".Length);

				if(!fileType.Contains("csv"))
				{
					throw new System.Exception("Invalid file type. Please upload a csv file.");
				}
				var data = split[1].Substring("base64,".Length); // can't convert the "base64,"
				byte[] csv = System.Convert.FromBase64String(data);
				using (MemoryStream ms = new MemoryStream(csv))
				{
					using (TextReader sr = new StreamReader(ms))
					{
						using (CsvHelper.CsvReader csvReader = new CsvHelper.CsvReader(sr, System.Globalization.CultureInfo.CurrentCulture))
						{
							var productPriceImport = csvReader.GetRecords<CustomerPriceImportCSV>();

							var result = await _productFacade.SaveProductPriceAdjustmentsAsync(productPriceImport.Select(s => s.ToProductPriceViewModel()).ToList());
							return Json(result);
						}
					}
				}

			}
			else
			{
				return BadRequest("file was either empty or corrupted. Please upload and try again.");
			}
		}
	}
}