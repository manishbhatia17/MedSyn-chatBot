using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Authorize]
	[Route("api/purchaseOrder")]
	public class PurchaseOrderAPIController : BaseApiController
	{
		private readonly ISecurityFacade _securityFacade;
		private readonly IPurchaseOrderFacade _purchaseOrderFacade;

		public PurchaseOrderAPIController(IPurchaseOrderFacade purchaseOrderFacade, ISecurityFacade securityFacade)
		{
			_purchaseOrderFacade = purchaseOrderFacade;
			_securityFacade = securityFacade;
		}

		[HttpPost, Route("")]
		public IActionResult FetchPurchaseOrders([FromBody] SearchCriteriaViewModel searchCriteria, PurchaseOrderStatusEnum status, DateRangeEnum timeframe)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderView,
				SecurityKeyEnum.PurchaseOrderEdit,
				SecurityKeyEnum.PurchaseOrderDomesticView,
				SecurityKeyEnum.PurchaseOrderDomesticEdit,
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionView,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalView,
				SecurityKeyEnum.PurchaseOrderInternationalEdit,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,
				SecurityKeyEnum.PurchaseOrderReceive};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var purchaseOrderListModel = _purchaseOrderFacade.GetPurchaseOrderListViewModel(searchCriteria, status, timeframe);
			return Json(purchaseOrderListModel);
		}

		[HttpGet, Route("{purchaseOrderID}")]
		public IActionResult GetPurchaseOrderDetails(int purchaseOrderID, int? productID, int? priVendorID)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderView,
				SecurityKeyEnum.PurchaseOrderEdit,
				SecurityKeyEnum.PurchaseOrderDomesticView,
				SecurityKeyEnum.PurchaseOrderDomesticEdit,
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionView,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalView,
				SecurityKeyEnum.PurchaseOrderInternationalEdit,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,
				SecurityKeyEnum.PurchaseOrderReceive};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var model = _purchaseOrderFacade.GetPurchaseOrderDetails(purchaseOrderID, productID, priVendorID);
			return Json(model);
		}

		[HttpDelete, Route("{purchaseOrderID}")]
		public IActionResult DeletePurchaseOrderDetails(int purchaseOrderID)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderView,
				SecurityKeyEnum.PurchaseOrderEdit,
				SecurityKeyEnum.PurchaseOrderDomesticView,
				SecurityKeyEnum.PurchaseOrderDomesticEdit,
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionView,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalView,
				SecurityKeyEnum.PurchaseOrderInternationalEdit,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,
				SecurityKeyEnum.PurchaseOrderReceive};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var results = _purchaseOrderFacade.DeletePurchaseOrder(purchaseOrderID);
			return Json(results);
		}

		[HttpGet, Route("{purchaseOrderID}/receive")]
		public async Task<IActionResult> GetPurchaseOrderReceipt(int purchaseOrderID)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PurchaseOrderReceive);
			if (!isAuthorized)
				return StatusCode(403);
			var model = await _purchaseOrderFacade.GetPurchaseOrderReceipt(purchaseOrderID);
			return Json(model);
		}

		[HttpGet, Route("productHistory")]
		public IActionResult GetPurchaseOrderHistoryForProduct(int productID)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderView,
				SecurityKeyEnum.PurchaseOrderEdit,
				SecurityKeyEnum.PurchaseOrderDomesticView,
				SecurityKeyEnum.PurchaseOrderDomesticEdit,
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionView,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalView,
				SecurityKeyEnum.PurchaseOrderInternationalEdit,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,
				SecurityKeyEnum.PurchaseOrderReceive};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var model = _purchaseOrderFacade.GetPurchaseOrderHistoryForProduct(productID);
			return Json(model);
		}

		[HttpPost, Route("save")]
		public async Task<IActionResult> SavePurchaseOrder([FromBody] PurchaseOrderViewModel purchaseOrder, bool submit)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderEdit,
				SecurityKeyEnum.PurchaseOrderDomesticEdit,
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalEdit,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (isAuthorized)
				isAuthorized = _purchaseOrderFacade.CanEditOrder(purchaseOrder);
			if (!isAuthorized)
				return StatusCode(403);
			var purchaseOrderModel = await _purchaseOrderFacade.SavePurchaseOrder(purchaseOrder, submit);
			return Json(purchaseOrderModel);
		}

		[HttpPost, Route("{purchaseOrderID}/receiptComplete")]
		public IActionResult ReceiptComplete(int purchaseOrderID, [FromBody] IList<PurchaseOrderProductReceiptViewModel> products)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PurchaseOrderReceive);
			if (!isAuthorized)
				return StatusCode(403);
			var results = _purchaseOrderFacade.ReceiptComplete(purchaseOrderID, products);
			return Json(results);
		}

		[HttpPost, Route("{purchaseOrderID}/approve")]
		public IActionResult ApprovePurchaseOrder(int purchaseOrderID)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (isAuthorized)
				isAuthorized = _purchaseOrderFacade.CanApproveOrder(purchaseOrderID);
			if (!isAuthorized)
				return StatusCode(403);
			var result = _purchaseOrderFacade.ApprovePurchaseOrder(purchaseOrderID);
			return Json(result);
		}

		[HttpPost, Route("{purchaseOrderID}/rescind")]
		public IActionResult RescindPurchaseOrder(int purchaseOrderID)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PurchaseOrderRescind);
			if (!isAuthorized)
				return StatusCode(403);
			var result = _purchaseOrderFacade.RescindPurchaseOrder(purchaseOrderID);
			return Json(result);
		}

		[HttpGet, Route("SendReport/{purchaseOrderID}")]
		public IActionResult GetPurchaseOrderReport(int purchaseOrderID)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderView,
				SecurityKeyEnum.PurchaseOrderEdit,
				SecurityKeyEnum.PurchaseOrderDomesticView,
				SecurityKeyEnum.PurchaseOrderDomesticEdit,
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionView,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalView,
				SecurityKeyEnum.PurchaseOrderInternationalEdit,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,
				SecurityKeyEnum.PurchaseOrderReceive};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);

			var result = _purchaseOrderFacade.SendPurchaseOrderReport(purchaseOrderID);
			if(!result.Success)
			{
				Response.StatusCode = (int) HttpStatusCode.NotFound;
				return Json(result);
			}

			return Ok();
		}
	}
}
