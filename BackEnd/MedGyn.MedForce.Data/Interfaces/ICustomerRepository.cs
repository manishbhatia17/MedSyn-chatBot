using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Interfaces
{
	public interface ICustomerRepository
	{
		IEnumerable<Customer> GetAllCustomers(string search, string sortCol, bool sortAsc, bool seeAll, int userId, bool seeDomestic, bool seeDomesticDistribution, bool seeDomesticAfaxys, bool seeInternational);
		IEnumerable<CustomerShippingInfo> GetCustomerShippingInfo(int customerID);
		Customer GetCustomer(int customerID);
		Customer GetCustomerByEmail(string email);
		Task<Customer> GetCustomerByEmailAsync(string email);
		void UpdateCustomer(Customer customerModel);
		Customer SaveCustomer(Customer customerModel);
		void SaveCustomerShippingInfo(CustomerShippingInfo shippingInfo);
		void UpdateCustomerShippingInfo(CustomerShippingInfo shippingInfo);

        void SaveCustomerShippingInfo(List<CustomerShippingInfo> shippingInfo);
		void DeleteCustomerShippingInfo(CustomerShippingInfo shippingInfo);
		bool ValidateCustomerCustomID(int customerID, string customerCustomID);
	}
}
