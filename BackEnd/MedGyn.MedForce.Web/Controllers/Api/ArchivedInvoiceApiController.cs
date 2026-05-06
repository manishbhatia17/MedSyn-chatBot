using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers.Api
{
    [Route("api/archivedInvoice")]
    [ApiController]
    public class ArchivedInvoiceApiController : BaseApiController
    {
		private readonly ISecurityFacade _securityFacade;
		private readonly ICustomerOrderFacade _customerOrderFacade;

		public ArchivedInvoiceApiController(ICustomerOrderFacade customerOrderFacade, ISecurityFacade securityFacade)
		{
			_customerOrderFacade = customerOrderFacade;
			_securityFacade = securityFacade;
		}

		[HttpPost, Route("")]
		public async Task<IActionResult> FetchArchivedInvoices([FromBody] SearchCriteriaViewModel searchCriteria, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
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
			var archivedInvoiceListModel = await _customerOrderFacade.GetArchivedInvoiceListViewModel(searchCriteria);
			return Json(archivedInvoiceListModel);
		}

        [HttpPost, Route("Report")]
        public async Task<IActionResult> FetchArchivedInvoicesReport([FromBody] SearchCriteriaViewModel searchCriteria, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
        {
            var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ArchiveSeeAll,
				SecurityKeyEnum.ArchiveView,};
            var isAuthorized = _securityFacade.IsAuthorized(validKeys);
            if (!isAuthorized)
                return StatusCode(403);
            var archivedInvoiceListModel = await _customerOrderFacade.GetArchivedInvoiceListViewModel(searchCriteria);
            return Json(archivedInvoiceListModel);
        }

		[HttpPost, Route("/api/Invoices/Activity")]
		public async Task<IActionResult> InvoiceActivityReport([FromBody] SearchCriteriaViewModel searchCriteria, DateTime StartDate, DateTime EndDate)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ArchiveSeeAll,
				SecurityKeyEnum.ArchiveView,};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);
			var archivedInvoiceListModel = await _customerOrderFacade.GetInvoiceActivtyViewModel(searchCriteria, StartDate, EndDate);
			archivedInvoiceListModel.Results = archivedInvoiceListModel.Results;//.FindAll(i => i.InvoiceDate.HasValue).ToList();
			return Json(archivedInvoiceListModel);
		}

		[HttpPost, Route("/api/Invoices/Activity/export/excel")]
		public async Task<IActionResult> InvoiceActivityReportExport([FromBody] SearchCriteriaViewModel searchCriteria, DateTime StartDate, DateTime EndDate)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.ArchiveSeeAll,
				SecurityKeyEnum.ArchiveView,};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if (!isAuthorized)
				return StatusCode(403);

			return File(await _customerOrderFacade.GetInvoiceActivtyExport(searchCriteria, StartDate, EndDate), "application/excel");
		}
	}
}
