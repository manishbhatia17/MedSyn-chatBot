using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedGyn.MedForce.Facade.Interfaces;
using System.Collections.Generic;
using System.Net.Mime;

namespace MedGyn.MedForce.Web.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class CustomerOrderController : BaseController
	{
		private readonly ISecurityFacade _securityFacade;
		private readonly ICustomerOrderFacade _customerOrderFacade;
		public CustomerOrderController(ISecurityFacade securityFacade, ICustomerOrderFacade customerOrderFacade)
		{
			_securityFacade      = securityFacade;
			_customerOrderFacade = customerOrderFacade;
		}

		public override IActionResult Index()
		{
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
			return View();
		}

		[Route("fill/{id}")]
		public virtual IActionResult Fill(int id)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerOrderFulfillment);
			if (!isAuthorized)
				return StatusCode(403);
			_customerOrderFacade.MarkOrderAsFilling(id);
			return View(id);
		}

		[Route("ship/{id}")]
		public virtual IActionResult Ship(int id)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerOrderShippable);
			if (!isAuthorized)
				return StatusCode(403);
			return View(id);
		}

		[Route("report/{customerOrderID}")]
		public virtual IActionResult GetCustomerOrderReport(int customerOrderID, [FromQuery] string documentTitle)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderView,
				SecurityKeyEnum.CustomerOrderRescindOrder,
				SecurityKeyEnum.CustomerDoNotFillFlag,
				SecurityKeyEnum.CustomerOrderExport};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);

			var data = _customerOrderFacade.GetCustomerOrderReportPdf(customerOrderID, documentTitle);
			var cd = new ContentDisposition {
				FileName = "Customer-order.pdf",
				Inline = true
			};

			Response.Headers.Add("Content-Disposition", cd.ToString());
			return File(data, "application/pdf");
		}

		[Route("invoice/{shipmentID}")]
		public virtual IActionResult Invoice(int shipmentID, bool? html)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ToBeInvoiced,
				SecurityKeyEnum.CustomerOrderView,
			};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);

			if(html ?? false)
			{
				var body = _customerOrderFacade.GetInvoiceHtml(shipmentID);
				return Content(body, "text/html");
			}

			var data = _customerOrderFacade.GetInvoice(shipmentID);
			var cd = new ContentDisposition {
				FileName = $"Invoice.pdf",
				Inline = true
			};

			Response.Headers.Add("Content-Disposition", cd.ToString());
			return File(data, "application/pdf");
		}

		[Route("PackingSlip/{shipmentID}/{generateMultiplePackingSlip}")]
		public virtual IActionResult PackingSlip(int shipmentID,bool? generateMultiplePackingSlip, bool? html)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ToBeInvoiced,
				SecurityKeyEnum.CustomerOrderFulfillment
			};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);

			if (html ?? false)
			{
				var body = _customerOrderFacade.GetPackingSlipHtml(shipmentID, generateMultiplePackingSlip);
				return Content(body, "text/html");
			}

			var data = _customerOrderFacade.GetPackingSlipPdf(shipmentID, generateMultiplePackingSlip);
			var cd = new ContentDisposition {
				FileName = $"Packing-slip.pdf",
				Inline = true
			};

			Response.Headers.Add("Content-Disposition", cd.ToString());
			return File(data, "application/pdf");
		}
	}
}