using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class VendorListViewModel : BaseListViewModel<VendorBriefViewModel>
	{
		public VendorListViewModel(SearchCriteriaViewModel sc, List<VendorBriefViewModel> results) : base(sc, results)
		{

		}
	}
}
