using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Facade.ViewModels.Reports;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface IProductFacade
	{

		//List
		ProductListViewModel GetProductListViewModel(SearchCriteriaViewModel searchCriteria, bool showDiscontinued);
		byte[] ExportProductListExcel(SearchCriteriaViewModel searchCriteria);

		//Details
		ProductDetailsViewModel GetProductDetails(int productId);
		Task<(ProductViewModel ProductViewModel, SaveResults result)> SaveProduct(ProductViewModel product);
		ProductAdjustmentDetailViewModel GetProductInventoryAdjustments(int productID);
		Task<ProductInventoryAdjustmentViewModel> SaveProductAdjustment(ProductInventoryAdjustmentViewModel productAdjustment);
        ProductsPricingListViewModel GetProductsPriceListViewModel(SearchCriteriaViewModel sc, bool showDiscontinued);
		Task<bool> SaveProductPriceAdjustmentsAsync(List<ProductPriceViewModel> productPriceViewModels);
		byte[] ExportProductPriceListExcel(SearchCriteriaViewModel sc);
		OnHandActivityReportViewModel GetOnHandActivityReport(DateTime StartDate, DateTime EndDate, int ProductId);


    }
}
