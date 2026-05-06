using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class PurchaseOrderListViewModel : BaseListViewModel<PurchaseOrderBriefViewModel>
	{
		public PurchaseOrderListViewModel(SearchCriteriaViewModel sc, List<PurchaseOrderBriefViewModel> results) : base(sc, results) { }
	}
}
