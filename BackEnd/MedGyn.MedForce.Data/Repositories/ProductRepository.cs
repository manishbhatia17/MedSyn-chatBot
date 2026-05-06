using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Constants;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly IDbContext _dbContext;

		public ProductRepository(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<Product> GetAllProducts(string search, string sortCol, bool sortAsc, bool showDiscontinued, bool includeSpecialOrderOnly = false)
		{
			var queryText = "SELECT p.* FROM Product p ";
			if (!search.IsNullOrEmpty())
			{
				queryText += $@"
					LEFT JOIN Vendor pv ON pv.{nameof(Vendor.VendorID)} = p.{nameof(Product.PrimaryVendorID)}
					LEFT JOIN Vendor av1 ON av1.{nameof(Vendor.VendorID)} = p.{nameof(Product.AdditionalVendor1ID)}
					LEFT JOIN Vendor av2 ON av2.{nameof(Vendor.VendorID)} = p.{nameof(Product.AdditionalVendor2ID)}
					LEFT JOIN Vendor av3 ON av3.{nameof(Vendor.VendorID)} = p.{nameof(Product.AdditionalVendor3ID)}
					LEFT JOIN Vendor av4 ON av4.{nameof(Vendor.VendorID)} = p.{nameof(Product.AdditionalVendor4ID)}
					LEFT JOIN Vendor av5 ON av5.{nameof(Vendor.VendorID)} = p.{nameof(Product.AdditionalVendor5ID)}
					LEFT JOIN Vendor av6 ON av6.{nameof(Vendor.VendorID)} = p.{nameof(Product.AdditionalVendor6ID)}
						WHERE (p.{nameof(Product.ProductName)} LIKE :searchTerm
						OR p.{nameof(Product.ProductCustomID)} LIKE :searchTerm
						OR p.{nameof(Product.Description)} LIKE :searchTerm
						OR p.{nameof(Product.Notes)} LIKE :searchTerm
						OR p.{nameof(Product.Manufacturer)} LIKE :searchTerm
						OR pv.{nameof(Vendor.VendorName)} LIKE :searchTerm
						OR av1.{nameof(Vendor.VendorName)} LIKE :searchTerm
						OR av2.{nameof(Vendor.VendorName)} LIKE :searchTerm
						OR av3.{nameof(Vendor.VendorName)} LIKE :searchTerm
						OR av4.{nameof(Vendor.VendorName)} LIKE :searchTerm
						OR av5.{nameof(Vendor.VendorName)} LIKE :searchTerm
						OR av6.{nameof(Vendor.VendorName)} LIKE :searchTerm)
				";

				if(!showDiscontinued)
					queryText += $"AND {nameof(Product.IsDiscontinued)} = 0 ";
				if (includeSpecialOrderOnly)
					queryText += $" AND (SpecialOrderOnly = 0 or SpecialOrderOnly is null) ";
			}
			else if(!showDiscontinued)
			{
				queryText += $"WHERE {nameof(Product.IsDiscontinued)} = 0";
				if (includeSpecialOrderOnly)
					queryText += $" AND (SpecialOrderOnly = 0 or SpecialOrderOnly is null) ";
			}

			var sortProp = typeof(Product).GetProperties().FirstOrDefault(x => x.Name.ToLower() == sortCol.ToLower())?.Name;
			if (!sortProp.IsNullOrEmpty())
			{ 
				queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}"; 
			}
            else
            {
				queryText += $"ORDER BY ProductCustomId";
			}

			var query = _dbContext.Session.CreateSQLQuery(queryText).AddEntity(typeof(Product));
			if (!search.IsNullOrEmpty())
			{
				query.SetString("searchTerm", $"%{search}%");
			}
			try
			{
			return query.List<Product>();
		}
			catch (Exception ex)
            {
				throw ex;
            }
		}

		public IEnumerable<Product> GetProductsForVendor(int vendorID)
		{
			return _dbContext.Products
				.Where(p =>
					p.PrimaryVendorID     == vendorID ||
					p.AdditionalVendor1ID == vendorID ||
					p.AdditionalVendor2ID == vendorID ||
					p.AdditionalVendor3ID == vendorID ||
					p.AdditionalVendor4ID == vendorID ||
					p.AdditionalVendor5ID == vendorID ||
					p.AdditionalVendor6ID == vendorID
				);
		}

		public Product GetProduct(int productId)
		{
			var model = _dbContext.Get<Product>(productId);
			if (model == null)
			{
				return new Product();
			}
			return model;
		}

		public Product GetProductByCustomId(string customProductId)
		{
			var queryText = $@"SELECT * FROM Product p WHERE p.{nameof(Product.ProductCustomID)} = :customProductId";
	

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetString("customProductId", customProductId);

			var result = query.DynamicList().FirstOrDefault();

			if (result == null)
				return new Product();

			return new Product(result);
		}

		public Product SearchProductByName(string productName)
		{
			var queryText = $@"SELECT * FROM Product p WHERE p.{nameof(Product.ProductName)} LIKE :productName";


			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetString("productName", $"%{productName}%");

			var result = query.DynamicList().FirstOrDefault();

			if (result == null)
				return new Product();

			return new Product(result);
		}

		public async Task<Product> MergeProductAsync(Product product) {
			_dbContext.BeginTransaction();
			var model = await _dbContext.MergeAsync(product);
			await _dbContext.CommitAsync();

			return model;
		}


		public async Task<Product> SaveProductAsync(Product product)
		{
			_dbContext.BeginTransaction();
			await _dbContext.SaveAsync(product);
			await _dbContext.CommitAsync();
			return product;
		}
		public async Task<ProductInventoryAdjustment> SaveProductAdjustmentAsync(ProductInventoryAdjustment productInventoryAdjustment)
		{
			_dbContext.BeginTransaction();
			await _dbContext.SaveAsync(productInventoryAdjustment);
			await _dbContext.CommitAsync();
			return productInventoryAdjustment;
		}

		public async Task UpdateProductAsync(Product product)
		{
			_dbContext.BeginTransaction();
			await _dbContext.SaveOrUpdateAsync(product);
			await _dbContext.CommitAsync();
		}

		public IEnumerable<ProductInventoryAdjustment> GetProductInventoryAdjustments(int productID)
		{
			return _dbContext.ProductInventoryAdjustments
				.Where(p => p.ProductID == productID)
				.ToList();
		}

		public IEnumerable<dynamic> GetAllStockInfo(int? productID = null)
		{
			var queryText = $@"
				SELECT p.ProductID, AdjustedInventory, FilledCOs, CommittedCOs, RecievedPOs, OpenPOs
				FROM Product p
				LEFT JOIN
					(SELECT ProductID, SUM(Quantity) AdjustedInventory FROM ProductInventoryAdjustment
					GROUP BY ProductID) a on a.ProductID = p.ProductID
				LEFT JOIN
					(SELECT ProductID, SUM(QuantityPacked) FilledCOs FROM CustomerOrderProductFill copf
					JOIN CustomerOrderProduct cop on cop.CustomerOrderProductID = copf.CustomerOrderProductID
					GROUP BY ProductID) b ON b.ProductID = p.ProductID
				LEFT JOIN (
					SELECT 
						cop.ProductID, 
						SUM(cop.Quantity - ISNULL(filled.FilledQty, 0)) AS CommittedCOs
					FROM CustomerOrderProduct cop
					JOIN CustomerOrder co ON co.CustomerOrderID = cop.CustomerOrderID
					LEFT JOIN (
						SELECT CustomerOrderProductID, SUM(QuantityPacked) AS FilledQty
						FROM CustomerOrderProductFill
						GROUP BY CustomerOrderProductID
					) filled ON filled.CustomerOrderProductID = cop.CustomerOrderProductID
					WHERE co.IsDoNotFill = 0
					  AND co.SubmitDate IS NOT NULL
					  AND co.MGApprovedOn IS NOT NULL 
					  AND co.VPApprovedOn IS NOT NULL
					  AND (cop.Quantity - ISNULL(filled.FilledQty, 0)) > 0
					GROUP BY cop.ProductID
				) c ON c.ProductID = p.ProductID
				LEFT JOIN
					(SELECT ProductID, SUM(QuantityReceived) RecievedPOs FROM PurchaseOrderProductReceipt popr
					JOIN PurchaseOrderProduct pop on pop.PurchaseOrderProductID = popr.PurchaseOrderProductID
					GROUP BY ProductID) d ON d.ProductID = p.ProductID
				LEFT JOIN (
					SELECT 
						pop.ProductID, 
						SUM(pop.Quantity - ISNULL(received.ReceivedQty, 0)) AS OpenPOs
					FROM PurchaseOrderProduct pop
					JOIN PurchaseOrder po ON po.PurchaseOrderID = pop.PurchaseOrderID
					LEFT JOIN (
						SELECT PurchaseOrderProductID, SUM(QuantityReceived) AS ReceivedQty
						FROM PurchaseOrderProductReceipt
						GROUP BY PurchaseOrderProductID
					) received ON received.PurchaseOrderProductID = pop.PurchaseOrderProductID
					WHERE po.IsDoNotReceive = 0
					  AND po.SubmitDate IS NOT NULL
					  AND (pop.Quantity - ISNULL(received.ReceivedQty, 0)) > 0
					GROUP BY pop.ProductID
				) e ON e.ProductID = p.ProductID
			";

			if(productID.HasValue)
				queryText += "WHERE p.ProductID = :productID";

			var query = _dbContext.Session.CreateSQLQuery(queryText);

			if(productID.HasValue)
				query.SetInt32("productID", productID.Value);

			return query.DynamicList();
		}

		public bool ValidateProductCustomID(int productID, string productCustomID)
		{
			return (from p in _dbContext.Products where p.ProductCustomID == productCustomID && p.ProductID != productID select p.ProductID).Count() == 0;
		}


		public async Task<bool> SaveProductPriceAdjustmentsAsync(List<Product> products)
		{
			var productIds = products.Select(x => x.ProductID).ToArray();
			var productsInDb = _dbContext.Products.Where(p => productIds.Contains(p.ProductID)).ToList();

			_dbContext.BeginTransaction();
			foreach (var product in products)
			{
				var productInDb = productsInDb.Single(x=>x.ProductID== product.ProductID);
				if(canSave(productInDb, product))
				{
					productInDb.Cost = product.Cost ;
					productInDb.PriceDomesticList = product.PriceDomesticList ;
					productInDb.PriceDomesticDistribution = product.PriceDomesticDistribution ;
					productInDb.PriceDomesticAfaxys = product.PriceDomesticAfaxys ;
					productInDb.PriceInternationalDistribution = product.PriceInternationalDistribution ;
					productInDb.PriceDomesticPremier = product.PriceDomesticPremier ;
					productInDb.PriceMainDistributor = product.PriceMainDistributor;
					productInDb.UpdatedBy = product.UpdatedBy;
					productInDb.UpdatedOn= product.UpdatedOn;

					await _dbContext.SaveOrUpdateAsync(productInDb);
				}
			}
			await _dbContext.CommitAsync();
			return true;

			bool canSave(Product productInDb, Product product){
				return product.Cost != productInDb.Cost ||
					product.PriceDomesticList != productInDb.PriceDomesticList ||
					product.PriceDomesticDistribution != productInDb.PriceDomesticDistribution ||
					product.PriceDomesticAfaxys != productInDb.PriceDomesticAfaxys ||
					product.PriceInternationalDistribution != productInDb.PriceInternationalDistribution ||
					product.PriceDomesticPremier != productInDb.PriceDomesticPremier ||
					product.PriceMainDistributor != productInDb.PriceMainDistributor;
			}
		}

		public dynamic GetCustomerOrderProductUnitsSold(int ProductId)
		{
			string queryText = $@"SELECT 
    p.ProductID,
    copf.CustomerOrderProductFillID,
    copf.QuantityPacked,
    cop.CustomerOrderProductID,
    cop.CustomerOrderID,
    co.CustomerOrderCustomID,
    cos.CreatedOn,
	cos.CreatedBy
FROM Product p
LEFT JOIN CustomerOrderProduct cop ON cop.ProductID = p.ProductID
INNER JOIN CustomerOrderProductFill copf ON copf.CustomerOrderProductID = cop.CustomerOrderProductID
INNER JOIN CustomerOrder co ON cop.CustomerOrderID = co.CustomerOrderID
INNER JOIN CustomerOrderShipmentBox cosb on copf.CustomerOrderShipmentBoxID = cosb.CustomerOrderShipmentBoxID
INNER JOIN CustomerOrderShipment cos on cos.CustomerOrderShipmentID = cosb.CustomerOrderShipmentID 
WHERE p.ProductID = :ProductID";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetInt32("ProductID", ProductId);

			return query.DynamicList();
		}

		public dynamic GetPurchasOrderProductUnitsReceived(int ProductId)
		{
			string queryText = $@"SELECT DISTINCT ProductID, PurchaseOrderCustomID, QuantityReceived, popr.ReceiptDate, po.ReceivedBy, pop.PurchaseOrderID FROM PurchaseOrderProductReceipt popr
JOIN PurchaseOrderProduct pop on pop.PurchaseOrderProductID = popr.PurchaseOrderProductID
inner join PurchaseOrder po on po.PurchaseOrderID = pop.PurchaseOrderID
where ProductID = :ProductID
order by pop.ProductID";

            var query = _dbContext.Session.CreateSQLQuery(queryText);
            query.SetInt32("ProductID", ProductId);
            return query.DynamicList();
		}
	}
}
