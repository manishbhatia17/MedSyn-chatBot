using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Facade.ViewModels.Product;
using MedGyn.MedForce.Facade.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.ViewModels.Reports
{
    public class OnHandActivityReportViewModel 
    {
        public int BeginningTotal { get; set; }
        public int EndingTotal { get; set; }

        public List<ProductActivity> ProductActivities { get; set; }

    }

    public class OnHandActivityListViewModel : BaseListViewModel<ProductActivity>
    {
        public OnHandActivityListViewModel(SearchCriteriaViewModel sc, List<ProductActivity> results) : base(sc, results) { }
    }
}
