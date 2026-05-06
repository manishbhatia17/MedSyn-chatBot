using System.Collections.Generic;
namespace MedGyn.MedForce.Facade.ViewModels
{
	public class ProductListViewModel : BaseListViewModel<ProductBriefViewModel>
	{
		public ProductListViewModel(SearchCriteriaViewModel sc, List<ProductBriefViewModel> results) : base(sc, results) 
		{
		
		}
	}
}