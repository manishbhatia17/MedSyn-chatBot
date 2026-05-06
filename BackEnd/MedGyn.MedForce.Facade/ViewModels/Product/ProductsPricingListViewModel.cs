using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class ProductsPricingListViewModel : BaseListViewModel<ProductPriceViewModel>
    {
        public ProductsPricingListViewModel(SearchCriteriaViewModel sc, List<ProductPriceViewModel> results) : base(sc, results)
        {

        }
    }
}
