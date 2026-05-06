using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository _customerRepository;

		public CustomerService(ICustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
		}

		public IEnumerable<CustomerContract> GetAllCustomers()
		{
			return GetAllCustomers("", "", true, true, 0, false, false, false, false);
		}

		public IEnumerable<CustomerContract> GetAllCustomers(string search, string sortCol, bool sortAsc, bool seeAll, int userId, bool seeDomestic, bool seeDomesticDistribution, bool seeDomesticAfaxys, bool seeInternational)
		{
			return _customerRepository.GetAllCustomers(search, sortCol, sortAsc, seeAll, userId, seeDomestic, seeDomesticDistribution, seeDomesticAfaxys, seeInternational).Select(v => new CustomerContract(v));
		}

		public IEnumerable<CustomerShippingInfoContract> GetCustomerShippingInfo(int customerID, bool showDeleted = true)
		{
			List<CustomerShippingInfo> result = _customerRepository.GetCustomerShippingInfo(customerID).ToList();
			//IsDeleted was added after the initial creation of the CustomerShippingInfo table, so we need to check for nulls
			//we want to return deleted addresses for old forms and invoices that reference the deleted address
			if(!showDeleted)
				result = result.Where(x => !x.IsDeleted.HasValue || x.IsDeleted == false).ToList();

			return result.Select(c => new CustomerShippingInfoContract(c)); 
		}

		public CustomerContract GetCustomer(int customerID)
		{
			var model = _customerRepository.GetCustomer(customerID);
			return new CustomerContract(model);
		}

		public CustomerContract GetCustomerByEmail(string email)
		{
			var model = _customerRepository.GetCustomerByEmail(email);
			return new CustomerContract(model);
		}

		public async Task<bool> VerifyCustomerByEmailAsync(string email)
		{
			var model = await _customerRepository.GetCustomerByEmailAsync(email);
			return model != null;
		}

		public CustomerContract SaveCustomer(CustomerContract customer)
		{
			var customerModel = customer.ToModel();

			if (customerModel.CustomerID > 0)
			{
				_customerRepository.UpdateCustomer(customerModel);
			}
			else
			{
				customerModel = _customerRepository.SaveCustomer(customerModel);
			}

			return new CustomerContract(customerModel);
		}

		//checks if shipping info is new or existing and calls the appropriate method
		public void SaveCustomerShippingInfo(CustomerShippingInfoContract shippingInfo)
		{
            if (shippingInfo.CustomerShippingInfoID > 0)
			{
                _customerRepository.UpdateCustomerShippingInfo(shippingInfo.ToModel());
            }
            else
			{
                _customerRepository.SaveCustomerShippingInfo(shippingInfo.ToModel());
            }
        }

		public void SaveCustomerShippingInfo(List<CustomerShippingInfoContract> shippingInfo) {
			var shippingInfoList = shippingInfo.Select(x => x.ToModel()).ToList();
			_customerRepository.SaveCustomerShippingInfo(shippingInfoList);
		}

		public void DeleteCustomerShippingInfo(CustomerShippingInfoContract shippingInfo)
        {
			_customerRepository.DeleteCustomerShippingInfo(shippingInfo.ToModel());
        }

		public bool ValidateCustomerCustomID(int customerID, string customerCustomID)
		{
			return _customerRepository.ValidateCustomerCustomID(customerID, customerCustomID);
		}
	}
}
