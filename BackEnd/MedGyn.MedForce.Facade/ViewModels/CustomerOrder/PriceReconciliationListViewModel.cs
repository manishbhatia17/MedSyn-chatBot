using MedGyn.MedForce.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class PriceReconciliationListViewModel : BaseListViewModel<dynamic>
    {
        public PriceReconciliationListViewModel() { }


        public PriceReconciliationListViewModel(SearchCriteriaViewModel sc, IList<PriceReconciliationContract> Data):base(sc, Data)
        {
            
        }
    }
}
