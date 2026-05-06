using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _productRepository;
		private readonly IAuthenticationService _authenticationService;
		public ProductService(IProductRepository productRepository, IAuthenticationService authenticationService)
		{
			_productRepository = productRepository;
			_authenticationService = authenticationService;
		}

		public IEnumerable<ProductContract> GetAllProducts(string search, string sortCol, bool sortAsc, bool showDiscontinued,bool includeSpecialOrderOnly=false)
		{
			return _productRepository.GetAllProducts(search, sortCol, sortAsc, showDiscontinued,includeSpecialOrderOnly).Select(p => new ProductContract(p));
		}

		public IEnumerable<ProductContract> GetProductsForVendor(int vendorID)
		{
			return _productRepository.GetProductsForVendor(vendorID).Select(p => new ProductContract(p)).OrderBy(o => o.ProductCustomID);
		}

		public ProductContract GetProduct(int productId)
		{
			var model = _productRepository.GetProduct(productId);

			return new ProductContract(model);
		}

		public ProductContract GetProductByCustomId(string customProductId)
		{
			var model = _productRepository.GetProductByCustomId(customProductId);

			return new ProductContract(model);
		}

		public ProductContract SearchProductByName(string productName)
		{
			var model = _productRepository.SearchProductByName(productName);

			return new ProductContract(model);
		}

		public async Task<ProductContract> SaveProduct(ProductContract product)
		{
			var productModel = product.ToModel();

			productModel.IsDeleted = false;
			productModel.UpdatedBy = _authenticationService.GetUserID();;
			productModel.UpdatedOn = DateTime.UtcNow;

			if (productModel.ProductID > 0)
			{
				await _productRepository.UpdateProductAsync(productModel);
			}
			else
			{
				productModel = await _productRepository.SaveProductAsync(productModel);
			}

			return new ProductContract(productModel);
		}

		public async Task<ProductInventoryAdjustmentContract> SaveProductAdjustment(ProductInventoryAdjustmentContract productAdjustment)
		{
			var productAdjustmentModel = productAdjustment.ToModel();

			productAdjustmentModel.AdjustedBy     = _authenticationService.GetUserID();
			productAdjustmentModel.AdjustmentDate = DateTime.UtcNow;

			productAdjustmentModel = await _productRepository.SaveProductAdjustmentAsync(productAdjustmentModel);
			return new ProductInventoryAdjustmentContract(productAdjustmentModel);
		}

		public async Task<ProductContract> MergeProductAsync(ProductContract product)
		{
			var model = await _productRepository.MergeProductAsync(product.ToModel());
			return new ProductContract(model);
		}

		public List<CustomerOrderActivityContract> GetProductCustomerOrderActiviy(int ProductId)
		{
			List<CustomerOrderActivityContract> customerOrderActivityContracts = new List<CustomerOrderActivityContract>();
			var customerOrderActivity = _productRepository.GetCustomerOrderProductUnitsSold(ProductId);

			foreach(var item in customerOrderActivity)
			{
                customerOrderActivityContracts.Add(new CustomerOrderActivityContract(item));
            }

			//ensures only distinct values are returned
			return customerOrderActivityContracts.GroupBy(x => x.CustomerOrderProductFillID).Select(x => x.FirstOrDefault()).ToList();
        }

		public List<PurchaseOrderActivityContract> GetProductPurchaseOrderReceivedActivity(int ProductId)
		{
			List<PurchaseOrderActivityContract> purchaseOrderActivityContracts = new List<PurchaseOrderActivityContract>();
			var purchaseOrderActivity = _productRepository.GetPurchasOrderProductUnitsReceived(ProductId);

			foreach (var item in purchaseOrderActivity)
			{
				   purchaseOrderActivityContracts.Add(new PurchaseOrderActivityContract(item));
			}

            return purchaseOrderActivityContracts;
        }

		public List<AdjustmentActivityContract> GetAdjustmentActivityContracts(int ProductId, Dictionary<int, CodeContract> Codes)
		{
            return _productRepository.GetProductInventoryAdjustments(ProductId).Select(x => new AdjustmentActivityContract(x, Codes)).ToList();
        }

		public IEnumerable<ProductInventoryAdjustmentContract> GetProductInventoryAdjustments(int productID)
		{
			return _productRepository.GetProductInventoryAdjustments(productID).Select(p => new ProductInventoryAdjustmentContract(p));
		}

		public IEnumerable<StockInfo> GetAllStockInfo()
		{
			return _productRepository.GetAllStockInfo().Select(x => new StockInfo(x));
		}

		public StockInfo GetProductStockInfo(int productID)
		{
			return _productRepository.GetAllStockInfo(productID).Select(x => new StockInfo(x)).FirstOrDefault();
		}

		public bool ValidateProductCustomID(ProductContract product)
		{
			return _productRepository.ValidateProductCustomID(product.ProductID, product.ProductCustomID);
		}

		public async Task<bool> SaveProductPriceAdjustmentsAsync(List<ProductContract> productContracts)
		{
			var productModels = productContracts.Select(x => x.ToModel()).ToList();
			foreach (var productModel in productModels)
			{
				productModel.UpdatedBy = _authenticationService.GetUserID(); ;
				productModel.UpdatedOn = DateTime.UtcNow;
			}
			return await _productRepository.SaveProductPriceAdjustmentsAsync(productModels);
		}
	}
}
