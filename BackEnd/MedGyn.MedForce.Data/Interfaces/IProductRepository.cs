using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Models;


namespace MedGyn.MedForce.Data.Interfaces
{
	public interface IProductRepository
	{
		IEnumerable<Product> GetAllProducts(string search, string sortCol, bool sortAsc, bool showDiscontinued, bool includeSpecialOrderOnly = false);
		IEnumerable<Product> GetProductsForVendor(int vendorID);
		Product GetProduct(int productId);
		Product GetProductByCustomId(string customProductId);
		Product SearchProductByName(string productName);

		Task UpdateProductAsync(Product product);
		Task<Product> SaveProductAsync(Product product);
		Task<Product> MergeProductAsync(Product product);
		IEnumerable<ProductInventoryAdjustment> GetProductInventoryAdjustments(int productID);
		Task<ProductInventoryAdjustment> SaveProductAdjustmentAsync(ProductInventoryAdjustment productInventoryAdjustment);
		IEnumerable<dynamic> GetAllStockInfo(int? productID = null);
		bool ValidateProductCustomID(int productID, string productCustomId);
		Task<bool> SaveProductPriceAdjustmentsAsync(List<Product> products);
		dynamic GetPurchasOrderProductUnitsReceived(int ProductId);
		dynamic GetCustomerOrderProductUnitsSold(int ProductId);

    }
}
