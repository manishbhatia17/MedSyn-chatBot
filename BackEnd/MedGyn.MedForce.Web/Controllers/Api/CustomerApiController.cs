﻿using System.Collections.Generic;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Web.Controllers.Api
{
	[Authorize]
	[Route("api/customer")]
	public class CustomerAPIController : BaseApiController
	{
		private readonly ICustomerFacade _customerFacade;
		private readonly ISecurityFacade _securityFacade;

		public CustomerAPIController(ICustomerFacade customerFacade, ISecurityFacade securityFacade)
		{
			_customerFacade = customerFacade;
			_securityFacade = securityFacade;
		}

		[HttpPost, Route("")]
		public IActionResult FetchCustomers([FromBody] SearchCriteriaViewModel searchCriteria)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerView,
				SecurityKeyEnum.CustomerEdit,
				SecurityKeyEnum.CustomerSeeAll};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if(!isAuthorized)
				return StatusCode(403);

			var userId = _securityFacade.GetUserId();
			var seeAll = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerSeeAll);
			
			bool seeDomestic = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticEdit);
			bool seeDomesticDistribution = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticDistributionView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticDistributionEdit);
			bool seeDomesticAfaxys = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticAfaxysView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticAfaxysEdit);
			bool seeInternational = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerInternationalView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerInternationalView);
			
			var customerListModel = _customerFacade.GetCustomerListViewModel(searchCriteria, seeAll, userId, seeDomestic, seeDomesticDistribution, seeDomesticAfaxys, seeInternational);
			return Json(customerListModel);
		}

		[HttpGet, Route("{customerID}")]
		public IActionResult GetCustomerDetails(int customerID)
		{
			var validKeys = new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerView,
				SecurityKeyEnum.CustomerEdit,
				SecurityKeyEnum.CustomerSeeAll,
				SecurityKeyEnum.CustomerOrderEdit};
			var isAuthorized = _securityFacade.IsAuthorized(validKeys);
			if(!isAuthorized)
				return StatusCode(403);
			var model = _customerFacade.GetCustomerDetails(customerID);
			return Json(model);
		}

		[HttpPost, Route("save")]
		public IActionResult SaveCustomer([FromBody] CustomerViewModel customer)
        {
			try
			{ 
				var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerEdit);
				if (!isAuthorized)
					return StatusCode(403);
				var results = _customerFacade.SaveCustomer(customer);
				return Json(results);
			}
			catch(Exception ex)
            {
				return BadRequest(ex.Message);
            }
		}

		[HttpPost, Route("shippinginfo/update")]
		public IActionResult UpdateCustomerShippingInfo([FromBody] CustomerShippingInfoViewModel shippingInfo)
		{
			try
			{
				var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerEdit);
				if (!isAuthorized)
					return StatusCode(403);
				var results = _customerFacade.SaveCustomerShippingInfo(shippingInfo);
				return Json(results);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete, Route("{customerId}/shippinginfo/{customerShippingInfoId}")]
		public IActionResult DeleteCustomerShippingInf(int customerId, int customerShippingInfoId)
        {
			try
			{
				var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerEdit);
				if (!isAuthorized)
					return StatusCode(403);
				var results =  _customerFacade.DeleteCustomerShippingInfo(customerId, customerShippingInfoId);
				return Json(results);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost, Route("export/excel")]
		public IActionResult ExportListExcel([FromBody] SearchCriteriaViewModel sc)
		{
			var isAuthorized = _securityFacade.IsAuthorized(SecurityKeyEnum.ExportCustomerList);
			if (!isAuthorized)
				return StatusCode(403);

			var cd = new ContentDisposition() {
				FileName = "CustomerList.xlsx"
			};

			Response.Headers.Add("Content-Disposition", cd.ToString());
			var userID = _securityFacade.GetUserId();
			var seeAll = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerSeeAll);

			bool seeDomestic = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticEdit);
			bool seeDomesticDistribution = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticDistributionView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticDistributionEdit);
			bool seeDomesticAfaxys = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticAfaxysView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerDomesticAfaxysEdit);
			bool seeInternational = _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerInternationalView) || _securityFacade.IsAuthorized(SecurityKeyEnum.CustomerInternationalView);

			return File(_customerFacade.ExportListExcel(sc, seeAll, userID, seeDomestic, seeDomesticDistribution, seeDomesticAfaxys, seeInternational), "application/excel");
		}
	}
}
