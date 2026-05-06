using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface ICustomerFacade
	{
		CustomerListViewModel GetCustomerListViewModel(SearchCriteriaViewModel sc, bool seeAll, int userId, bool seeDomestic, bool seeDomesticDistribution, bool seeDomesticAfaxys, bool seeInternational);
		CustomerDetailsViewModel GetCustomerDetails(int customerID);
		int SaveCustomer(CustomerViewModel customer);
		List<CustomerShippingInfoViewModel> UpdateShippingInfo(CustomerViewModel customer);
		List<CustomerShippingInfoViewModel> DeleteCustomerShippingInfo(int customerID, int customerShippingInfoID);
		byte[] ExportListExcel(SearchCriteriaViewModel searchCriteria, bool seeAll, int userId, bool seeDomestic, bool seeDomesticDistribution, bool seeDomesticAfaxys, bool seeInternational);
		List<CustomerShippingInfoViewModel> SaveCustomerShippingInfo(CustomerShippingInfoViewModel customerShippingInfo);
		Task<bool> VerifyCustomerByEmailAsync(string email);
	}
}
