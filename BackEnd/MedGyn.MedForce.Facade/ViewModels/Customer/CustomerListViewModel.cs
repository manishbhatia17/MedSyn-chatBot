using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerListViewModel : BaseListViewModel<CustomerBriefViewModel>
	{
		public CustomerListViewModel(SearchCriteriaViewModel sc, List<CustomerBriefViewModel> results) : base(sc, results)
		{

		}
	}
}
