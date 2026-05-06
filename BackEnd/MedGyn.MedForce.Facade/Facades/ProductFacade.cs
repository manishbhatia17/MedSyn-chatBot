using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Facade.ViewModels.Product;
using MedGyn.MedForce.Facade.ViewModels.Reports;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;
using OfficeOpenXml;

namespace MedGyn.MedForce.Facade.Facades
{
	public class ProductFacade : IProductFacade
	{
		private readonly IProductService _productService;
		private readonly IVendorService _vendorService;
		private readonly ICodeService _codeService;
		private readonly IBlobStorageService _blobService;
		private readonly ICustomerOrderService _customerOrderService;
		private readonly IPurchaseOrderService _purchaseOrderService;
		private readonly IUserService _userService;

		public ProductFacade(
			IBlobStorageService blobService,
			ICodeService codeService,
			ICustomerOrderService customerOrderService,
			IProductService productService,
			IPurchaseOrderService purchaseOrderService,
			IVendorService vendorService,
			IUserService userService
		)
		{
			_blobService          = blobService;
			_codeService          = codeService;
			_customerOrderService = customerOrderService;
			_purchaseOrderService = purchaseOrderService;
			_productService       = productService;
			_vendorService        = vendorService;
			_userService          = userService;
		}

		public ProductListViewModel GetProductListViewModel(SearchCriteriaViewModel sc, bool showDiscontinued)
		{
			var products  = _productService.GetAllProducts(sc.Search, sc.SortColumn, sc.SortAsc, showDiscontinued);
			var umCodes   = _codeService.GetCodeLookupByType(CodeTypeEnum.UnitOfMeasure);
			var vendors   = _vendorService.GetAllVendors().ToDictionary(v => v.VendorID, v => v.VendorName);
			var stockInfo = _productService.GetAllStockInfo().ToDictionary(x => x.ProductID);

			var productViewModels = products.Select(p => new ProductBriefViewModel(p, umCodes, vendors, stockInfo));

			if(sc.SortColumn.ToLower() == "NetQuantity".ToLower())
			{
				productViewModels = sc.SortAsc ?
					productViewModels.OrderBy(x => x.NetQuantity) :
					productViewModels.OrderByDescending(x => x.NetQuantity);
			}

			return new ProductListViewModel(sc, productViewModels.ToList());
		}

		public ProductAdjustmentDetailViewModel GetProductInventoryAdjustments(int productID)
		{
			var product = _productService.GetProduct(productID);
			var stockInfo = _productService.GetProductStockInfo(productID);
			var adjustments = _productService.GetProductInventoryAdjustments(productID);
			var users = _userService.GetAllUsers().ToDictionary(u => u.UserId);

			var reasonCodes = _codeService.GetCodesByType(CodeTypeEnum.InventoryAdjustmentReason).ToDropdownList();
			reasonCodes.Add(new DropdownDisplayViewModel(-1, "Other"));

			return new ProductAdjustmentDetailViewModel()
			{
				ProductName     = product.ProductName,
				ProductCustomID = product.ProductCustomID,
				OnHand          = stockInfo.OnHand,
				ReasonCodes     = reasonCodes,
				HistoryList     = adjustments.Select(a => {
					users.TryGetValue(a.AdjustedBy, out var user);
					return new ProductInventoryAdjustmentViewModel(a) {
						AdjustedBy = user?.FullName,
					};
				}).ToList(),
			};
		}

		public ProductDetailsViewModel GetProductDetails(int productID)
		{
			var ret = new ProductDetailsViewModel() {
				UMCodes                = _codeService.GetCodesByType(CodeTypeEnum.UnitOfMeasure).ToDropdownList(),
				ShipWeightUnitCodes    = _codeService.GetCodesByType(CodeTypeEnum.ShipWeightUnit).ToDropdownList(),
				ShipDimensionUnitCodes = _codeService.GetCodesByType(CodeTypeEnum.ShipDimensionsUnit).ToDropdownList(),
				Vendors                = _vendorService.GetAllVendors().ToDropdownList(),
				Product                = productID == 0 ? new ProductViewModel() : GetProductViewModel(productID),
			};

			return ret;
		}

		public async Task<(ProductViewModel, SaveResults)> SaveProduct(ProductViewModel product)
		{
			// check Product Custom ID DUP
			var isValidCustomID = _productService.ValidateProductCustomID(product.ToContract());
			if(!isValidCustomID)
			{
				return (null, new SaveResults("DUP_ID"));
			}

			var list = GetImagesToUpdate(product);

			// save the product in case it's new to get the productID
			var savedProduct = await _productService.SaveProduct(product.ToContract());

			foreach(var img in list){
				var uri = await _blobService.UploadFileToBlob($"Product_{savedProduct.ProductID}_{img.property}", img.data);
				savedProduct.GetType().GetProperty(img.property).SetValue(savedProduct, $"{uri}?cb={savedProduct.UpdatedOn.Ticks}");
			}

			savedProduct = await _productService.MergeProductAsync(savedProduct);

			return (new ProductViewModel(savedProduct), new SaveResults());
		}

		public async Task<ProductInventoryAdjustmentViewModel> SaveProductAdjustment(ProductInventoryAdjustmentViewModel productAdjustment)
		{
			var savedProductAdjustment = await _productService.SaveProductAdjustment(productAdjustment.ToContract());
			return new ProductInventoryAdjustmentViewModel(savedProductAdjustment);
		}

		public OnHandActivityReportViewModel GetOnHandActivityReport(DateTime StartDate, DateTime EndDate, int ProductId)
		{
			var codes = _codeService.GetCodesByType(CodeTypeEnum.InventoryAdjustmentReason).ToDictionary(x => x.CodeID);
			List<ActivityContract> adjustmentActivity = _productService.GetAdjustmentActivityContracts(ProductId, codes).Cast<ActivityContract>().ToList();
			List<ActivityContract> coActivity = _productService.GetProductCustomerOrderActiviy(ProductId).Cast<ActivityContract>().ToList();
			List<ActivityContract> poActivity = _productService.GetProductPurchaseOrderReceivedActivity(ProductId).Cast<ActivityContract>().ToList();
			var users = _userService.GetAllUsers().ToDictionary(x => x.UserId);
			DateTime startDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
			DateTime endDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59);

			int startingAdjustment = adjustmentActivity.Where(a => a.ActivityDate < startDate).Sum(a => a.Quantity);
			int startingCO = coActivity.Where(a => a.ActivityDate < startDate).Sum(a => a.Quantity);
			int startingPO = poActivity.Where(a => a.ActivityDate < startDate).Sum(a => a.Quantity);

			List<ActivityContract> productActivity = adjustmentActivity
				.Concat(coActivity)
				.Concat(poActivity)
				.Where(w => w.ActivityDate >= startDate && w.ActivityDate <= endDate).ToList();

			int beginningActivity = startingAdjustment + startingCO + startingPO;
			int qtyActivity = productActivity.Sum(a => a.Quantity);
			return new OnHandActivityReportViewModel()
			{
                BeginningTotal = beginningActivity,
				EndingTotal = beginningActivity + qtyActivity,
                ProductActivities = productActivity.Select(s => new ProductActivity(s, users)).ToList()
            };
		}

		public byte[] ExportProductListExcel(SearchCriteriaViewModel sc)
		{
			var products  = _productService.GetAllProducts("", sc.SortColumn, sc.SortAsc, true);
			var umCodes   = _codeService.GetCodeLookupByType(CodeTypeEnum.UnitOfMeasure);
			var vendors   = _vendorService.GetAllVendors().ToDictionary(v => v.VendorID, v => v.VendorName);
			var stockInfo = _productService.GetAllStockInfo().ToDictionary(x => x.ProductID);

			var productViewModels = products.Select(p => new ProductBriefViewModel(p, umCodes, vendors, stockInfo)).ToList();

			var table = new System.Data.DataTable("Product List");
			var properties = new List<(string name, string label)>() {
				(nameof(ProductBriefViewModel.ProductName), "Name"),
				(nameof(ProductBriefViewModel.ProductCustomID), "Prod ID"),
				(nameof(ProductBriefViewModel.UnitOfMeasureCodeID), "U/M"),
				(nameof(ProductBriefViewModel.OnHandQuantity), "On Hand"),
				(nameof(ProductBriefViewModel.CommittedQuantity), "Committed"),
				(nameof(ProductBriefViewModel.POQuantity), "PO"),
				(nameof(ProductBriefViewModel.NetQuantity), "Net Qty"),
				(nameof(ProductBriefViewModel.ReorderPoint), "R/O Pnt"),
				(nameof(ProductBriefViewModel.ReorderQuantity), "R/O Qty"),
			//	(nameof(ProductBriefViewModel.PrimaryVendorID), "Pri Vendor"),
			//	(nameof(ProductBriefViewModel.Cost), "Cost"),
				(nameof(ProductBriefViewModel.IsDiscontinued), "Discontinued"),
			};

			using(var excel = new ExcelPackage())
			{
				var sheet = excel.Workbook.Worksheets.Add("Product List");
				for(var col = 1; col < properties.Count + 1; col++)
				{
					sheet.Cells[1, col].Value = properties[col - 1].label;
					sheet.Cells[1, col].Style.Font.Bold = true;
				}

				for(var row = 2; row < productViewModels.Count + 2; row++)
				{
					for(var col = 1; col < properties.Count + 1; col++)
					{
						sheet.Cells[row, col].Value = typeof(ProductBriefViewModel)
							.GetProperty(properties[col - 1].name)
							.GetValue(productViewModels[row - 2]);
					}
				}
				return excel.GetAsByteArray();
			}
		}

        public ProductsPricingListViewModel GetProductsPriceListViewModel(SearchCriteriaViewModel sc, bool showDiscontinued)
		{
			var products = _productService.GetAllProducts(sc.Search, sc.SortColumn, sc.SortAsc, showDiscontinued);
			var productsPriceViewModels = products.Select(x => new ProductPriceViewModel(x)).ToList();

            return new ProductsPricingListViewModel(sc, productsPriceViewModels);
		}

		public async Task<bool> SaveProductPriceAdjustmentsAsync(List<ProductPriceViewModel> productPriceViewModels)
		{
			return await _productService.SaveProductPriceAdjustmentsAsync(productPriceViewModels.Select(x => x.ToContract()).ToList());
		}

		public byte[] ExportProductPriceListExcel(SearchCriteriaViewModel sc)
		{
			var products = _productService.GetAllProducts(sc.Search, sc.SortColumn, sc.SortAsc, false);
			var productsPriceViewModels = products.Select(x => new ProductPriceViewModel(x)).ToList();

			var table = new System.Data.DataTable("Product Price");
			var properties = new List<(string name, string label)>() {
				(nameof(ProductPriceViewModel.ProductID), "Internal Product ID"),
				(nameof(ProductPriceViewModel.ProductCustomID), "Product ID"),
				(nameof(ProductPriceViewModel.Description), "Description"),
				(nameof(ProductPriceViewModel.PriceDomesticDistribution), "Distributor List Price"),
				(nameof(ProductPriceViewModel.PriceDomesticPremier), "Domestic Premier Price"),
				(nameof(ProductPriceViewModel.PriceDomesticAfaxys), "Domestic Afaxsys Price"),
				(nameof(ProductPriceViewModel.PriceDomesticList), "Domestic List Price"),
				(nameof(ProductPriceViewModel.PriceMainDistributor), "Main Distributor Price"),
				(nameof(ProductPriceViewModel.PriceInternationalDistribution), "International Distribution Price"),
				(nameof(ProductPriceViewModel.Cost), "Cost"),
			};

			using (var excel = new ExcelPackage())
			{ 
				var sheet = excel.Workbook.Worksheets.Add("Product Price");
				for (var col = 1; col < properties.Count + 1; col++)
				{
					sheet.Cells[1, col].Value = properties[col - 1].label;
					sheet.Cells[1, col].Style.Font.Bold = true;
				}

				for (var row = 2; row < productsPriceViewModels.Count + 2; row++)
				{
					for (var col = 1; col < properties.Count + 1; col++)
					{
						sheet.Cells[row, col].Value = typeof(ProductPriceViewModel)
							.GetProperty(properties[col - 1].name)
							.GetValue(productsPriceViewModels[row - 2]);
					}
				}
				return excel.GetAsByteArray();
			}
		}

		private ProductViewModel GetProductViewModel(int productID)
		{
				var product   = _productService.GetProduct(productID);
				var stockInfo = _productService.GetProductStockInfo(productID);
				var user      = _userService.GetUser(product.UpdatedBy);
			List<PurchaseOrderContract> openPOs = _purchaseOrderService.GetOpenPO(productID);

			var viewModel =  new ProductViewModel(product)
			{
				AdjustedInventory = stockInfo.AdjustedInventory,
				FilledCOs = stockInfo.FilledCOs,
				RecievedPOs = stockInfo.RecievedPOs,
				Committed = stockInfo.UnfilledCOs,
				PO = stockInfo.PendingPOs,
				UpdatedBy = user.FullName,
		};

			foreach(var openPO in openPOs)
			{
				string expectedDate = openPO.ExpectedDate.HasValue && openPO.ExpectedDate != DateTime.MinValue ? openPO.ExpectedDate.Value.ToString("MM/dd/yyyy") : "No expected date";
				viewModel.POExpectedDates.Add($"{openPO.Quantity} on {expectedDate}");
			}

			return viewModel;
			
		}

		private List<(string property, string data)> GetImagesToUpdate(ProductViewModel product) {
			var list = new List<(string property, string data)>();
			var regex = new Regex(@"data:image\/.*;base64");
			if (regex.IsMatch(product.PrimaryImageURI ?? ""))
			{
				list.Add(("PrimaryImageURI", product.PrimaryImageURI));
				product.PrimaryImageURI = null;
			}

			if (regex.IsMatch(product.ExtraImage1URI ?? ""))
			{
				list.Add(("ExtraImage1URI", product.ExtraImage1URI));
				product.ExtraImage1URI = null;
			}

			if (regex.IsMatch(product.ExtraImage2URI ?? ""))
			{
				list.Add(("ExtraImage2URI", product.ExtraImage2URI));
				product.ExtraImage2URI = null;
			}

			if (regex.IsMatch(product.ExtraImage3URI ?? ""))
			{
				list.Add(("ExtraImage3URI", product.ExtraImage3URI));
				product.ExtraImage3URI = null;
			}

			if (regex.IsMatch(product.ExtraImage4URI ?? ""))
			{
				list.Add(("ExtraImage4URI", product.ExtraImage4URI));
				product.ExtraImage4URI = null;
			}

			if (regex.IsMatch(product.ExtraImage5URI ?? ""))
			{
				list.Add(("ExtraImage5URI", product.ExtraImage5URI));
				product.ExtraImage5URI = null;
			}

			if (regex.IsMatch(product.ExtraImage6URI ?? ""))
			{
				list.Add(("ExtraImage6URI", product.ExtraImage6URI));
				product.ExtraImage6URI = null;
			}

			if (regex.IsMatch(product.ExtraImage7URI ?? ""))
			{
				list.Add(("ExtraImage7URI", product.ExtraImage7URI));
				product.ExtraImage7URI = null;
			}

			if (regex.IsMatch(product.ExtraImage8URI ?? ""))
			{
				list.Add(("ExtraImage8URI", product.ExtraImage8URI));
				product.ExtraImage8URI = null;
			}

			return list;
		}

	}
}
