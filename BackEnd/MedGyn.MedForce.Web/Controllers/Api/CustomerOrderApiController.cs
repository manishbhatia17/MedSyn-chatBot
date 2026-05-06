using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Facades;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Authorize]
	[Route("api/customerOrder")]
	public class CustomerOrderAPIController : BaseApiController
	{
		private readonly ISecurityFacade _securityFacade;
		private readonly ICustomerOrderFacade _customerOrderFacade;
		private readonly ILogger<CustomerOrderAPIController> _logger;

		public CustomerOrderAPIController(ICustomerOrderFacade customerOrderFacade, ISecurityFacade securityFacade, ILogger<CustomerOrderAPIController> Logger)
		{
			_customerOrderFacade = customerOrderFacade;
			_securityFacade = securityFacade;
			_logger = Logger;
		}

		[HttpPost, Route("")]
		public async Task<IActionResult> FetchCustomerOrders([FromBody] SearchCriteriaViewModel searchCriteria, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
		{
			try { 
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderView,
				SecurityKeyEnum.CustomerOrderEdit,
				SecurityKeyEnum.CustomerOrderRescindOrder,
				SecurityKeyEnum.CustomerOrderExport,
				SecurityKeyEnum.CustomerOrderCustomersSeeAll,
				SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				SecurityKeyEnum.CustomerOrderDomesticVPApproval,
				SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
				SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				SecurityKeyEnum.CustomerOrderInternationalVPApproval,
				SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				SecurityKeyEnum.CustomerOrderShippable,
				SecurityKeyEnum.CustomerOrderShipRescind,
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var customerOrderListModel = await _customerOrderFacade.GetCustomerOrderListViewModel(searchCriteria, status, dateOption);

			return Json(customerOrderListModel);
		}
			catch (Exception ex)
			{
                _logger.LogError(ex, "Error fetching customer orders");
                return StatusCode(500);
            }
		}

		[HttpGet, Route("{customerOrderID}")]
		public IActionResult GetCustomerOrderDetails(int customerOrderID, int? customerID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderView,
				SecurityKeyEnum.CustomerOrderEdit,
				SecurityKeyEnum.CustomerOrderRescindOrder,
				SecurityKeyEnum.CustomerDoNotFillFlag,
				SecurityKeyEnum.CustomerOrderExport};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var model = _customerOrderFacade.GetCustomerOrderDetails(customerOrderID, customerID);
				return Json(model);
			}
            catch (Exception ex)
			{
                _logger.LogError(ex, "Error fetching customer order details");
                return StatusCode(500);
			}
		}

		[HttpPost, Route("fill")]
		public IActionResult GetFilledOrders([FromBody] SearchCriteriaViewModel searchCriteriaViewModel, DateTime StartDate, DateTime EndDate)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum>
			{
				SecurityKeyEnum.PriceAdjustmentEdit
			};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var model = _customerOrderFacade.GetPriceReconciliationList(searchCriteriaViewModel, StartDate, EndDate);
				return Json(model);
			}
			catch (Exception ex)
			{
                _logger.LogError(ex, "Error fetching filled orders");
                return StatusCode(500);
            }
		}

        [HttpPost, Route("fill/export/excel")]
        public IActionResult GetFilledOrdersExcelExport([FromBody] SearchCriteriaViewModel searchCriteriaViewModel, DateTime StartDate, DateTime EndDate)
        {
			try
			{
				var validKeys = new List<SecurityKeyEnum>
			{
				SecurityKeyEnum.PriceAdjustmentEdit
			};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);

				var cd = new ContentDisposition()
				{
					FileName = "PriceReconciliationReport.xlsx"
				};

				Response.Headers.Add("Content-Disposition", cd.ToString());
				var userID = _securityFacade.GetUserId();
				var seeAll = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerSeeAll);

				bool seeDomestic = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticEdit);
				bool seeDomesticDistribution = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticDistributionView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticDistributionEdit);
				bool seeDomesticAfaxys = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticAfaxysView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticAfaxysEdit);
				bool seeInternational = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerInternationalView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerInternationalView);

				return File(_customerOrderFacade.ExportPriceReconciliationExcel(searchCriteriaViewModel, StartDate, EndDate), "application/excel");
			}
			catch (Exception ex)
			{
                _logger.LogError(ex, "Error exporting filled orders");
                return StatusCode(500);
            }	
        }

        [HttpGet, Route("{customerOrderID}/fill")]
		public IActionResult GetCustomerOrderFill(int customerOrderID, int? boxID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var model = _customerOrderFacade.GetCustomerOrderFill(customerOrderID, boxID);
				return Json(model);
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error fetching customer order fill");
                return StatusCode(500);
			}
		}

		[HttpPost, Route("{customerOrderID}/DoNotFill")]
		public IActionResult GetCustomerDoNotFill(int customerOrderID, [FromBody] DoNotFillViewModel doNotFillViewModel)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);

				_customerOrderFacade.SetCustomerOrderToDoNotFill(customerOrderID, doNotFillViewModel);
				return Ok();
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error setting customer order to do not fill");
                return StatusCode(500);
            }
		}

		[HttpGet, Route("{customerOrderID}/ship/{shipmentID}")]
		public IActionResult GetCustomerOrderShip(int customerOrderID, int shipmentID, int? boxID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderShippable,
				SecurityKeyEnum.CustomerOrderShipRescind,};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var model = _customerOrderFacade.GetCustomerOrderShip(customerOrderID, shipmentID, boxID);
				return Json(model);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error fetching customer order ship");
                return StatusCode(500);
            }
		}

		[HttpGet, Route("productHistory")]
		public async Task<IActionResult> GetCustomerOrderHistoryForProduct(int customerID, int productID)
		{
			try
			{
				var model = await _customerOrderFacade.GetCustomerOrderHistoryForProduct(customerID, productID);
				return Json(model);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error fetching customer order history for product");
                return StatusCode(500);
            }
		}

        [HttpPost, Route("uploadOrder")]
        public async Task<IActionResult> CreateCustomerOrder([FromBody] CustomerOrderUploadDTO OrderUpload)
        {
            try
            {
                var validKeys = new List<SecurityKeyEnum> {
                SecurityKeyEnum.CustomerOrderEdit,
                SecurityKeyEnum.CustomerOrderRescindOrder,
                SecurityKeyEnum.CustomerOrderShippable,
                SecurityKeyEnum.CustomerOrderShipRescind,
                SecurityKeyEnum.CustomerOrderFulfillment,
                SecurityKeyEnum.CustomerOrderFulfillmentRescind};
                var isAuthorized = _securityFacade.IsAuthorized(validKeys);
                if (!isAuthorized)
                    return StatusCode(403);
				var customerOrderModel = await _customerOrderFacade.CreateCustomerOrderFromFile(OrderUpload.File);
                return Json(customerOrderModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer order");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("save")]
		public async Task<IActionResult> SaveCustomerOrder([FromBody] CustomerOrderViewModel customerOrder, bool submit)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderEdit,
				SecurityKeyEnum.CustomerOrderRescindOrder,
				SecurityKeyEnum.CustomerOrderShippable,
				SecurityKeyEnum.CustomerOrderShipRescind,
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var customerOrderModel = await _customerOrderFacade.SaveCustomerOrder(customerOrder, submit);
				return Json(customerOrderModel);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error saving customer order");
                return BadRequest(ex.Message);
            }
		}

		[HttpPost, Route("{customerOrderID}/mgapprove")]
		public IActionResult MGApproveCustomerOrder(int customerOrderID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				SecurityKeyEnum.CustomerOrderInternationalManagerApproval};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (isAuthorized)
					isAuthorized = _customerOrderFacade.CanManagerApproveOrder(customerOrderID);
				if (!isAuthorized)
					return StatusCode(403);
				var result = _customerOrderFacade.ApproveCustomerOrder(customerOrderID, false);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error approving customer order");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderID}/vpapprove")]
		public IActionResult VPApproveCustomerOrder(int customerOrderID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderDomesticVPApproval,
				SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
				SecurityKeyEnum.CustomerOrderInternationalVPApproval};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (isAuthorized)
					isAuthorized = _customerOrderFacade.CanVpApproveOrder(customerOrderID);
				if (!isAuthorized)
					return StatusCode(403);
				var result = _customerOrderFacade.ApproveCustomerOrder(customerOrderID, true);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error approving customer order");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderID}/financeapprove")]
		public IActionResult ApproveOrderFinancing(int customerOrderID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderFinanceApprover};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);

				var results = _customerOrderFacade.ApproveCustomerFinancing(customerOrderID);

				return Json(results);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error approving customer order financing");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderID}/delete")]
		public IActionResult DeleteCustomerOrder(int customerOrderID)
		{
			try
			{
				var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerOrderEdit);
				if (!isAuthorized)
					return StatusCode(403);
				var result =
					_customerOrderFacade.DeleteCustomerOrder(customerOrderID);
				return Json(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting customer order");
				return StatusCode(500);
			}
		}

		[HttpPost, Route("{customerOrderID}/rescind")]
		public IActionResult RescindCustomerOrder(int customerOrderID, bool isFilling)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderRescindOrder,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var result = isFilling ?
					_customerOrderFacade.RescindFillingCustomerOrder(customerOrderID) :
					_customerOrderFacade.RescindCustomerOrder(customerOrderID);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error rescinding customer order");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderShipmentID}/rescindShip")]
		public IActionResult RescindCustomerOrderShipment(int customerOrderShipmentID)
		{
			try
			{
				var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerOrderShipRescind);
				if (!isAuthorized)
					return StatusCode(403);
				var result = _customerOrderFacade.RescindCustomerOrderShipment(customerOrderShipmentID);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error rescinding customer order shipment");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderID}/fillComplete")]
		public IActionResult FillComplete(int customerOrderID, [FromBody] CustomerOrderProductFillCompleteViewModel data)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var res = _customerOrderFacade.FillComplete(customerOrderID, data);
				if (!res.Success)
				{
					Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					return Json(res);
				}
				return Json(new { CustomerOrderShipmentID = (res.Payload as CustomerOrderShipmentViewModel).CustomerOrderShipmentID });
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error completing customer order fill");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderID}/addBox")]
		public IActionResult AddBox(int customerOrderID, [FromBody] CustomerOrderProductFillCompleteViewModel data)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var result = _customerOrderFacade.AddBox(customerOrderID, data);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error adding box to customer order");
                return StatusCode(500);
            }
		}

		/// <summary>
		/// Adds box to shipment screen
		/// </summary>
		/// <param name="customerOrderID"></param>
		/// <param name="CustomerOrderShippingID"></param>
		/// <returns></returns>
		[HttpPost, Route("{customerOrderID}/ship/{CustomerOrderShippingID}/box")]
		public IActionResult AddShipmentBox(int customerOrderID, int CustomerOrderShippingID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var result = _customerOrderFacade.AddAnotherShippingBox(customerOrderID, CustomerOrderShippingID);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error adding box to customer order shipment");
                return StatusCode(500);
            }
		}

		/// <summary>
		/// Removes box from shipment screen
		/// </summary>
		/// <param name="customerOrderID"></param>
		/// <param name="CustomerOrderShippingID"></param>
		/// <returns></returns>
		[HttpDelete, Route("{customerOrderID}/ship/{CustomerOrderShippingID}/box/{boxID}")]
		public IActionResult DeleteShipmentBox(int customerOrderID, int CustomerOrderShippingID, int boxID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var result = _customerOrderFacade.RemoveShipmentBox(boxID);
				return Json(result);
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error removing box from customer order shipment");
                return StatusCode(500);
			}
		}

		[HttpPost, Route("{customerOrderID}/updateBox/{boxID}")]
		public IActionResult UpdateBox(int customerOrderID, int boxID, [FromBody] CustomerOrderProductFillCompleteViewModel data)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var result = _customerOrderFacade.UpdateBox(customerOrderID, boxID, data);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error updating box on customer order");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderID}/updateBoxDims/{boxID}")]
		public IActionResult UpdateBoxDims(int customerOrderID, int boxID, [FromBody] CustomerOrderShipViewModel ship, int shipmentID)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderShippable,
				SecurityKeyEnum.CustomerOrderShipRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				var result = _customerOrderFacade.UpdateBoxDims(customerOrderID, boxID, ship);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error updating box dimensions on customer order");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderID}/getRateQuote/{shipmentID}")]
		public IActionResult GetRateQuote(int customerOrderID, int shipmentID, [FromBody] CustomerOrderShipBoxViewModel curBox, int shippingCompany = 0, int shippingMethod = 0)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderShippable,
				SecurityKeyEnum.CustomerOrderShipRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				if (shippingCompany == 0 || shippingMethod == 0 || curBox == null)
					return Json(false);

				var result = _customerOrderFacade.GetRateQuote(customerOrderID, shipmentID, shippingCompany, shippingMethod, curBox);
				return Json(result);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error getting rate quote for customer order");
                return StatusCode(500);
            }
		}

		[HttpPost, Route("{customerOrderID}/completeShipment/{shipmentID}")]
		public IActionResult CompleteShipment(int customerOrderID, int shipmentID, [FromBody] CustomerOrderShipViewModel ship)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderShippable,
				SecurityKeyEnum.CustomerOrderShipRescind};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);
				if (ship == null || ship.Shipment == null || ship.ShipmentBox == null)
					return Json(false);

				var result = _customerOrderFacade.CreateOrder(customerOrderID, shipmentID, ship);
				return Json(result);
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error completing shipment for customer order");
                return StatusCode(500);
			}
		}

		[HttpPost, Route("Invoice/send")]
		public virtual IActionResult SendInvoice([FromBody] IList<int> customerOrderShipmentIDs)
		{
			try
			{
				var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ToBeInvoiced,
			};
				var isAuthorized = _securityFacade.IsAuthorized(validKeys);
				if (!isAuthorized)
					return StatusCode(403);

				var ret = new SaveResults();
				foreach (var shipmentID in customerOrderShipmentIDs)
				{
					var res = _customerOrderFacade.SendInvoice(shipmentID);
					if (!res.Success)
					{
						Response.StatusCode = (int)HttpStatusCode.InternalServerError;
						ret = res;
						break;
					}
				}

				return Json(ret);
			}
			catch(Exception ex)
			{
                _logger.LogError(ex, "Error sending invoice for customer order");
                return StatusCode(500);
			}
		}
	}
}
