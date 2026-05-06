using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Interfaces;
using OfficeOpenXml;

namespace MedGyn.MedForce.Facade.Facades
{
	public class CustomerFacade: ICustomerFacade
	{
		private readonly ICustomerService _customerService;
		private readonly ICodeService _codeService;
		private readonly IUserService _userService;

		public CustomerFacade(
			ICustomerService customerService,
			ICodeService codeService,
			IUserService userService
		)
		{
			_customerService = customerService;
			_codeService     = codeService;
			_userService     = userService;
		}

		public async Task<bool> VerifyCustomerByEmailAsync(string email)
		{
			return await _customerService.VerifyCustomerByEmailAsync(email);
		}

		public CustomerListViewModel GetCustomerListViewModel(SearchCriteriaViewModel sc, bool seeAll, int userId, bool seeDomestic, bool seeDomesticDistribution, bool seeDomesticAfaxys, bool seeInternational)
		{
			var customers           = _customerService.GetAllCustomers(sc.Search, sc.SortColumn, sc.SortAsc, seeAll, userId, seeDomestic, seeDomesticDistribution, seeDomesticAfaxys, seeInternational);
			var countryCodes        = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var practiceTypeCodes   = _codeService.GetCodeLookupByType(CodeTypeEnum.CustomerPracticeType);
			var customerStatusCodes = _codeService.GetCodeLookupByType(CodeTypeEnum.CustomerStatus);
			var paymentTypeCodes    = _codeService.GetCodeLookupByType(CodeTypeEnum.PaymentType);

			var customerViewModels = customers.Select(c => new CustomerBriefViewModel(c, countryCodes, practiceTypeCodes, customerStatusCodes, paymentTypeCodes)).ToList();

			return new CustomerListViewModel(sc, customerViewModels);
		}

		public CustomerDetailsViewModel GetCustomerDetails(int customerID)
		{
			var statusCodes        = _codeService.GetCodesByType(CodeTypeEnum.CustomerStatus).ToList();
			var onHoldStatusCodeID = statusCodes.FirstOrDefault(x => x.CodeName == "CreditHold")?.CodeID ?? 0;
			var inactiveSatusCodeID = statusCodes.FirstOrDefault(x => x.CodeName == "Inactive")?.CodeID ?? 0;
			var repUsers = _userService.GetAllUsers()
					.Where(x => !string.IsNullOrEmpty(x.SalesRepID))
					.Select(x => new DropdownDisplayViewModel(x.UserId, $"{x.SalesRepID} {x.FullName}"))
					.ToList();

			var ret = new CustomerDetailsViewModel()
			{
				StateCodes                  = _codeService.GetCodesByType(CodeTypeEnum.States).ToDropdownList(),
				CountryCodes                = _codeService.GetCodesByType(CodeTypeEnum.Countries).ToDropdownList(),
				GLSalesNumberCodes          = _codeService.GetCodesByType(CodeTypeEnum.CustomerSalesGL).ToDropdownList(),
				GLShippingChargeNumberCodes = _codeService.GetCodesByType(CodeTypeEnum.CustomerShippingChargeGL).ToDropdownList(),
				GLReceivableNumberCodes     = _codeService.GetCodesByType(CodeTypeEnum.CustomerReceivableGL).ToDropdownList(),
				PracticeTypeCodes           = _codeService.GetCodesByType(CodeTypeEnum.CustomerPracticeType).ToDropdownList(),
				SalesTaxCodes               = _codeService.GetCodesByType(CodeTypeEnum.CustomerSalesTax).ToDropdownList(),
				PaymentTypes                = _codeService.GetCodesByType(CodeTypeEnum.PaymentType).ToDropdownList(),
				ShipCompanyCodes            = _codeService.GetCodesByType(CodeTypeEnum.ShipCompanies).ToDropdownList(),
				CustomerStatusCodes         = statusCodes.ToDropdownList(),
				RepUserIDs                  = repUsers
			};

			ret.PracticeTypeCodes.Add(new DropdownDisplayViewModel(-1, "Other"));

			if (customerID == 0)
			{
				ret.Customer = new CustomerViewModel
				{
					CountryCodeID        = ret.CountryCodes.FirstOrDefault(x => x.AltID == "USA")?.Value,
					CustomerStatusCodeID = ret.CustomerStatusCodes.FirstOrDefault(x => x.AltID == "Active")?.Value,
				};
			}
			else
			{
				var customer     = _customerService.GetCustomer(customerID);
				var shippingInfo = _customerService.GetCustomerShippingInfo(customerID, false);

				ret.Customer = new CustomerViewModel(customer)
				{
					ShippingInfo = shippingInfo.Select(x => new CustomerShippingInfoViewModel(x)).ToList(),
					OnCreditHold = customer.CustomerStatusCodeID == onHoldStatusCodeID,
					CustomerInactive = customer.CustomerStatusCodeID == inactiveSatusCodeID
				};
			}

			return ret;
		}

		public int SaveCustomer(CustomerViewModel customer)
		{
			var isValidCustomID = _customerService.ValidateCustomerCustomID(customer.CustomerID, customer.CustomerCustomID);
			if(!isValidCustomID)
				throw new Exception("Customer ID already exists, please enter a unique id.");
			var customerContract = customer.ToContract();

			var savedCustomer = _customerService.SaveCustomer(customerContract);

			//shipping is not part off form it saves on its own
			//UpdateShippingInfo(customer);

			return savedCustomer.CustomerID;
		}

		//Calls the service to save the shipping info and returns the updated list
		public List<CustomerShippingInfoViewModel> SaveCustomerShippingInfo(CustomerShippingInfoViewModel customerShippingInfo)
		{
			var shippingInfo = customerShippingInfo.ToContract();
			_customerService.SaveCustomerShippingInfo(shippingInfo);
            var shippingInfoContract = _customerService.GetCustomerShippingInfo(customerShippingInfo.CustomerID, false);
            return shippingInfoContract.Select(x => new CustomerShippingInfoViewModel(x)).ToList();
		}

		public List<CustomerShippingInfoViewModel> UpdateShippingInfo(CustomerViewModel customer)
        {
			if (customer.ShippingInfo.Any())
			{
				var shippingInfo = customer.ShippingInfo.Select(x => x.ToContract()).ToList();
				//6/7/20 jcb:set the shipping info customer id on the event of a new customer so the shipping info can still save
				if (customer.CustomerID == 0)
				{
					foreach (var loc in shippingInfo)
					{
						loc.CustomerID = customer.CustomerID;
					}
				}
				_customerService.SaveCustomerShippingInfo(shippingInfo);
			}


			var shippingInfoContract = _customerService.GetCustomerShippingInfo(customer.CustomerID, false);
			return shippingInfoContract.Select(x => new CustomerShippingInfoViewModel(x)).ToList();
		}

		public List<CustomerShippingInfoViewModel> DeleteCustomerShippingInfo(int customerID, int customerShippingInfoID)
        {
			var shippingInfo = _customerService.GetCustomerShippingInfo(customerID).ToList();

			if(shippingInfo.Any())
            {
				var shipmentToDelete = shippingInfo.FirstOrDefault(f => f.CustomerShippingInfoID == customerShippingInfoID);

				if(shipmentToDelete != default)
                {
				 _customerService.DeleteCustomerShippingInfo(shipmentToDelete);
                }
            }

			var shippingInfoContract = _customerService.GetCustomerShippingInfo(customerID, false);
			return shippingInfoContract.Select(x => new CustomerShippingInfoViewModel(x)).ToList();
		}

		public byte[] ExportListExcel(SearchCriteriaViewModel sc, bool seeAll, int userId, bool seeDomestic, bool seeDomesticDistribution, bool seeDomesticAfaxys, bool seeInternational)
		{
			var customers           = _customerService.GetAllCustomers("", sc.SortColumn, sc.SortAsc, seeAll, userId, seeDomestic, seeDomesticDistribution, seeDomesticAfaxys, seeInternational);
			var countryCodes        = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var practiceTypeCodes   = _codeService.GetCodeLookupByType(CodeTypeEnum.CustomerPracticeType);
			var customerStatusCodes = _codeService.GetCodeLookupByType(CodeTypeEnum.CustomerStatus);
			var paymentTypeCodes    = _codeService.GetCodeLookupByType(CodeTypeEnum.PaymentType);

			var customerViewModels = customers.Select(c => new CustomerBriefViewModel(c, countryCodes, practiceTypeCodes, customerStatusCodes, paymentTypeCodes)).ToList();

			var table = new System.Data.DataTable("Product List");
			var properties = new List<(string name, string label)>() {
				(nameof(CustomerBriefViewModel.CustomerName), "Name"),
				(nameof(CustomerBriefViewModel.CustomerCustomID), "Customer ID"),
				(nameof(CustomerBriefViewModel.City), "City"),
				(nameof(CustomerBriefViewModel.CountryCodeID), "Country"),
				(nameof(CustomerBriefViewModel.PracticeTypeCodeID), "Type"),
				(nameof(CustomerBriefViewModel.CustomerStatusCodeID), "Status"),
				(nameof(CustomerBriefViewModel.PaymentType), "Payment Type")
			};

			using(var excel = new ExcelPackage())
			{
				var sheet = excel.Workbook.Worksheets.Add("Customer List");
				for(var col = 1; col < properties.Count + 1; col++)
				{
					sheet.Cells[1, col].Value = properties[col - 1].label;
					sheet.Cells[1, col].Style.Font.Bold = true;
				}

				for(var row = 2; row < customerViewModels.Count + 2; row++)
				{
					for(var col = 1; col < properties.Count + 1; col++)
					{
						var value = typeof(CustomerBriefViewModel)
							.GetProperty(properties[col - 1].name)
							.GetValue(customerViewModels[row - 2]);
						if(value != null && !string.IsNullOrEmpty(value.ToString()))
							sheet.Cells[row, col].Value = value.ToString();
					}
				}
				return excel.GetAsByteArray();
			}
		}
	}
}
