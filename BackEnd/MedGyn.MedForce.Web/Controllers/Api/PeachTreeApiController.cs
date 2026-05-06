using System;
using System.Collections.Generic;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.IO;
using MedGyn.MedForce.Facade.ViewModels.CustomerOrder;
using System.Linq;
using MedGyn.MedForce.Facade.DTOs;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Authorize]
	[Route("api/peachtree")]
	public class PeachTreeAPIController : BaseApiController
	{
		private readonly ICustomerOrderFacade _customerOrderFacade;
		private readonly IPurchaseOrderFacade _purchaseOrderFacade;
		private readonly ISecurityFacade _securityFacade;

		public PeachTreeAPIController(ICustomerOrderFacade customerOrderFacade, IPurchaseOrderFacade purchaseOrderFacade, ISecurityFacade securityFacade)
		{
			_customerOrderFacade = customerOrderFacade;
			_purchaseOrderFacade = purchaseOrderFacade;
			_securityFacade = securityFacade;
		}

		[HttpGet, Route("export/invoice/list")]
		public IActionResult GetInvoiceExportList(int TopResults)
		{
			try
			{
				var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PeachTreeExportInvoices);
				if (!isAuthorized)
					return StatusCode(403);

				IList<PeachtreeInvoiceListViewModel> invoices = _customerOrderFacade.GetPreviousPeachTreeInvoiceList(TopResults);
				return Json(invoices);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPost, Route("export/invoice/bydate")]
		public IActionResult GetPreviousInvoicesByDate([FromBody] PeachTreeDateSearchDTO SearchParams)
		{
			try
			{
				var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PeachTreeExportInvoices);
				if (!isAuthorized)
					return StatusCode(403);

				var contents = _customerOrderFacade.GetPreviousPeachTreeInvoicesByDate(SearchParams);
				MemoryStream memoryStream = new MemoryStream((int)contents.Length);
				memoryStream.Write(contents, 0, contents.Length);
				memoryStream.Position = 0;

				var cd = $"attachment; filename = MedForce_PeachTree_Invoice_{DateTime.Today}.csv";
				Response.Headers.Add("Content-Disposition", cd);
				Response.Headers.Add("Expires", "0");
				// Flush the stream and reset the file cursor to the start
				//sw.Flush();
				//sw.BaseStream.Seek(0, SeekOrigin.Begin);
				// return the stream with Mime type
				return new FileStreamResult(memoryStream, "text/csv");
			}
			catch(Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpGet, Route("export/invoice")]
		public IActionResult ExportInvoice()
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PeachTreeExportInvoices);
			if (!isAuthorized)
				return StatusCode(403);

			var contents = _customerOrderFacade.GetPeachTreeInvoiceContents();
			MemoryStream memoryStream = new MemoryStream((int)contents.Length);
			memoryStream.Write(contents, 0, contents.Length);
			memoryStream.Position = 0;

			var cd = $"attachment; filename = MedForce_PeachTree_Invoice_{ DateTime.Today}.csv";
			Response.Headers.Add("Content-Disposition", cd);
			Response.Headers.Add("Expires", "0");
			// Flush the stream and reset the file cursor to the start
			//sw.Flush();
			//sw.BaseStream.Seek(0, SeekOrigin.Begin);
			// return the stream with Mime type
			return new FileStreamResult(memoryStream, "text/csv");

		}

		[HttpGet, Route("export/invoice/{id}")]
		public IActionResult ExportInvoiceByBatchId(int id)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PeachTreeExportInvoices);
			if (!isAuthorized)
				return StatusCode(403);

			var contents = _customerOrderFacade.GetPeachTreeInvoiceExportByBatchId(id);
			MemoryStream memoryStream = new MemoryStream((int)contents.Length);
			memoryStream.Write(contents, 0, contents.Length);
			memoryStream.Position = 0;

			var cd = $"attachment; filename = MedForce_PeachTree_Invoice_{id}.csv";
			Response.Headers.Add("Content-Disposition", cd);
			Response.Headers.Add("Expires", "0");
			// Flush the stream and reset the file cursor to the start
			//sw.Flush();
			//sw.BaseStream.Seek(0, SeekOrigin.Begin);
			// return the stream with Mime type
			return new FileStreamResult(memoryStream, "text/csv");

		}

		[HttpGet, Route("export/receipts")]
		public IActionResult ExportReceipts()
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.PeachTreeExportReceipts);
			if (!isAuthorized)
				return StatusCode(403);

			var contents = _purchaseOrderFacade.GetPeachTreeReceiptsContents();
			var sw = new StreamWriter(new MemoryStream());

			foreach (var row in contents)
			{
				sw.WriteLine(row);
			}
			var cd = $"attachment; filename = MedForce_PeachTree_Receipts_{ DateTime.Today.ToString("yyyy_MM_dd")}.csv";
			Response.Headers.Add("Content-Disposition", cd);
			Response.Headers.Add("Expires", "0");
			// Flush the stream and reset the file cursor to the start
			sw.Flush();
			sw.BaseStream.Seek(0, SeekOrigin.Begin);
			// return the stream with Mime type
			return new FileStreamResult(sw.BaseStream, "text/csv");

		}
	}
}
