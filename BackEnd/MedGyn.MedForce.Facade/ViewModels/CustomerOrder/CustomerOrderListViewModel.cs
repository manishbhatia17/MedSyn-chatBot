using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderListViewModel : BaseListViewModel<CustomerOrderBriefViewModel>
	{
		public CustomerOrderListViewModel(SearchCriteriaViewModel sc, List<CustomerOrderBriefViewModel> results) : base(sc, results) { }
		public decimal GrandTotal { get; set; }
		public int OnHandQty { get; set; }
	}
}
