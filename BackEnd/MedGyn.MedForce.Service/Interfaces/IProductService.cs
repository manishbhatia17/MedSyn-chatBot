using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IProductService
	{
		IEnumerable<ProductContract> GetAllProducts(string search, string sortCol, bool sortAsc, bool showDiscontinued,bool includeSpecialOrderOnly=false);
		IEnumerable<ProductContract> GetProductsForVendor(int vendorID);
		ProductContract GetProduct(int productId);
		ProductContract GetProductByCustomId(string customProductId);
		ProductContract SearchProductByName(string productName);
			Task<ProductContract> SaveProduct(ProductContract product);
		Task<ProductContract> MergeProductAsync(ProductContract product);
		IEnumerable<ProductInventoryAdjustmentContract> GetProductInventoryAdjustments(int productID);
		Task<ProductInventoryAdjustmentContract> SaveProductAdjustment(ProductInventoryAdjustmentContract productAdjustment);
		IEnumerable<StockInfo> GetAllStockInfo();
		StockInfo GetProductStockInfo(int productID);
		bool ValidateProductCustomID(ProductContract product);
		Task<bool> SaveProductPriceAdjustmentsAsync(List<ProductContract> productContracts);
		List<CustomerOrderActivityContract> GetProductCustomerOrderActiviy(int ProductId); 
		List<PurchaseOrderActivityContract> GetProductPurchaseOrderReceivedActivity(int ProductId);
		List<AdjustmentActivityContract> GetAdjustmentActivityContracts(int ProductId, Dictionary<int, CodeContract> Codes);

    }
}
