using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface ICustomerService
	{
		IEnumerable<CustomerContract> GetAllCustomers();
		IEnumerable<CustomerContract> GetAllCustomers(string search, string sortCol, bool sortAsc, bool seeAll, int userId, bool seeDomestic, bool seeDomesticDistribution, bool seeDomesticAfaxys, bool seeInternational);
		IEnumerable<CustomerShippingInfoContract> GetCustomerShippingInfo(int customerID, bool showDeleted = true);
		CustomerContract GetCustomer(int customerId);
		CustomerContract GetCustomerByEmail(string email);
		Task<bool> VerifyCustomerByEmailAsync(string email);
		CustomerContract SaveCustomer(CustomerContract customerContract);
		void SaveCustomerShippingInfo(CustomerShippingInfoContract shippingInfo);

        void SaveCustomerShippingInfo(List<CustomerShippingInfoContract> shippingInfo);
		void DeleteCustomerShippingInfo(CustomerShippingInfoContract shippingInfo);
		bool ValidateCustomerCustomID(int customerID, string customerCustomID);
	}
}
