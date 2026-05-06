using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Common.Constants;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Options;
using OpenHtmlToPdf;
using RazorLight;
using OfficeOpenXml;
using System.Globalization;
using System.IO.Compression;
using MedGyn.MedForce.Data.Models;
using System.Security.Cryptography;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using MedGyn.MedForce.Service;
using Spire.Pdf;
using CsvHelper;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Hosting.Server.Features;
using SkiaSharp;
using MedGyn.MedForce.Facade.ViewModels.CustomerOrder;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Service.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Hosting;

namespace MedGyn.MedForce.Facade.Facades
{
	public class CustomerOrderFacade : ICustomerOrderFacade
	{
		private readonly AppSettings _appSettings;
		private readonly IAuthenticationService _authenticationService;
		private readonly IBlobStorageService _blobService;
		private readonly ICodeService _codeService;
		private readonly ICustomerOrderService _customerOrderService;
		private readonly ICustomerService _customerService;
		private readonly IEmailService _emailService;
		private readonly IFedExService _fedexService;
		private readonly IProductService _productService;
		private readonly IShipStationAPIService _shipStationAPIService;
		private readonly IUPSService _upsService;
		private readonly IUserService _userService;
		private readonly IVendorService _vendorService;
		private readonly IOrderAnalysisService _orderAnalysisService;
		private readonly IPurchaseOrderService _purchaseOrderService;
		private readonly ShipStationAPISettings _shipStationAPISettings;

		//used to check environment variable for images
		private readonly IWebHostEnvironment _env;

		private const string tmLogo = "wwwroot/images/TM_Logo.png";
		private const string medGynLogo = "wwwroot/images/Logo.png";
		private const string tmEnv = "PROD-TM";

		public CustomerOrderFacade(
			IAuthenticationService authenticationService,
			IBlobStorageService blobService,
			ICodeService codeService,
			ICustomerOrderService customerOrderService,
			ICustomerService customerService,
			IEmailService emailService,
			IFedExService fedexService,
			IOptions<AppSettings> appSettings,
			IProductService productService,
			IShipStationAPIService shipStationAPIService,
			IUPSService upsService,
			IUserService userService,
			IOptions<ShipStationAPISettings> shipStationAPISettings,
			IOrderAnalysisService OrderAnalysisService,
			IPurchaseOrderService purchaseOrderService,
			IWebHostEnvironment webHostEnvironment
		)
		{
			_appSettings           = appSettings.Value;
			_authenticationService = authenticationService;
			_blobService           = blobService;
			_codeService           = codeService;
			_customerOrderService  = customerOrderService;
			_customerService       = customerService;
			_emailService          = emailService;
			_fedexService          = fedexService;
			_productService        = productService;
			_shipStationAPIService = shipStationAPIService;
			_upsService            = upsService;
			_userService           = userService;
			_shipStationAPISettings = shipStationAPISettings.Value;
			_orderAnalysisService = OrderAnalysisService;
			_purchaseOrderService = purchaseOrderService;
			_env = webHostEnvironment;
		}

		public async Task<CustomerOrderListViewModel> GetCustomerOrderListViewModel(SearchCriteriaViewModel sc, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
		{
			var showAll                     = _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderCustomersSeeAll);
			var showInternational           = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				(int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderInternationalVPApproval
			});
			var showDomesticDistributors    = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval
			});
			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticVPApproval
			});
			ValueTask<List<UserContract>> usersTask = _userService.GetAllUsersAsync();
			var results                     = await _customerOrderService.GetCustomerOrderList(sc.Search, sc.SortColumn, sc.SortAsc, status, dateOption, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);


			//checks if products were in stock, not what wnted but keeping if needed later
			//Dictionary<int,StockInfo> productStock = default;
			//if (status == CustomerOrderStatusEnum.ToBeFilled || status == CustomerOrderStatusEnum.Filling)
			//{
			//	productStock = _productService.GetAllStockInfo().ToDictionary(x => x.ProductID);

			//	for (int i = 0; i < results.Count; i++)
			//	{
			//		int onHand = 0;
			//		//var orderProducts = _customerOrderService.GetCustomerOrderFillProducts(results[i].CustomerOrderID, null);
			//		var orderProducts = results[i].ItemIDs.Split(',');
			//		bool onHandqty = false;
			//		foreach (var orderProduct in orderProducts)
			//		{
			//			if (!string.IsNullOrEmpty(orderProduct))
			//			{
			//				StockInfo stockInfo = default;
			//				if (productStock.TryGetValue(Convert.ToInt32(orderProduct), out stockInfo))
			//				{
			//					if (stockInfo.OnHand > 0)
			//					{
			//						onHandqty = true;
			//						onHand += stockInfo.OnHand;
			//						//break;
			//					}
			//				}
			//			}
			//		}

			//		results[i].OnHandQty = onHand;
			//	}
			//}

			var users = (await usersTask).ToDictionary(x => x.UserId);
			var customerOrderVMs = results.Select(x => new CustomerOrderBriefViewModel(x, users));
			if(status == CustomerOrderStatusEnum.ShowMyOrders && sc.SortColumn.ToLower() == "status"){
				customerOrderVMs = sc.SortAsc ?
					customerOrderVMs.OrderBy(x => x.Status) :
					customerOrderVMs.OrderByDescending(x => x.Status);
			}

			return new CustomerOrderListViewModel(sc, customerOrderVMs.ToList())
			{
				GrandTotal = customerOrderVMs.Sum(x => x.InvoiceTotal ?? 0)
			};
		}

		public CustomerOrderDetailsViewModel GetCustomerOrderDetails(int customerOrderID, int? customerID)
		{
			var userID = _authenticationService.GetUserID();
			var showAll = _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderCustomersSeeAll);
			var showInternational = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				(int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderInternationalVPApproval,
				(int)SecurityKeyEnum.CustomerInternationalEdit,
				(int)SecurityKeyEnum.CustomerInternationalView
			});
			var showDomesticDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
				(int)SecurityKeyEnum.CustomerDomesticDistributionEdit,
				(int)SecurityKeyEnum.CustomerDomesticDistributionView
			});
			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticVPApproval,
				(int)SecurityKeyEnum.CustomerDomesticEdit,
				(int)SecurityKeyEnum.CustomerDomesticView
			});
			var showDomesticAfaxys = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerDomesticAfaxysEdit,
				(int)SecurityKeyEnum.CustomerDomesticAfaxysView
			});


			//used in the dropdown to select a customer
			var accessibleCustomers = _customerService.GetAllCustomers("", "", true, showAll, userID, showDomesticNonDistributors, showDomesticDistributors, showDomesticAfaxys, showInternational).ToList();
			var illinoisCodeID       = _codeService.GetCodesByType(CodeTypeEnum.States).FirstOrDefault(x => x.CodeName == "IL")?.CodeID ?? 0;
			var inactiveStatusCodeID = _codeService.GetCodesByType(CodeTypeEnum.CustomerStatus).FirstOrDefault(x => x.CodeName == "Inactive")?.CodeID ?? 0;
			var exemptTaxCodeID      = _codeService.GetCodesByType(CodeTypeEnum.CustomerSalesTax).FirstOrDefault(x => x.CodeName == "Exempt")?.CodeID ?? 0;
			var paymentCodes		 = _codeService.GetCodesByType(CodeTypeEnum.PaymentType);
			var creditCardPaymentType = paymentCodes.First(f => f.CodeName == "CreditCard");

			CustomerOrderViewModel customerOrder = new CustomerOrderViewModel();

			if (customerOrderID == 0) {
				customerOrder = new CustomerOrderViewModel()
				{
					IsPartialShipAcceptable = true,
				};
				if (customerID.HasValue)
				{
					customerOrder.CustomerID = customerID.Value;
				}
			}
			else
			{
				var currentUserID = _authenticationService.GetUserID();
				var co            = _customerOrderService.GetCustomerOrder(customerOrderID, currentUserID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);
				var cops          = _customerOrderService.GetCustomerOrderProducts(customerOrderID);
				var updateUser    = _userService.GetUser(co.UpdatedBy);
				var shipUser	  = co.ShippedBy != null ? _userService.GetUser(co.ShippedBy.Value) : null;
				var fillUser     = _userService.GetUser(co.FilledBy);
				customerID = co.CustomerID;
				List<CustomerOrderProductViewModel> products = new List<CustomerOrderProductViewModel>();
				foreach(var cop in cops)
				{
					CustomerOrderProductViewModel product = new CustomerOrderProductViewModel(cop);
					List<PurchaseOrderContract> openPOs = _purchaseOrderService.GetOpenPO(product.ProductID);
					foreach (var openPO in openPOs)
					{
						string expectedDate = openPO.ExpectedDate.HasValue && openPO.ExpectedDate != DateTime.MinValue ? openPO.ExpectedDate.Value.ToString("MM/dd/yyyy") : "No expected date";
						product.POExpectedDates.Add($"{openPO.Quantity} on {expectedDate}");
					}
					products.Add(product);
				}

				customerOrder = new CustomerOrderViewModel(co)
				{
					Products  = products.OrderBy(i => i.CustomerOrderProductID).ToList(),
					UpdatedBy = updateUser != null ? updateUser.FullName : "",
					ShippedBy = shipUser != null ? shipUser.FullName : "",
					FilledBy  = fillUser != null ? fillUser.FullName : ""
				};


				if (co.MGApprovedBy.HasValue)
				{
					var approvalUser               = _userService.GetUser(co.MGApprovedBy.Value);
					customerOrder.MGApprovedBy = approvalUser != null ? approvalUser.FullName : "";
				}

				if (co.VPApprovedBy.HasValue)
				{
					var approvalUser               = _userService.GetUser(co.VPApprovedBy.Value);
					customerOrder.VPApprovedBy = approvalUser != null ? approvalUser.FullName : "";
				}
			}

			CustomerContract customer = null;

			if(customerID.HasValue)
				customer = _customerService.GetCustomer(customerOrder.CustomerID.Value);

			var ret = new CustomerOrderDetailsViewModel(_appSettings)
			{
				ShipCompanyCodes = _codeService.GetCodesByType(CodeTypeEnum.ShipCompanies).ToDropdownList(),
				FedExShipMethodCodes = _codeService.GetCodesByType(CodeTypeEnum.FedExShipMethods).ToDropdownList(),
				UPSShipMethodCodes = _codeService.GetCodesByType(CodeTypeEnum.UPSShipMethods).ToDropdownList(),
				OtherShipMethodCodes = _codeService.GetCodesByType(CodeTypeEnum.OtherShipMethods).ToDropdownList(),
				UMCodes = _codeService.GetCodesByType(CodeTypeEnum.UnitOfMeasure).ToDictionary(c => c.CodeID.ToString(), c => c.CodeDescription),
				Customers = accessibleCustomers.Select(x => new DropdownDisplayViewModel(x.CustomerID, $"{x.CustomerCustomID} {x.CustomerName}", visible: x.CustomerStatusCodeID != inactiveStatusCodeID)).ToList(),
				CustomerTaxes = accessibleCustomers.Where(x => x.SalesTaxCodeID != exemptTaxCodeID && x.StateCodeID == illinoisCodeID).ToDictionary(x => $"{x.CustomerID}", x => _appSettings.IllinoisTax),
				Products = _productService.GetAllProducts("", "", true, false, true).Select(x => new DropdownDisplayViewModel(x.ProductID, $"{x.ProductCustomID} {x.ProductName}", visible: !x.IsDiscontinued) { Data = new { IsInternationalOnly = x.InternationalOnly } }).ToList(),
				CustomerOrder = customerOrder,
				CreditCardFee = accessibleCustomers.Where(x => x.PaymentTypeCodeID == creditCardPaymentType.CodeID).ToDictionary(x => $"{x.CustomerID}", x => _appSettings.CreditCardFee)
			};

			ret.CustomerOrder.IsDomestic = customer != default ? customer.IsDomestic || customer.IsDomesticAfaxys :false;
			ret.CustomerOrder.IsDomesticDistribution = customer != default ? customer.IsDomesticDistributor : false;
			ret.CustomerOrder.IsInternational = customer != default ? customer.IsInternational : false;

			return ret;
		}

		public CustomerOrderFillViewModel GetCustomerOrderFill(int customerOrderID, int? boxID)
		{
			var cofi  = _customerOrderService.GetCustomerOrderFillInfo(customerOrderID);
			var users = _userService.GetAllUsers().ToDictionary(x => x.UserId);
			var products = _customerOrderService.GetCustomerOrderFillProducts(customerOrderID, boxID);
			var boxes = _customerOrderService.GetBoxesForFill(customerOrderID);

			var ret = new CustomerOrderFillViewModel(cofi, users)
			{
				Products = products.Select(x => new CustomerOrderProductFillViewModel(x)).ToList(),
				Boxes    = boxes,
			};

			if(boxes.Any())
				ret.FillOption = 2;

			return ret;
		}

		public PriceReconciliationListViewModel GetPriceReconciliationList(SearchCriteriaViewModel searchCriteriaViewModel, DateTime startTime, DateTime EndTime)
		{           
			var products = _customerOrderService.GetAllFillProducts(searchCriteriaViewModel.Search, searchCriteriaViewModel.SortColumn, searchCriteriaViewModel.SortAsc, startTime, EndTime);

			return new PriceReconciliationListViewModel(searchCriteriaViewModel, products);
        }

        public byte[] ExportPriceReconciliationExcel(SearchCriteriaViewModel searchCriteriaViewModel, DateTime startTime, DateTime EndTime)
        {
            var products = _customerOrderService.GetAllFillProducts(searchCriteriaViewModel.Search, searchCriteriaViewModel.SortColumn, searchCriteriaViewModel.SortAsc, startTime, EndTime);

            var priceReconciliationList = new PriceReconciliationListViewModel(searchCriteriaViewModel, products.ToList());

            var table = new System.Data.DataTable("Price Reconciliation List");


			//create a class based on the properties of the view model
			var properties = new List<(string name, string label)>() {
				( nameof(PriceReconciliationContract.CustomerOrderProductFillID), "Customer order Product Fill ID" ),
			   (nameof(PriceReconciliationContract.CustomerOrderProductID), "Customer Order Product ID"),
               (nameof(PriceReconciliationContract.QuantityPacked), "Quantity Packed"),
               (nameof(PriceReconciliationContract.SerialNumbers), "Serial Numbers"),
               (nameof(PriceReconciliationContract.CustomerOrderShipmentBoxID), "Customer Order Shipment Box ID"),
               (nameof(PriceReconciliationContract.CustomerOrderShipmentID), "Customer Order Shipment ID"),
               (nameof(PriceReconciliationContract.ProductID), "Product ID"),
			   (nameof(PriceReconciliationContract.ProductCustomID), "Product Custom ID"),
               (nameof(PriceReconciliationContract.InvoiceDate), "Invoice Date"),
			   (nameof(PriceReconciliationContract.InvoiceNumber), "Invoice Number"),
               (nameof(PriceReconciliationContract.Price), "Price"),
                    (nameof(PriceReconciliationContract.PriceDomesticList), "Domestic List Price"),
                    (nameof(PriceReconciliationContract.PriceDomesticAfaxys), "Domestic Afaxys Price"),
                    (nameof(PriceReconciliationContract.PriceInternationalDistribution), "International Distribution Price"),
					(nameof(PriceReconciliationContract.InvoiceTotal), "Total"),
					(nameof(PriceReconciliationContract.CustomerCustomID), "cust ID")

			};

            using (var excel = new ExcelPackage())
            {
                var sheet = excel.Workbook.Worksheets.Add("Price Reconciliation List");
                for (var col = 1; col < properties.Count + 1; col++)
                {
                    sheet.Cells[1, col].Value = properties[col - 1].label;
                    sheet.Cells[1, col].Style.Font.Bold = true;
                }

                for (var row = 2; row < products.Count + 2; row++)
                {
                    for (var col = 1; col < properties.Count + 1; col++)
                    {
                        var value = typeof(PriceReconciliationContract)
                            .GetProperty(properties[col - 1].name)
                            .GetValue(products[row - 2]);
                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                            sheet.Cells[row, col].Value = value.ToString();
                    }
                }
                return excel.GetAsByteArray();
            }
        }

        public CustomerOrderShipViewModel GetCustomerOrderShip(int customerOrderID, int shipmentID, int? boxID)
		{
			CustomerOrderFillInfo cofi = _customerOrderService.GetCustomerOrderFillInfo(customerOrderID);
			var shipment              = _customerOrderService.GetCustomerOrderShipment(shipmentID);
			var users                 = _userService.GetAllUsers().ToDictionary(x => x.UserId);
			var boxes                 = _customerOrderService.GetBoxesForShip(shipmentID).ToList();

			var shipLocationCompanies = _customerService.GetCustomerShippingInfo(cofi.CustomerID, false)
				.FirstOrDefault(x => x.CustomerShippingInfoID == cofi.CustomerShippingInfoID);

			var shipCompanyCodes = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipCompanies);

			

			var shipCompanies    = new List<DropdownDisplayViewModel>();
			var accountNumbers   = new Dictionary<string, string>();

			if(shipLocationCompanies.ShipCompany1CodeID.HasValue)
			{
				var company = shipCompanyCodes[shipLocationCompanies.ShipCompany1CodeID.Value];
				var dd = new DropdownDisplayViewModel(company.CodeID, company.CodeDescription, altID: company.CodeName);
				shipCompanies.Add(dd);
				accountNumbers[company.CodeID.ToString()] = shipLocationCompanies.ShipCompany1AccountNumber;
			}
			if(shipLocationCompanies.ShipCompany2CodeID.HasValue)
			{
				var company = shipCompanyCodes[shipLocationCompanies.ShipCompany2CodeID.Value];
				var dd = new DropdownDisplayViewModel(company.CodeID, company.CodeDescription, altID: company.CodeName);
				shipCompanies.Add(dd);
				accountNumbers[company.CodeID.ToString()] = shipLocationCompanies.ShipCompany2AccountNumber;
			}

			//TODO: add medgyn fed ex and ups accounts
			if(!shipCompanies.Any(a => a.Value == ShippingCarrierCodes.MedgynFedexCode))
			{
				var medgynFedex = shipCompanyCodes.Values.FirstOrDefault(x => x.CodeID == ShippingCarrierCodes.MedgynFedexCode);
				if(medgynFedex != null)
				{
					shipCompanies.Add(new DropdownDisplayViewModel(medgynFedex.CodeID, medgynFedex.CodeDescription, altID: medgynFedex.CodeName));
					accountNumbers[medgynFedex.CodeID.ToString()] = _appSettings.FedExAccountNumber;
				}
			}

			if(!shipCompanies.Any(a => a.Value == ShippingCarrierCodes.MedgynUpsCode))
			{
				var medgynUps = shipCompanyCodes.Values.FirstOrDefault(x => x.CodeID == ShippingCarrierCodes.MedgynUpsCode);
				if (medgynUps != null)
				{
					shipCompanies.Add(new DropdownDisplayViewModel(medgynUps.CodeID, medgynUps.CodeDescription, altID: medgynUps.CodeName));
					accountNumbers[medgynUps.CodeID.ToString()] = _appSettings.UPSAccountNumber;
				}
			}

			if (!boxID.HasValue)
				boxID = boxes.FirstOrDefault()?.CustomerOrderShipmentBoxID;

			var products = _customerOrderService.GetCustomerOrderFillProducts(customerOrderID, boxID);
			var box = _customerOrderService.GetCustomerOrderShipmentBox(boxID.Value);

			// prefill new boxes
			var isFormPrefilled = false;
			if(!box.Length.HasValue)
			{
				var idx = boxes.FindIndex(b => b.CustomerOrderShipmentBoxID == box.CustomerOrderShipmentBoxID);
				if(idx > 0)
				{
					var prevBox             = boxes[idx - 1];
					box.Length              = prevBox.Length;
					box.Width               = prevBox.Width;
					box.Depth               = prevBox.Depth;
					box.Weight              = prevBox.Weight;
					box.DimensionUnitCodeID = prevBox.DimensionUnitCodeID;
					box.WeightUnitCodeID    = prevBox.WeightUnitCodeID;
					isFormPrefilled         = true;
				}
			}

			var ret = new CustomerOrderShipViewModel()
			{
				Fill = new CustomerOrderFillViewModel(cofi, users)
				{
					Products = products.Select(x => new CustomerOrderProductFillViewModel(x)).ToList(),
					Boxes    = boxes.Select(x => x.CustomerOrderShipmentBoxID).ToList(),
				},
				Shipment                  = new CustomerOrderShipmentViewModel(shipment),
				ShipmentBox               = new CustomerOrderShipBoxViewModel(box) { IsFormPrefilled = isFormPrefilled },
				WeightUnitCodes           = _codeService.GetCodesByType(CodeTypeEnum.ShipWeightUnit).ToDropdownList(),
				DimensionUnitCodes        = _codeService.GetCodesByType(CodeTypeEnum.ShipDimensionsUnit).ToDropdownList(),
				ShipCompanyCodes          = shipCompanies,
				AccountNumbers            = accountNumbers,
				FedExShipMethodCodes      = _codeService.GetCodesByType(CodeTypeEnum.FedExShipMethods).ToDropdownList(),
				UPSShipMethodCodes        = _codeService.GetCodesByType(CodeTypeEnum.UPSShipMethods).ToDropdownList(),
				OtherShipMethodCodes      = _codeService.GetCodesByType(CodeTypeEnum.OtherShipMethods).ToDropdownList(),
				FedExCodeID               = _codeService.GetCodesByType(CodeTypeEnum.ShipCompanies,"FedEx").FirstOrDefault().CodeID,
				FedExMedGynCodeID         = _codeService.GetCodesByType(CodeTypeEnum.ShipCompanies,"FedEx - MedGyn").FirstOrDefault().CodeID,
				UPSCodeID                 = _codeService.GetCodesByType(CodeTypeEnum.ShipCompanies, "UPS").FirstOrDefault().CodeID,
				UPSMedGynCodeID           = _codeService.GetCodesByType(CodeTypeEnum.ShipCompanies, "UPS - MedGyn").FirstOrDefault().CodeID,
				UPSFreightCodeID          = _codeService.GetCodesByType(CodeTypeEnum.ShipCompanies, "UPS Freight").FirstOrDefault().CodeID,
				BillCountry				  = cofi.CustomerBillingCountry
			};

			// need to determine if the other boxes have been finished in order to activate Get Quote and Complete Shipment
			if (boxes.Count() > 1)
			{
				ret.OtherBoxesDone = boxes.Where(x => x.CustomerOrderShipmentBoxID != boxID).All(x => x.HasShippingInfoFilled);
			}
			else
			{
				ret.OtherBoxesDone = true;
			}

			return ret;
		}

		public async Task<OrderProductsViewModel> GetCustomerOrderHistoryForProduct(int customerID, int productID)
		{
			var product   = _productService.GetProduct(productID);
			var stockInfo = _productService.GetProductStockInfo(productID);
			var history   = await _customerOrderService.GetHistoryForProduct(customerID, productID);

			var ret = new OrderProductsViewModel()
			{
				ProductID                      = productID,
				UMCodeID                       = product.UnitOfMeasureCodeID,
				PriceDomesticList              = product.PriceDomesticList,
				PriceDomesticDistribution      = product.PriceDomesticDistribution,
				PriceDomesticAfaxys            = product.PriceDomesticAfaxys,
				PriceInternationalDistribution = product.PriceInternationalDistribution,
				StockInfo                      = stockInfo,
				COHistory                      = history.Select(x => new CustomerOrderProductsViewModelHistory(x)).ToList()
			};

			List<PurchaseOrderContract> openPOs = _purchaseOrderService.GetOpenPO(product.ProductID);
			foreach (var openPO in openPOs)
			{
				string expectedDate = openPO.ExpectedDate.HasValue && openPO.ExpectedDate != DateTime.MinValue ? openPO.ExpectedDate.Value.ToString("MM/dd/yyyy") : "No expected date";
				ret.POExpectedDates.Add($"{openPO.Quantity} on {expectedDate}");
			}



			return ret;
		}


		public async Task<BackOrderListViewModel> GetBackOrderListViewModel(SearchCriteriaViewModel sc, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
		{
			var showAll = _authenticationService.HasAnyClaim(new List<int>(){(int)SecurityKeyEnum.CustomerOrderCustomersSeeAll,
				(int)SecurityKeyEnum.BackorderSeeAll,
				(int)SecurityKeyEnum.BackorderSeeAllNoTotals});
			var showInternational = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				(int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderInternationalVPApproval
            });
			var showDomesticDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
			});

			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticVPApproval,
			});

			var results = await _customerOrderService.GetBackOrderList(sc.Search, sc.SortColumn, sc.productCustomID, sc.SortAsc, status, dateOption, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);
            var users = _userService.GetAllUsers().ToDictionary(x => x.UserId);
            var customerOrderVMs = results.Select(x => new BackOrderViewModel(x, users));
			return new BackOrderListViewModel(sc, customerOrderVMs.ToList
				());

		}

		public bool CanManagerApproveOrder(int customerOrderId)
		{
			var order = _customerOrderService.GetCustomerOrder(customerOrderId);
			var customer = _customerService.GetCustomer(order.CustomerID);
			if(customer.IsDomestic || customer.IsDomesticAfaxys)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval);
			if (customer.IsDomesticDistributor)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval);
			if (customer.IsInternational)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval);
			return false;
		}

		public bool CanVpApproveOrder(int customerOrderId)
		{
			var order = _customerOrderService.GetCustomerOrder(customerOrderId);
			var customer = _customerService.GetCustomer(order.CustomerID);
			if (customer.IsDomestic || customer.IsDomesticAfaxys)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderDomesticVPApproval);
			if (customer.IsDomesticDistributor)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval);
			if (customer.IsInternational)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderInternationalVPApproval);

			return false;
		}

		public async Task<CustomerOrderViewModel> CreateCustomerOrderFromFile(string File)
		{
            var attachment = GetCustomerOrderFile(File);
            if (!string.IsNullOrWhiteSpace(attachment))
            {
                var uri = await _blobService.UploadFileToBlob($"Customer-Order_{Guid.NewGuid().ToString()}_Attachment.pdf", attachment);
				Task<IDictionary<string, OrderAnalysisContract>> orderPropertiesTask = _orderAnalysisService.AnalizeOrder(uri);

                List<CustomerContract> customers = _customerService.GetAllCustomers().ToList();
                List<ProductContract> products = _productService.GetAllProducts("", "", false, false).ToList();
				

                IDictionary<string, OrderAnalysisContract> orderProperties = await orderPropertiesTask;

				//TODO: Get customer from orderProperties["CustomerId"].Value
                OrderAnalysisContract customerIdInfo = orderProperties["CustomerId"];
                if (customerIdInfo == null)
					throw new Exception("Customer ID not found in the order");

				


                CustomerContract customer = customers.Find(f => customerIdInfo.Value.Contains(f.CustomerCustomID));              
                List<CustomerShippingInfoContract> shippingInfoList = _customerService.GetCustomerShippingInfo(customer.CustomerID).ToList();
                string shippingAddress = orderProperties["ShippingAddress"].Value != null ? orderProperties["ShippingAddress"].Value : "";
				shippingAddress = FormatAddress(shippingAddress);


                CustomerShippingInfoContract shippingInfo = shippingInfoList.FirstOrDefault(f => shippingAddress.Contains(FormatAddress(f.Address)));

				if (shippingInfo == default)
					shippingInfo = new CustomerShippingInfoContract();

                List<CustomerOrderProductViewModel> orderProducts = new List<CustomerOrderProductViewModel>();

                if (orderProperties["Items"].Items != null && orderProperties["Items"].Items.Count > 0)
				{
					foreach (Dictionary<string, OrderAnalysisContract> row in orderProperties["Items"].Items)
					{
						string productId = row["ProductCode"].Value;

						ProductContract p = products.Find(f => f.ProductCustomID == productId);
						if (p != null)
						{
							orderProducts.Add(new CustomerOrderProductViewModel()
							{
								ProductID = p.ProductID,
								Quantity = Convert.ToInt32(row["Quantity"].Value),
								Price = row["Amount"].Value.Replace("$", ""),
								UnitOfMeasureCodeID = p.UnitOfMeasureCodeID.Value,
								ProductCustomID = p.ProductCustomID,
								ProductName = p.ProductName,
								CustomerOrderProductID = -1
							});

						}
					}
				}

                CustomerOrderViewModel newOrder = new CustomerOrderViewModel()
				{
					CustomerID = customer.CustomerID,
					AttachmentFileName = "",
					AttachmentURI = uri,
					Contact = customer.PrimaryContact,
					CustomerOrderCustomID = "",
                    CustomerShippingInfoID = shippingInfo.CustomerShippingInfoID,
                    CustomerOrderID = 0,
                    HandlingCharge = null,
                    IsPartialShipAcceptable = true,
					IsDoNotFill = false,
					Instructions = "",
					IsInternational = customer.IsInternational,
					IsDomestic = customer.IsDomestic,
					InsuranceCharge = 0,
					IsDomesticDistribution = customer.IsDomesticDistributor,
					IntermediaryShippingAddress = shippingInfo.Address,
					IntermediaryShippingContactEmail = customer.PrimaryEmail,
					IntermediaryShippingContactName = shippingInfo.Name,
					IntermediaryShippingContactNumber = customer.PrimaryPhone,
					IntermediaryShippingName = shippingInfo.Name,
					ShipChoiceCodeID = null,
					ShipCompanyType = null,
					ShippingCharge = null,
					ShippingCustomerName = shippingInfo.Name,
					SubmitDate = null,
					DoNotFillReason ="",
					MGApprovedBy = null,
					MGApprovedOn = null,
					VPApprovedBy = null,
					VPApprovedOn = null,
					Notes = "",
					PONumber = orderProperties["PurchaseOrder"].Value != null ? orderProperties["PurchaseOrder"].Value : "" ,
					Products = orderProducts,
                };

                var savedCO = _customerOrderService.SaveCustomerOrder(newOrder.ToContract(), false);
                var productsContract = orderProducts.Select(x => x.ToContract(savedCO.CustomerOrderID)).ToList();
                _customerOrderService.SaveCustomerOrderProducts(productsContract);

                return new CustomerOrderViewModel(savedCO);
            }

			return new CustomerOrderViewModel();
        }

		private string FormatAddress(string address)
		{
            return address.ToUpper().Replace('\n', ' ').Replace(".", "").ToUpper().Replace("RD", "ROAD").Replace("LN", "LANE").Replace("ST", "STREET");
        }

		public async Task<CustomerOrderViewModel> SaveCustomerOrder(CustomerOrderViewModel customerOrder, bool submit)
		{
			var contract = customerOrder.ToContract();
			var savedCO = _customerOrderService.SaveCustomerOrder(contract, submit);

			var attachment = GetCustomerOrderFile(customerOrder.AttachmentURI);
			if(!string.IsNullOrWhiteSpace(attachment)) {
				var uri = await _blobService.UploadFileToBlob($"Customer-Order_{savedCO.CustomerOrderID}_Attachment", attachment);
				savedCO.AttachmentURI =  $"{uri}?cb={savedCO.UpdatedOn.Ticks}";
				_customerOrderService.SaveCustomerOrder(savedCO, false);
			}

			var toSave = customerOrder.Products.Where(x => !x.MarkedForDelete);
			var toDelete = customerOrder.Products.Where(x => x.MarkedForDelete);
			if (toSave.Any())
			{
				var products = toSave.Select(x => x.ToContract(savedCO.CustomerOrderID)).ToList();
				_customerOrderService.SaveCustomerOrderProducts(products);
			}
			if (toDelete.Any())
			{
				var ids = toDelete.Select(x => x.CustomerOrderProductID).ToList();
				_customerOrderService.DeleteCustomerOrderProduct(ids);
			}

			//if (submit)
			//{
			//	var customer = _customerService.GetCustomer(savedCO.CustomerID);
			//	EmailApprovers(savedCO, customer, false);
			//}

			return new CustomerOrderViewModel(savedCO);
		}

		public void SetCustomerOrderToDoNotFill(int customerOrderID, DoNotFillViewModel doNotFillViewModel)
		{
			var customerOrder = _customerOrderService.GetCustomerOrder(customerOrderID);
			customerOrder.IsDoNotFill = doNotFillViewModel.doNotFill;
			customerOrder.DoNotFillReason = doNotFillViewModel.doNotFillReason;
			_customerOrderService.SaveCustomerOrder(customerOrder, false);

			var products = _customerOrderService.GetCustomerOrderFillProducts(customerOrder.CustomerOrderID, null);

			//adds to adjustment inventory what was filled so that stock is correct
			// we do notwant all of the order removed, just what was not filled if paretial orders should nto be removed from inventory
			//foreach(var product in products)
			//{
			//	if (product.QuantityAlreadyPacked != null)
			//	{
			//		//want a reason for the adjustment
			//		string reason = !string.IsNullOrEmpty(doNotFillViewModel.doNotFillReason) ? doNotFillViewModel.doNotFillReason : "DoNotFill Adjustment";

			//		//positive adjustment if we are setting to Do Not Fill, negative if we are setting to Fill
			//		int qty = doNotFillViewModel.doNotFill ? product.QuantityAlreadyPacked : product.QuantityAlreadyPacked * -1;
			//		ProductInventoryAdjustmentContract productAdjustmentActivity = new ProductInventoryAdjustmentContract()
			//		{
			//			AdjustedBy = _authenticationService.GetUserID(),
			//			AdjustmentDate = DateTime.UtcNow,
			//			ProductID = product.ProductID,
			//			Quantity = qty,
			//			ReasonCodeID = AdjustmentReasonConstants.DoNotFill,
			//			ReasonCodeOther = $"{customerOrder.CustomerOrderCustomID} - {reason}"
			//		};

			//		_productService.SaveProductAdjustment(productAdjustmentActivity).Wait();
			//	}
			//}

		}

		public SaveResults ApproveCustomerOrder(int customerOrderID, bool isVPApproval)
		{
			var results = _customerOrderService.ApproveCustomerOrder(customerOrderID, isVPApproval);
			var userID = _authenticationService.GetUserID();
			var customerOrder = _customerOrderService.GetCustomerOrder(customerOrderID, userID, true, true, true, true);
			var customer = _customerService.GetCustomer(customerOrder.CustomerID);

			//if (!isVPApproval)
			//{
			//	EmailApprovers(customerOrder, customer, true);
			//}

			return results;
		}

		public SaveResults ApproveCustomerFinancing(int customerOrderID)
		{
			var results = _customerOrderService.ApproveCustomerFinancing(customerOrderID);
			
			return results;
		}

		public bool RescindCustomerOrder(int customerOrderID)
		{
			var canRevoke = _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderRescindOrder);
			var canRescindFill = _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderFulfillmentRescind);
			return _customerOrderService.RescindCustomerOrder(customerOrderID, canRevoke, canRescindFill);
		}

		public bool DeleteCustomerOrder(int customerOrderID)
		{
			return _customerOrderService.DeleteCustomerOrder(customerOrderID);
		}

		public bool RescindFillingCustomerOrder(int customerOrderID)
		{
			return _customerOrderService.RescindFillingCustomerOrder(customerOrderID);
		}

		public bool RescindCustomerOrderShipment(int customerOrderShipmentID)
		{
			var canRescindShip = _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderShipRescind);
			return _customerOrderService.RescindCustomerOrderShipment(customerOrderShipmentID, canRescindShip);
		}

		public SaveResults FillComplete(int customerOrderID, CustomerOrderProductFillCompleteViewModel data)
		{

			var currentOrder = GetCustomerOrderFill(customerOrderID, null);
			List<CustomerOrderProductFillContract> contracts = new List<CustomerOrderProductFillContract>();
			foreach(var product in data.Products)
			{
				var item = currentOrder.Products.First(f => f.CustomerOrderProductID == product.CustomerOrderProductID);
				if(product.QuantityPacked > item.QuantityToShip)
				{
					return new SaveResults($"{product.ProductName} quantity packed cannot be greater than Quantity to Ship, please refresh the page and check the quantity again and ensure the order has not already been filled.");
				}

				contracts.Add(product.ToContract());
			}
			
				
			var res = _customerOrderService.FillComplete(customerOrderID, data.FillOption, data.NumberOfSameBoxes, data.NumberOfPackingSlips, contracts);
			if(res.Payload != null)
				res.Payload = new CustomerOrderShipmentViewModel(res.Payload as CustomerOrderShipmentContract);

			return res;
		}

		public SaveResults AddAnotherShippingBox(int customerOrderID, int? CustomerOrderShipmentID)
		{

			var box = _customerOrderService.SaveShipmentBox(customerOrderID, CustomerOrderShipmentID);
			var shipment = _customerOrderService.GetCustomerOrderShipment(CustomerOrderShipmentID.Value);
			shipment.FillOption = 2;
			SaveResults results = _customerOrderService.UpdateShipment(customerOrderID, shipment);

			if (results.Success)
			{
				results.Payload = box;
			}

			return results;
		}

		
		public SaveResults AddBox(int customerOrderID, CustomerOrderProductFillCompleteViewModel data)
		{
			var contracts = data.Products.Select(x => x.ToContract()).ToList();
			return _customerOrderService.AddBox(customerOrderID, contracts);
		}

		public SaveResults RemoveShipmentBox(int boxID)
		{
			return _customerOrderService.RemoveShipmentBox(boxID);
		}

		public SaveResults UpdateBox(int customerOrderID, int boxID, CustomerOrderProductFillCompleteViewModel data)
		{
			var contracts = data.Products.Select(x => x.ToContract()).ToList();
			return _customerOrderService.UpdateBox(customerOrderID, boxID, contracts);
		}

		public SaveResults UpdateBoxDims(int customerOrderID, int boxID, CustomerOrderShipViewModel ship)
		{
			var results = _customerOrderService.UpdateShipment(customerOrderID, ship.Shipment.ToContract());
			if(results.Success)
				return _customerOrderService.UpdateBoxDims(customerOrderID, boxID, ship.ShipmentBox.ToContract());
			return results;
		}

		public SaveResults CreateOrder(int customerOrderID, int shipmentID,CustomerOrderShipViewModel ship)
		{
			string boInvoiceNumber = "";
			string invoiceNumber = "";

			int invoiceCount = _customerOrderService.GetCustomerOrderShipmentCount(customerOrderID);
			string orgInvoiceNumber = _customerOrderService.GetCustomerOrderLastShipment(customerOrderID);
			if (orgInvoiceNumber != null && orgInvoiceNumber.Length > 0)
				boInvoiceNumber = GetBOInvoiceNumber(orgInvoiceNumber, invoiceCount);
			var results = _customerOrderService.UpdateShipment(customerOrderID, ship.Shipment.ToContract());
			if (results.Success)
				results = _customerOrderService.UpdateBoxDims(customerOrderID, ship.ShipmentBox.CustomerOrderShipmentBoxID, ship.ShipmentBox.ToContract());
			else return results;

			var invoiceTotal = GetInvoiceData(shipmentID).TotalAmount;
			if (boInvoiceNumber == "")
				invoiceNumber = $"{1089837 + shipmentID}";
			else
				invoiceNumber = boInvoiceNumber;
			var shippingComp = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipCompanies)[ship.Shipment.ShipCompanyType.Value];
			var shippingMethod = ship.Shipment.ShipMethodCodeID;

			var customerOrder = _customerOrderService.GetCustomerOrder(customerOrderID);
			string poNumber = !string.IsNullOrEmpty(customerOrder.PONumber) ? customerOrder.PONumber : customerOrder.CustomerOrderCustomID;
			var cofi = _customerOrderService.GetCustomerOrderFillInfo(customerOrderID);
			var shippingAccountId = shippingComp.CodeName == "FedEx - MedGyn" || shippingComp.CodeName == "UPS - MedGyn" ? "" : ship.Shipment.ShipAccountNumber;

			if (shippingComp.CodeName == "FedEx" || shippingComp.CodeName == "FedEx - MedGyn")
			{				
				return CreateShipmentOrder(ShippingCarrierCodes.FedExCarrierCode, shippingMethod, shippingAccountId, shipmentID, cofi.CustomerID, invoiceNumber, poNumber, invoiceTotal, ship.Shipment.MasterTrackingNumber, ship.Shipment.ToContract(), cofi);
			}
			if (shippingComp.CodeName == "UPS" || shippingComp.CodeName == "UPS - MedGyn")
			{
				return CreateShipmentOrder(ShippingCarrierCodes.UpsCarrierCode, shippingMethod, shippingAccountId, shipmentID, cofi.CustomerID, invoiceNumber, poNumber, invoiceTotal,ship.Shipment.MasterTrackingNumber, ship.Shipment.ToContract(), cofi);
			}

			//we accept 3rd party shippers!
			var res = new SaveResults();
			_customerOrderService.CompleteNonFedexNonUPSShipment(shipmentID, invoiceNumber, ship.Shipment.MasterTrackingNumber, null, invoiceTotal);
			return res;
		}


		public string GetRateQuote(int customerOrderID, int shipmentID, int shippingCompany, int shippingMethod, CustomerOrderShipBoxViewModel curBox)
		{
			var customerOrder = _customerOrderService.GetCustomerOrder(customerOrderID);
			var customer      = _customerService.GetCustomer(customerOrder.CustomerID);
			var shippingComp  = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipCompanies)[shippingCompany];

			var isMedgynAccount = shippingComp.CodeName == "FedEx - MedGyn" || shippingComp.CodeName == "UPS - MedGyn";
			var isFedex         = shippingComp.CodeName == "FedEx - MedGyn" || shippingComp.CodeName == "FedEx";
			var isUPS           = shippingComp.CodeName == "UPS - MedGyn" || shippingComp.CodeName == "UPS";

			var shippingCost = 0f;
			var handlingCost = 0f;

			// row 1 of Medgyn Shipping Logic Table chart
			if((customer.IsDomestic || customer.IsDomesticAfaxys || customer.IsDomesticDistributor) && isMedgynAccount)
			{
				//we always want to use UPS for rate, then we will calculate fed ex based on the ups rate
				var carrierCode = ShippingCarrierCodes.UpsCarrierCode;
				var shipCode = isFedex ? GetUPSCodeFromFedExCode(shippingMethod) : shippingMethod;
				shippingCost = GetRateQuote(carrierCode, customerOrderID, shipmentID, shipCode, curBox);

				handlingCost = shippingCost * 0.55f;

				if (isFedex)
				{
					//want fed ex to be $1.50 cheaper than UPS
					handlingCost = handlingCost - 1.50f;
				}

				return (shippingCost + handlingCost).ToString();
			}

			// row 2
			if((customer.IsDomestic || customer.IsDomesticAfaxys) && !isMedgynAccount)
			{
				handlingCost = 10f;
				return handlingCost.ToString();
			}

			// row 3
			if(customer.IsDomesticDistributor && !isMedgynAccount)
			{
				return "0.00";
			}

			// row 4
			if(customer.IsInternational && isMedgynAccount)
			{
				var carrierCode = isFedex ? ShippingCarrierCodes.FedExCarrierCode : ShippingCarrierCodes.UpsCarrierCode;
				shippingCost = GetRateQuote(carrierCode, customerOrderID, shipmentID, shippingMethod, curBox);
				handlingCost = shippingCost * 1.4f;

				return (shippingCost + handlingCost).ToString();
			}

			// row 5/6
			if(customer.IsInternational && !isMedgynAccount)
			{
				handlingCost = (float)GetInvoiceData(shipmentID).Subtotal * .004f;

				return handlingCost.ToString();
			}

			return "Shipping Company not implemented";
		}

		private int GetUPSCodeFromFedExCode(int shippingMethod)
		{
			switch(shippingMethod)
			{
				case 382: //Fed ex express saver
					return 473; // 3 Day Select
				case 461: //fed ex 2day
					return 471; //ups 2nd day air
				case 462: //fed ex 2day am
					return 472; //ups 2nd day air am
				case 463: //fed ex first overnight
					return 476; //ups next day air early
				case 464: //fed ex ground
					return 474; // ups ground
				case 465: //fed ex international economy
					return 481;
				case 466: // fed ex international priority
					return 478;
				case 467: //fed ex priority overnight
					return 476; //ups next day air early am
				case 468: //fed ex standard overnight
					return 475; //ups next day air
				default:
					return 474; //ground
			}
		}

		[Obsolete("Moving to Order Service")]
		public SaveResults CompleteShipment(int customerOrderID, int shipmentID, CustomerOrderShipViewModel ship)
		{
			//var shipment = _customerOrderService.GetCustomerOrderShipment(shipmentID);
			//if(!shipment.MasterTrackingNumber.IsNullOrEmpty()) {
			//	return new SaveResults("This shipment has already been completed.");
			//}
			//Cannot check above anymore because master tracking number might be provided from UI
			var boInvoiceNumber = "";
			var invoiceNumber = "";
			int invoiceCount;
			invoiceCount = _customerOrderService.GetCustomerOrderShipmentCount(customerOrderID);
			var orgInvoiceNumber = _customerOrderService.GetCustomerOrderLastShipment(customerOrderID);
			if (orgInvoiceNumber != null && orgInvoiceNumber.Length > 0)
				boInvoiceNumber = GetBOInvoiceNumber(orgInvoiceNumber, invoiceCount);
			var results = _customerOrderService.UpdateShipment(customerOrderID, ship.Shipment.ToContract());
			if(results.Success)
				results = _customerOrderService.UpdateBoxDims(customerOrderID, ship.ShipmentBox.CustomerOrderShipmentBoxID, ship.ShipmentBox.ToContract());
			else return results;
			
			var invoiceTotal = GetInvoiceData(shipmentID).TotalAmount;
			if (boInvoiceNumber == "")
				invoiceNumber = $"{1089837 + shipmentID}";
			else
				invoiceNumber = boInvoiceNumber;
			var shippingComp = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipCompanies)[ship.Shipment.ShipCompanyType.Value];
			var shippingMethod = ship.Shipment.ShipMethodCodeID;

			var customerOrder = _customerOrderService.GetCustomerOrder(customerOrderID);
			string poNumber = !string.IsNullOrEmpty(customerOrder.PONumber) ? customerOrder.PONumber : customerOrder.CustomerOrderCustomID;


			if (shippingComp.CodeName == "FedEx" || shippingComp.CodeName == "FedEx - MedGyn")
			{
				var cofi = _customerOrderService.GetCustomerOrderFillInfo(customerOrderID);
				var csi = _customerService.GetCustomerShippingInfo(cofi.CustomerID).FirstOrDefault(x => x.CustomerShippingInfoID == cofi.CustomerShippingInfoID);
				var boxes = _customerOrderService.GetBoxesForShip(shipmentID).ToList();
				var serviceCode =  _codeService.GetCodeLookupByType(CodeTypeEnum.FedExShipMethods)[shippingMethod ?? 0];
				var shippingAccountId = shippingComp.CodeName == "FedEx - MedGyn" ? "" : ship.Shipment.ShipAccountNumber;

				return _customerOrderService.CompleteShipment(csi, shipmentID, invoiceTotal, boxes,
					ShippingCarrierCodes.FedExCarrierCode, serviceCode.CodeName, invoiceNumber, shippingAccountId, ship.Shipment.MasterTrackingNumber, customerOrder.ShippingCustomerName, poNumber);
			}
			if(shippingComp.CodeName == "UPS" || shippingComp.CodeName == "UPS - MedGyn")
			{
				var cofi = _customerOrderService.GetCustomerOrderFillInfo(customerOrderID);
				var csi = _customerService.GetCustomerShippingInfo(cofi.CustomerID).FirstOrDefault(x => x.CustomerShippingInfoID == cofi.CustomerShippingInfoID);
				var boxes = _customerOrderService.GetBoxesForShip(shipmentID).ToList();
				var serviceCode = _codeService.GetCodeLookupByType(CodeTypeEnum.UPSShipMethods)[shippingMethod ?? 0];
				var shippingAccountId = shippingComp.CodeName == "UPS - MedGyn" ? "" : ship.Shipment.ShipAccountNumber;

				return _customerOrderService.CompleteShipment(csi, shipmentID, invoiceTotal, boxes,
					ShippingCarrierCodes.UpsCarrierCode, serviceCode.CodeName, invoiceNumber, shippingAccountId, ship.Shipment.MasterTrackingNumber, customerOrder.ShippingCustomerName, poNumber);
			}

			//we accept 3rd party shippers!
			var res = new SaveResults();
			_customerOrderService.CompleteNonFedexNonUPSShipment(shipmentID, invoiceNumber, ship.Shipment.MasterTrackingNumber, null, invoiceTotal);
			return res;
		}

		public SaveResults SendInvoice(int shipmentID)
		{
			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(CustomerOrderFacade))
				.UseMemoryCachingProvider()
				.Build();

			var shipment = _customerOrderService.GetCustomerOrderShipment(shipmentID);
			var co       = _customerOrderService.GetCustomerOrder(shipment.CustomerOrderID, 0, true, true, true, true);
			var c        = _customerService.GetCustomer(co.CustomerID);
			var csi      = _customerService.GetCustomerShippingInfo(co.CustomerID).FirstOrDefault(x => x.CustomerShippingInfoID == co.CustomerShippingInfoID);
			var salesRep = _userService.GetUser(csi.RepUserID);

			var model = new {
				Contact       = c.PrimaryContact,
				OrderNumber   = co.CustomerOrderCustomID,
				SalesRepName  = salesRep != null ? salesRep.FullName : "",
				SalesRepEmail = salesRep != null ? salesRep.Email : "",
			};

			var body = engine.CompileRenderAsync("Views.CustomerOrder.InvoiceEmail", model).Result;
			var data = GetInvoice(shipmentID);
			KeyValuePair<string, Stream> paymentFile = GetPaymentInstructions(c.CustomerCustomID);
			
            using (var stream = new MemoryStream(data))
			{
				var attachments = new Dictionary<string, Stream>()
				{
						{"Invoice.pdf", stream},
				};

				if(paymentFile.Key != "no-file" && paymentFile.Value != null)
				{
					attachments.Add(paymentFile.Key, paymentFile.Value);
				}
                _emailService.SendEmail(
					c.PrimaryEmail,
					$"Invoice for {co.CustomerOrderCustomID}",
					body, null,
					attachments
                );
			}

			shipment.InvoiceSent = true;
			return _customerOrderService.UpdateShipmentInvoiceStatus(shipment);
		}

		private KeyValuePair<string, Stream> GetPaymentInstructions(string CustomerId)
		{
			string fileName = "";

			if(_authenticationService.GetHostUrl().ToLower().Contains("thomasmedical"))
			{
				fileName = "Invoice Payment Instructions-Thomas Medical.pdf";

            }
			else if(CustomerId.StartsWith("3-") || CustomerId.StartsWith("4-"))
            {
                fileName = "3- and 4- Invoice Payment Instructions.pdf";
            }
            else if(CustomerId.StartsWith("1-"))
            {
                fileName = "1-Invoice Payment Instructions.pdf";
            }
			else
			{
				return new KeyValuePair<string, Stream>("no-file", null);
			}

            FileStream fs = File.OpenRead(@$"Files/{fileName}");

			return new KeyValuePair<string, Stream>(fileName, fs);
			
		}

		public byte[] GetCustomerOrderReportPdf(int customerOrderID, string documentTitle)
		{
			var html = GetCustomerOrderReportHtml(customerOrderID, documentTitle);
			var pdf = Pdf
				.From(html)
				.OfSize(PaperSize.A4)
				.Content();

			return pdf;
		}

		public byte[] GetPeachTreeInvoiceContents()
		{
			
			var invoiceItems = _customerOrderService.GetPeachTreeInvoiceContents();
			using (var ms = new MemoryStream())
			using (var writer = new StreamWriter(ms))
			{
				using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
				{
					csv.WriteRecords(invoiceItems);
				}

				return ms.ToArray();
			}
		}

		public IList<PeachtreeInvoiceListViewModel> GetPreviousPeachTreeInvoiceList(int TopResults = 10)
		{

			var invoiceItems = _customerOrderService.GetPreviousPeachTreeInvoiceList(TopResults);
			return invoiceItems.Select(s => new PeachtreeInvoiceListViewModel(s)).ToList();
			
		}

		public byte[] GetPreviousPeachTreeInvoicesByDate(PeachTreeDateSearchDTO DateSearch)
		{

			var invoiceItems = _customerOrderService.GetPreviousPeachTreeInvoicesByDate(DateSearch.StartDate, DateSearch.EndDate);
			using (var ms = new MemoryStream())
			using (var writer = new StreamWriter(ms))
			{
				using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
				{
					csv.WriteRecords(invoiceItems);
				}

				return ms.ToArray();
			}

		}

		public string GetCustomerOrderReportHtml(int customerOrderID, string documentTitle)
		{
			var countries         = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var states            = _codeService.GetCodeLookupByType(CodeTypeEnum.States);
			var illinoisCodeID    = _codeService.GetCodesByType(CodeTypeEnum.States).FirstOrDefault(x => x.CodeName == "IL")?.CodeID ?? 0;
			var exemptTaxCodeID   = _codeService.GetCodesByType(CodeTypeEnum.CustomerSalesTax).FirstOrDefault(x => x.CodeName == "Exempt")?.CodeID ?? 0;
			var umCodesLookup     = _codeService.GetCodeLookupByType(CodeTypeEnum.UnitOfMeasure);
			var shipCompanyCodes  = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipCompanies);
			var practiceTypeCodes = _codeService.GetCodeLookupByType(CodeTypeEnum.CustomerPracticeType);
			var shipMethodCodes   = _codeService.GetCodesByType(CodeTypeEnum.FedExShipMethods)
				.Concat(_codeService.GetCodesByType(CodeTypeEnum.UPSShipMethods))
				.Concat(_codeService.GetCodesByType(CodeTypeEnum.OtherShipMethods));
			var paymentCodes = _codeService.GetCodesByType(CodeTypeEnum.PaymentType);
			var creditCardPaymentType = paymentCodes.First(f => f.CodeName == "CreditCard");

			practiceTypeCodes[-1] = new CodeContract { CodeDescription = "Other" };

			var co                = _customerOrderService.GetCustomerOrder(customerOrderID, 0, true, true, true, true);
			var customer          = _customerService.GetCustomer(co.CustomerID);
			var shippingInfo      = _customerService.GetCustomerShippingInfo(customer.CustomerID).FirstOrDefault(s => s.CustomerShippingInfoID == co.CustomerShippingInfoID);
			var products          = _customerOrderService.GetCustomerOrderReportProducts(customerOrderID);


			var terms = new Dictionary<int, string>()
			{
				{1, "COD"},
				{2, "Prepay"},
				{3, $"Net Due in {customer.PaymentTermsNetDueDays} days"},
			};
			var customerTerms = "";
			terms.TryGetValue(customer.PaymentTermsType, out customerTerms);
			var rep = _userService.GetUser(shippingInfo.RepUserID);
			
			var bytes = _env.EnvironmentName == tmEnv ? File.ReadAllBytes(tmLogo) : File.ReadAllBytes(medGynLogo);
			var file = Convert.ToBase64String(bytes);

			states.TryGetValue(customer.StateCodeID ?? -1, out var customerStateCode);
			var customerState = customerStateCode?.CodeName;

			countries.TryGetValue(customer.CountryCodeID, out var customerCountryCode);
			var customerCountry = customerCountryCode?.CodeDescription;

			states.TryGetValue(shippingInfo.StateCodeID ?? -1, out var customerShipStateCode);
			var customerShipState = customerShipStateCode?.CodeName;

			countries.TryGetValue(shippingInfo.CountryCodeID, out var customerShipCountryCode);
			var customerShipCountry = customerShipCountryCode?.CodeDescription;

			var subTotal                  = products.Sum(p => p.Ext);
			var shippingCharge            = (co.ShippingCharge ?? 0) + (co.InsuranceCharge ?? 0) + (co.HandlingCharge ?? 0);
			var taxes                     = ((customer.SalesTaxCodeID == exemptTaxCodeID || customer.StateCodeID != illinoisCodeID) ? 0 : _appSettings.IllinoisTax)/100 * subTotal;
			var creditCardFee             = (customer.PaymentTypeCodeID == creditCardPaymentType.CodeID) ? _appSettings.CreditCardFee/100 * (subTotal +shippingCharge + taxes) : 0;
			var totalAmount               = subTotal + shippingCharge + taxes + creditCardFee;

			var model = new {
				Title                     = documentTitle,
				LogoString                = file,
				MedGynAddress             = _appSettings.MedGynAddress,
				MedGynCity                = _appSettings.MedGynCity,
				MedGynStateOrProvinceCode = _appSettings.MedGynStateOrProvinceCode,
				MedGynPostalCode          = _appSettings.MedGynPostalCode,
				MedGynContactPhone        = Regex.Replace(_appSettings.MedGynContactPhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
				MedGynFax                 = Regex.Replace(_appSettings.MedGynFax, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
				MedGynTollFreePhone       = Regex.Replace(_appSettings.MedGynTollFreePhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
				MedGynEmail               = _appSettings.MedGynEmail,
				RemittanceAddress         = _appSettings.PackingSlipRemittanceAddress,
				CustomerOrderNumber       = co.CustomerOrderCustomID,
				CustomerOrderDate         = co.SubmitDate?.ToString("MM/dd/yyyy"),
				CustomerID                = customer.CustomerCustomID,
				CustomerName              = customer.CustomerName,
				CustomerAddress           = customer.Address1,
				CustomerAddress2          = customer.Address2,
				CustomerCity              = customer.City,
				CustomerState             = customerState,
				CustomerCountry           = customerCountry,
				CustomerZip               = customer.ZipCode,
				CustomerShipName          = shippingInfo.Name,
				CustomerShipAddress       = shippingInfo.Address,
				CustomerShipAddress2      = shippingInfo.Address2,
				CustomerShipCity          = shippingInfo.City,
				CustomerShipState         = customerShipState,
				CustomerShipCountry       = customerShipCountry,
				CustomerShipZip           = shippingInfo.ZipCode,
				SalesRep                  = $"{rep?.SalesRepID} {rep?.FullName}",
				ShipLocation              = shippingInfo?.Name,
				CustomerPONumber          = co.PONumber,
				Contact                   = co.Contact,
				ShipCompany               = shipCompanyCodes[co.ShipCompanyType ?? 0]?.CodeName,
				ShipMethod                = shipMethodCodes.FirstOrDefault(c => c.CodeID == co.ShipChoiceCodeID)?.CodeDescription,
				ShipAccount               = shippingInfo?.ShipCompany1AccountNumber,
				IsPartialShipAcceptable   = co.IsPartialShipAcceptable,
				ISName                    = co.IntermediaryShippingName,
				ISAddress                 = co.IntermediaryShippingAddress,
				ISContactName             = co.IntermediaryShippingContactName,
				ISContactNumber           = co.IntermediaryShippingContactNumber,
				ISContactEmail            = co.IntermediaryShippingContactEmail,
				IsInternational           = customer.IsInternational,
				Instructions              = co.Instructions,
				Products                  = products,
				TotalItems                = products.Sum(p => p.Quantity),
				Subtotal                  = subTotal.ToString("0.00"),
				ShippingCharge            = shippingCharge.ToString("0.00"),
				Taxes                     = taxes.ToString("0.00"),
				TotalAmount               = totalAmount.ToString("0.00"),
				CreditCardFee			 = creditCardFee.ToString("0.00"),
				ShippingCustomerName      = String.IsNullOrWhiteSpace(co.ShippingCustomerName) ? customer.CustomerName : co.ShippingCustomerName,
			};

			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(PurchaseOrderDetailsViewModel))
				.UseMemoryCachingProvider()
				.Build();

			return engine.CompileRenderAsync("Views.CustomerOrder.CustomerOrderReport", model).Result;
		}

		public byte[] GetInvoice(int shipmentID) {
			var html = GetInvoiceHtml(shipmentID);
			var pdf = Pdf.From(html).OfSize(PaperSize.A4).Content();
			return pdf;
		}

		public string GetInvoiceHtml(int shipmentID)
		{
			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(CustomerOrderFacade))
				.UseMemoryCachingProvider()
				.Build();

			var model = GetInvoiceData(shipmentID);
			return engine.CompileRenderAsync("Views.CustomerOrder.Invoice", model).Result;
		}

		public byte[] GetPackingSlipPdf(int shipmentID, bool? generateMultiplePackingSlip) {
			var html = GetPackingSlipHtml(shipmentID, generateMultiplePackingSlip);
			var pdf = Pdf.From(html).OfSize(PaperSize.A4).Content();
			return pdf;
		}

		public string GetPackingSlipHtml(int shipmentID, bool? generateMultiplePackingSlip) {
			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(CustomerOrderFacade))
				.UseMemoryCachingProvider()
				.Build();

			var bytes = _env.EnvironmentName == tmEnv ? File.ReadAllBytes(tmLogo) : File.ReadAllBytes(medGynLogo);
			var file = Convert.ToBase64String(bytes);

			var countries         = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var exemptTaxCodeID   = _codeService.GetCodesByType(CodeTypeEnum.CustomerSalesTax).FirstOrDefault(x => x.CodeName == "Exempt")?.CodeID ?? 0;
			var states            = _codeService.GetCodeLookupByType(CodeTypeEnum.States);
			var shipCompanies     = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipCompanies);
			var shipMethods       = _codeService.GetCodesByType(CodeTypeEnum.FedExShipMethods)
				.Concat(_codeService.GetCodesByType(CodeTypeEnum.UPSShipMethods))
				.Concat(_codeService.GetCodesByType(CodeTypeEnum.OtherShipMethods))
				.ToDictionary(x => x.CodeID);

			var shipment = _customerOrderService.GetCustomerOrderShipment(shipmentID);
			var co       = _customerOrderService.GetCustomerOrder(shipment.CustomerOrderID, 0, true, true, true, true);
			var coProds  = _customerOrderService.GetCustomerOrderProducts(co.CustomerOrderID).ToDictionary(x => x.ProductID);
			var ips      = _customerOrderService.GetInvoiceProducts(shipment.CustomerOrderShipmentID);
			var boxes    = _customerOrderService.GetBoxesForShip(shipmentID);
			var c        = _customerService.GetCustomer(co.CustomerID);
			var csi      = _customerService.GetCustomerShippingInfo(co.CustomerID).FirstOrDefault(x => x.CustomerShippingInfoID == co.CustomerShippingInfoID);
			var salesRep = _userService.GetUser(csi.RepUserID);

			var packingSlips = new List<PackingSlipViewModel>();
			foreach(var box in boxes)
			{
				var fills = _customerOrderService.GetCustomerOrderFillProducts(co.CustomerOrderID, null)
					.ToDictionary(x => (int)x.ProductID, x => (int?)x.QuantityAlreadyPacked);

				var boxProducts = ips.Where(x => x.CustomerOrderShipmentBoxID == box.CustomerOrderShipmentBoxID).GroupBy(x => x.ProductID);
				var products = new List<InvoiceProduct>();
				foreach(var bp in boxProducts)
				{
					var curIP = bp.First();
					fills.TryGetValue(curIP.ProductID, out var quantityAlreadyPacked);

					var newIP = new InvoiceProduct()
						{
							ProductID         = curIP.ProductID,
							ProductCustomID   = curIP.ProductCustomID,
							ProductName       = curIP.ProductName,
							Quantity          = curIP.Quantity,
							BackorderQuantity = curIP.Quantity - (quantityAlreadyPacked ?? 0),
							QuantityPacked    = bp.Sum(x => x.QuantityPacked),
							SerialNumbers     = string.Join(",", bp.Select(x => x.SerialNumbers)),
							Price             = coProds[curIP.ProductID].Price,
						};

					if(newIP.BackorderQuantity == 0)
						newIP.BackorderQuantity = null;

					products.Add(newIP);
				}

				var paymentTerms = c.PaymentTermsType == 1 ? "COD"
					: c.PaymentTermsType == 2 ? "Prepay"
					: $"Net Due in {c.PaymentTermsNetDueDays} days";

				var subtotal = products.Sum(x => x.Ext);
				var totalAmount = subtotal + (shipment.ShippingCharge ?? 0);
				var taxes = 0m;

				if(c.SalesTaxCodeID != exemptTaxCodeID && states[csi.StateCodeID ?? 0]?.CodeName == "IL")
					taxes = Decimal.Round(totalAmount * (_appSettings.IllinoisTax / 100), 2);

				totalAmount += taxes;

				states.TryGetValue(c.StateCodeID ?? -1, out var customerStateCode);
				var customerState = customerStateCode?.CodeName;

				countries.TryGetValue(c.CountryCodeID, out var customerCountryCode);
				var customerCountry = customerCountryCode?.CodeDescription;

				states.TryGetValue(csi.StateCodeID ?? -1, out var shipStateCode);
				var customerShipState = shipStateCode?.CodeName;

				countries.TryGetValue(csi.CountryCodeID, out var customerShipCountryCode);
				var customerShipCountry = customerShipCountryCode?.CodeDescription;

				shipMethods.TryGetValue(shipment.ShipMethodCodeID ?? -1, out var shipMethodCode);
				var shipMethod = shipMethodCode?.CodeDescription;              

				for (var i = 0; i < (shipment.NumberOfPackingSlips ?? 1); i++)
				{
					packingSlips.Add(new PackingSlipViewModel
					{
						CustomerName         = c.CustomerName,
						CustomerAddress      = c.Address1,
						CustomerAddress2     = c.Address2,
						CustomerCity         = c.City,
						CustomerState        = customerState,
						CustomerCountry      = customerCountry,
						CustomerZip          = c.ZipCode,
						CustomerShipName     = csi.Name,
						CustomerShipAddress  = csi.Address,
						CustomerShipAddress2 = csi.Address2,
						CustomerShipCity     = csi.City,
						CustomerShipState    = customerShipState,
						CustomerShipCountry  = customerShipCountry,
						CustomerShipZip      = csi.ZipCode,
						CustomerOrderNumber  = co.CustomerOrderCustomID,
						CustomerOrderDate    = co.SubmitDate.Value.ToString("MM/dd/yyyy"),
						SalesRep             = $"{salesRep.SalesRepID} {salesRep.FullName}",
						ShipCompany          = shipCompanies[shipment.ShipCompanyType ?? 0]?.CodeDescription,
						ShipMethod           = shipMethod,
						IsPartialShip        = co.IsPartialShipAcceptable ? "Yes" : "No",
						CustomerID           = c.CustomerCustomID,
						CustomerPONumber     = co.PONumber,
						PaymentTerms         = paymentTerms,
						TrackingNumber       = shipment.MasterTrackingNumber,
						Instructions         = co.Instructions,
						Subtotal             = subtotal,
						Taxes                = taxes,
						ShippingCharge       = shipment.ShippingCharge,
						TotalAmount          = totalAmount,
						Products             = products,
						ShippingCustomerName = String.IsNullOrWhiteSpace(co.ShippingCustomerName) ? c.CustomerName : co.ShippingCustomerName,
					});
				}
			}

			List<PackingSlipViewModel> singlePackingSlipViewModels = new List<PackingSlipViewModel>();
			PackingSlipViewModel updatedModel = new PackingSlipViewModel();
			if (generateMultiplePackingSlip != null && generateMultiplePackingSlip == false)
			{							
				updatedModel = new PackingSlipViewModel
				{
					CustomerName = c.CustomerName,
					CustomerAddress = c.Address1,
					CustomerAddress2 = c.Address2,
					CustomerCity = c.City,
					CustomerState = packingSlips[0].CustomerState,
					CustomerCountry = packingSlips[0].CustomerCountry,
					CustomerZip = c.ZipCode,
					CustomerShipName = csi.Name,
					CustomerShipAddress = csi.Address,
					CustomerShipAddress2 = csi.Address2,
					CustomerShipCity = csi.City,
					CustomerShipState = packingSlips[0].CustomerShipState,
					CustomerShipCountry = packingSlips[0].CustomerShipCountry,
					CustomerShipZip = csi.ZipCode,
					CustomerOrderNumber = co.CustomerOrderCustomID,
					CustomerOrderDate = co.SubmitDate.Value.ToString("MM/dd/yyyy"),
					SalesRep = $"{salesRep.SalesRepID} {salesRep.FullName}",
					ShipCompany = shipCompanies[shipment.ShipCompanyType ?? 0]?.CodeDescription,
					ShipMethod = packingSlips[0].ShipMethod,
					IsPartialShip = co.IsPartialShipAcceptable ? "Yes" : "No",
					CustomerID = c.CustomerCustomID,
					CustomerPONumber = co.PONumber,
					PaymentTerms = packingSlips[0].PaymentTerms,
					TrackingNumber = shipment.MasterTrackingNumber,
					Instructions = co.Instructions,
					Subtotal = packingSlips[0].Subtotal,
					Taxes = packingSlips[0].Taxes,
					ShippingCharge = shipment.ShippingCharge,
					TotalAmount = packingSlips[0].TotalAmount,
					Products = packingSlips[0].Products,
					ShippingCustomerName = String.IsNullOrWhiteSpace(co.ShippingCustomerName) ? c.CustomerName : co.ShippingCustomerName,
				};

				List<int> uniqueProducts = new List<int>();				

				foreach (PackingSlipViewModel pslip in packingSlips)
                {
					foreach(InvoiceProduct ip in pslip.Products)
                    {
						if(!uniqueProducts.Contains(ip.ProductID))
						uniqueProducts.Add(ip.ProductID);
                    }
                }

				List<InvoiceProduct> uniqueInvoiceProducts = new List<InvoiceProduct>();

				foreach(int uniqueProductId in uniqueProducts)
                {
					string serialNumbers = "";
					int totalQuantity = 0;
					int totalQuantityPacked = 0;
					string productCustomId = "";
					int customerOrderShipmentBoxID = 0;
					string productName = "";
					decimal price = 0;
					foreach (PackingSlipViewModel pslip in packingSlips)
					{
						foreach (InvoiceProduct ip in pslip.Products)
						{
                            if (ip.ProductID == uniqueProductId)
                            {
								serialNumbers = serialNumbers + ip.SerialNumbers + ",";								
								totalQuantityPacked = totalQuantityPacked + ip.QuantityPacked;
								if (totalQuantity == 0) { totalQuantity = ip.Quantity; }
								if (productCustomId == "") { productCustomId = ip.ProductCustomID; }
								if (customerOrderShipmentBoxID == 0) { customerOrderShipmentBoxID = ip.CustomerOrderShipmentBoxID; }
								if (productName == "") { productName = ip.ProductName; }
								if (price == 0) { price = ip.Price; }
							}
						}
					}

					uniqueInvoiceProducts.Add(new InvoiceProduct
					{
						ProductID = uniqueProductId,
						ProductCustomID = productCustomId,
						CustomerOrderShipmentBoxID = customerOrderShipmentBoxID,
						ProductName = productName,
						Quantity = totalQuantity,
						QuantityPacked = totalQuantityPacked,
						SerialNumbers = serialNumbers.Substring(0, serialNumbers.Length - 1),
						Price = price
					});
				}

				updatedModel.Products = uniqueInvoiceProducts;
			}
			singlePackingSlipViewModels.Add(updatedModel);

			if (generateMultiplePackingSlip != null && generateMultiplePackingSlip == false)
            {
				return engine.CompileRenderAsync("Views.CustomerOrder.PackingSlip", new
				{
					PackingSlips = singlePackingSlipViewModels,
					LogoString = file,
					MedGynAddress = _appSettings.MedGynAddress,
					MedGynCity = _appSettings.MedGynCity,
					MedGynStateOrProvinceCode = _appSettings.MedGynStateOrProvinceCode,
					MedGynPostalCode = _appSettings.MedGynPostalCode,
					MedGynContactPhone = Regex.Replace(_appSettings.MedGynContactPhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
					MedGynFax = Regex.Replace(_appSettings.MedGynFax, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
					MedGynTollFreePhone = Regex.Replace(_appSettings.MedGynTollFreePhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
					MedGynEmail = _appSettings.MedGynEmail,
					RemittanceAddress = _appSettings.PackingSlipRemittanceAddress,
					RemittanceMessage = _appSettings.PackingSlipRemittanceMessage,
				}).Result;
			}
            else
            {
				return engine.CompileRenderAsync("Views.CustomerOrder.PackingSlip", new
				{
					PackingSlips = packingSlips,
					LogoString = file,
					MedGynAddress = _appSettings.MedGynAddress,
					MedGynCity = _appSettings.MedGynCity,
					MedGynStateOrProvinceCode = _appSettings.MedGynStateOrProvinceCode,
					MedGynPostalCode = _appSettings.MedGynPostalCode,
					MedGynContactPhone = Regex.Replace(_appSettings.MedGynContactPhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
					MedGynFax = Regex.Replace(_appSettings.MedGynFax, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
					MedGynTollFreePhone = Regex.Replace(_appSettings.MedGynTollFreePhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
					MedGynEmail = _appSettings.MedGynEmail,
					RemittanceAddress = _appSettings.PackingSlipRemittanceAddress,
					RemittanceMessage = _appSettings.PackingSlipRemittanceMessage,
				}).Result;
			}
			
		}

		private string GetCustomerOrderFile(string AttachmentURI) {
			var regex = new Regex(@"^data:.*;base64");
			return regex.IsMatch(AttachmentURI ?? "") ?AttachmentURI : null;
		}

		private SaveResults CreateShipmentOrder(string carrierCode, int? shippingMethod, string shippingAccount, int shipmentID, int customerID, string invoiceNumber, string poNumber, decimal invoiceTotal, string masterTrackingNumber,
		 CustomerOrderShipmentContract shipment, CustomerOrderFillInfo cofi)
        {
			var shippingCost = 0f;
			var trackingNumber = "";
			DateTime? deliveryDate = null;
			var labelBytes = new List<byte[]>();

			var countries = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var states = _codeService.GetCodeLookupByType(CodeTypeEnum.States);
			var dimLookup = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipDimensionsUnit);
			var weightLookup = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipWeightUnit);
			//var cofi = _customerOrderService.GetCustomerOrderFillInfo(customerOrderID);
			var csi = _customerService.GetCustomerShippingInfo(cofi.CustomerID).FirstOrDefault(x => x.CustomerShippingInfoID == cofi.CustomerShippingInfoID);
			var customer = _customerService.GetCustomer(customerID);
			var boxes = _customerOrderService.GetBoxesForShip(shipmentID).ToList();
			
			//var serviceCode = _codeService.GetCodeLookupByType(CodeTypeEnum.UPSShipMethods)[shippingMethod ?? 0];
			//var shippingAccountId = shippingComp.CodeName == "UPS - MedGyn" ? "" : shipment.ShipAccountNumber;
			if (shipment.NumberOfSameBoxes > 1 && boxes.Count == 1)
				// add the box id more times to get an accurate quote
				for (var i = 1; i < shipment.NumberOfSameBoxes; i++)
					boxes.Add(boxes[0]);

			var cost = 0f;

			if (!string.IsNullOrEmpty(masterTrackingNumber))
			{
				trackingNumber = masterTrackingNumber;
			}
			else
			{
				for (int i = 0; i < boxes.Count; i++)
				{
					var serviceCode = carrierCode == ShippingCarrierCodes.FedExCarrierCode
						? _codeService.GetCodeLookupByType(CodeTypeEnum.FedExShipMethods)[shippingMethod ?? 0]
						: carrierCode == ShippingCarrierCodes.UpsCarrierCode
						? _codeService.GetCodeLookupByType(CodeTypeEnum.UPSShipMethods)[shippingMethod ?? 0]
						: null;

					string shipCustOrderName = $"{cofi.CustomerName}";
					string shipCustOrderID = $"{cofi.CustomerOrderID}";
					string ticks = $"{DateTime.Now.Ticks.ToString()}";
					int numUnderscores = 3;
					int tickLength = ticks.Length;
					int shippingOrderNumberLength = 50;
					int currentOrderLength = shipCustOrderName.Length + shipCustOrderID.Length + tickLength;
					if (currentOrderLength >= (shippingOrderNumberLength - numUnderscores))
					{
						int difference = shippingOrderNumberLength - tickLength - numUnderscores;
						int substringLength = difference / 2;
						shipCustOrderName = shipCustOrderName.Length > substringLength ? shipCustOrderName.Substring(0, substringLength) : shipCustOrderName;
						shipCustOrderID = shipCustOrderID.Length > substringLength ? shipCustOrderID.Substring(0, substringLength) : shipCustOrderID;
					}

					string shippingOrderNumber = $"{shipCustOrderName}_{shipCustOrderID}_{ticks}";
					string count = (i + 1).ToString();
					string boxCount = $"box {count} of {boxes.Count.ToString()}";

					var shippingOrder = _customerOrderService.CreateShippingOrderRequest(carrierCode, serviceCode.CodeName, boxes[i], csi, shippingAccount, customer.CustomerName, shippingOrderNumber, cofi.CustomerOrderCustomID, poNumber, boxCount);

					//extra box added from shipping does not have contents for international shipping need to use contents from first box
					if (shippingOrder.billTo.country.ToUpper() != "US" && shippingOrder.internationalOptions != null && shippingOrder.internationalOptions.customsItems.Count() == 0)
					{
						shippingOrder.internationalOptions.customsItems = _customerOrderService.GetBoxFillContentsForCustoms(cofi.CustomerOrderID, boxes[0].CustomerOrderShipmentBoxID);
					}

					//delaying 2 seconds to avoid rate limit error
					System.Threading.Thread.Sleep(2000);

					var order = _shipStationAPIService.CreateOrder(shippingOrder);

					if (order.success)
					{
						var labelRepsone = _shipStationAPIService.CreateOrderLabel(order);

						if (labelRepsone.success)
						{
							DateTime shipDate;
							shippingCost += labelRepsone.shipmentCost;
							if (!trackingNumber.IsNullOrEmpty())
								trackingNumber += ";";
							trackingNumber += labelRepsone.trackingNumber;
							if (DateTime.TryParse(labelRepsone.shipDate, out shipDate))
								deliveryDate = shipDate;
							labelBytes.Add(Convert.FromBase64String(labelRepsone.labelData));


						}
						else
						{
							return new SaveResults(labelRepsone.errorMessage);
						}
					}
					else
					{
						return new SaveResults(order.errorMessage);
					}


				}
			}

			_customerOrderService.CompleteShipment(shipmentID, shipment.CustomerOrderID,  invoiceNumber, trackingNumber, deliveryDate, invoiceTotal);
			var res = new SaveResults();
			if (string.IsNullOrEmpty(masterTrackingNumber))
			{
				using (var memoryStream = new MemoryStream())
				{
					using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
					{
						PdfDocument pdfDocument = new PdfDocument();
						var boxCount = 1;
						foreach (var image in labelBytes)
						{
                            PdfDocument label = new PdfDocument(image);
							pdfDocument.InsertPage(label,0);
						}

                        var zipEntry = archive.CreateEntry($"{invoiceNumber}-{boxCount++}.pdf");

						using (var zipEntryStream = zipEntry.Open())
						{
							pdfDocument.SaveToStream(zipEntryStream);
						}
						
                    }

					var bytes = memoryStream.ToArray();
					res.Payload = new { ZipFile = Convert.ToBase64String(bytes), Filename = $"{invoiceNumber}_Labels" };
				}
			}
			return res;
		}

		private float GetRateQuote(string carrierCode, int customerOrderID, int shipmentID, 
			int? shippingMethod, CustomerOrderShipBoxViewModel curBox)
		{
			var countries    = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var states       = _codeService.GetCodeLookupByType(CodeTypeEnum.States);
			var dimLookup    = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipDimensionsUnit);
			var weightLookup = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipWeightUnit);
			var cofi         = _customerOrderService.GetCustomerOrderFillInfo(customerOrderID);
			var csi          = _customerService.GetCustomerShippingInfo(cofi.CustomerID).FirstOrDefault(x => x.CustomerShippingInfoID == cofi.CustomerShippingInfoID);
			var customer     = _customerService.GetCustomer(cofi.CustomerID);
			var shipment     = _customerOrderService.GetCustomerOrderShipment(shipmentID);
			var boxes        = _customerOrderService.GetBoxesForShip(shipmentID).ToList();

			if (shipment.NumberOfSameBoxes > 1 && boxes.Count == 1)
				// add the box id more times to get an accurate quote
				for (var i = 1; i < shipment.NumberOfSameBoxes; i++)
					boxes.Add(boxes[0]);

			var cost = 0f;
			foreach (var box in boxes)
			{
				var weight = 0;
				var weightModel = new Weight
				{
					units = "",
					value = weight
				};

				var package = new Package();
				CodeContract weightUnit = null;
				if (box.CustomerOrderShipmentBoxID == curBox.CustomerOrderShipmentBoxID)
				{
					weight += curBox.Weight ?? 0;
					weightUnit = weightLookup[curBox.WeightUnitCodeID ?? 0];
				}
				else
				{
					weight += box.Weight ?? 0;
					weightUnit = weightLookup[box.WeightUnitCodeID ?? 0];
				}

				if (weightUnit?.CodeName == "LB")
				{
					weightModel.units = "ounces";
					weightModel.value = weight * 16;
				}
				if (weightUnit?.CodeName == "KG")
				{
					weightModel.units = "grams";
					weightModel.value = weight * 1000;
				}

				var serviceCode = carrierCode == ShippingCarrierCodes.FedExCarrierCode
					? _codeService.GetCodeLookupByType(CodeTypeEnum.FedExShipMethods)[shippingMethod ?? 0]
					: carrierCode == ShippingCarrierCodes.UpsCarrierCode
					? _codeService.GetCodeLookupByType(CodeTypeEnum.UPSShipMethods)[shippingMethod ?? 0]
					: null;

				var shippingRequest = new ShippingRateRequest()
				{
					carrierCode    = carrierCode,
					fromPostalCode = _shipStationAPISettings.ShipFromZip,
					toState        = states[csi.StateCodeID ?? 0]?.CodeName,
					toCountry      = CountryCodeLookup.Lookup(countries[csi.CountryCodeID].CodeName),
					toPostalCode   = csi.ZipCode ?? "",
					toCity         = csi.City,
					weight         = weightModel
				};


				var rates = _shipStationAPIService.GetRatesByCarrier(shippingRequest);
				var rate = rates.Where(r => r.serviceCode == serviceCode.CodeName).FirstOrDefault();
				if(rate != null)
				{
					cost += (rate.shipmentCost + rate.otherCost);
				}
			}
			return cost;
		}

		private void EmailApprovers(CustomerOrderContract customerOrder, CustomerContract customer, bool isRecipientVP)
		{
			var keys = new List<SecurityKeyEnum>();
			if(customer.IsDomestic || customer.IsDomesticAfaxys)
			{
				keys.Add(
					isRecipientVP
						? SecurityKeyEnum.CustomerOrderDomesticVPApproval
						: SecurityKeyEnum.CustomerOrderDomesticManagerApproval
					);
			}
			if(customer.IsDomesticDistributor)
			{
				keys.Add(
					isRecipientVP
						? SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval
						: SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval);
			}
			if(customer.IsInternational)
			{
				keys.Add(
					isRecipientVP
						? SecurityKeyEnum.CustomerOrderInternationalVPApproval
						: SecurityKeyEnum.CustomerOrderInternationalManagerApproval);
			}

			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(ApprovalEmailViewModel))
				.UseMemoryCachingProvider()
				.Build();


			//6/7/20 jcb: instead of sending 1 email to each approver each, send 1 email to all of them together
			//				so they know about each other and can reply all
			var model = new ApprovalEmailViewModel
			{
				ApproverName = "Team",
				ApproveeName = customer.CustomerName,
				Url = _appSettings.Url
			};
			var body = engine.CompileRenderAsync("Views.CustomerOrder.ApprovalRequestEmail", model).Result;

			var users = _userService.GetUsersWithKey(keys.ToArray());
			string recipients = "";
			foreach (var user in users)
			{
				if (!user.IsDeleted)
				{
					recipients += user.Email.Trim() + ';';
					if (users.Count == 1)
					{
						model.ApproverName = user.FirstName;
					}
				}
			}

			if(users.Any())
				_emailService.SendEmail(recipients, "Approval Needed for Customer Order", body);
		}

		public void MarkOrderAsFilling(int customerOrderID)
		{
			var fillInfo = _customerOrderService.GetCustomerOrderFillInfo(customerOrderID);

			if(fillInfo == null || !fillInfo.IsFilling) 
				_customerOrderService.MarkOrderAsFilling(customerOrderID);
		}

		private Invoice GetInvoiceData(int shipmentID)
		{
			var countries         = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var exemptTaxCodeID   = _codeService.GetCodesByType(CodeTypeEnum.CustomerSalesTax).FirstOrDefault(x => x.CodeName == "Exempt")?.CodeID ?? 0;
			var states            = _codeService.GetCodeLookupByType(CodeTypeEnum.States);
			var shipCompanies     = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipCompanies);
			var shipMethods       = _codeService.GetCodesByType(CodeTypeEnum.FedExShipMethods)
				.Concat(_codeService.GetCodesByType(CodeTypeEnum.UPSShipMethods))
				.Concat(_codeService.GetCodesByType(CodeTypeEnum.OtherShipMethods))
				.ToDictionary(x => x.CodeID);

			var shipment = _customerOrderService.GetCustomerOrderShipment(shipmentID);
			var co       = _customerOrderService.GetCustomerOrder(shipment.CustomerOrderID, 0, true, true, true, true);
			var coProds  = _customerOrderService.GetCustomerOrderProducts(co.CustomerOrderID).ToDictionary(x => x.ProductID);
			var ips      = _customerOrderService.GetInvoiceProducts(shipmentID);
			var c        = _customerService.GetCustomer(co.CustomerID);
			var csi      = _customerService.GetCustomerShippingInfo(co.CustomerID).FirstOrDefault(x => x.CustomerShippingInfoID == co.CustomerShippingInfoID);

			UserContract salesRep = null;
			if (csi != null)
            {
				salesRep = _userService.GetUser(csi.RepUserID);
			}
			
			

			var fills = _customerOrderService.GetCustomerOrderFillProducts(shipment.CustomerOrderID, null)
				.ToDictionary(x => (int)x.ProductID, x => (int?)x.QuantityAlreadyPacked);

			var products = new List<InvoiceProduct>();
			foreach(var ip in ips.GroupBy(x => x.ProductID))
			{
				var curIP = ip.First();
				fills.TryGetValue(curIP.ProductID, out var quantityAlreadyPacked);

				var newIP = new InvoiceProduct()
					{
						ProductID         = curIP.ProductID,
						ProductCustomID   = curIP.ProductCustomID,
						ProductName       = curIP.ProductName,
						Quantity          = curIP.Quantity,
						BackorderQuantity = curIP.Quantity - (quantityAlreadyPacked ?? 0),
						QuantityPacked    = ip.Sum(x => x.QuantityPacked),
						SerialNumbers     = string.Join(",", ip.Select(x => x.SerialNumbers)),
						
					};

				//if there are more than 2 decimal places, we want to round the price to 2 decimal places when it ends in multiple 0s
				string cents = coProds[curIP.ProductID].Price.ToString().Split('.')[1];
				if (cents.Length > 2)
				{
					string extraCents = cents.Substring(2, (cents.Length - 2));

					if (Convert.ToDouble(extraCents) == 0)
					{
						newIP.Price = decimal.Parse(coProds[curIP.ProductID].Price.ToString("0.00"));
					}
					else
					{
						newIP.Price = coProds[curIP.ProductID].Price;
					}

				}
				else
				{
					newIP.Price = coProds[curIP.ProductID].Price;
				}

				if (newIP.BackorderQuantity == 0)
					newIP.BackorderQuantity = null;

				products.Add(newIP);
			}

			var bytes = _env.EnvironmentName == tmEnv ? File.ReadAllBytes(tmLogo) : File.ReadAllBytes(medGynLogo);
			var file = Convert.ToBase64String(bytes);

			var paymentTerms = c.PaymentTermsType == 1 ? "COD"
				: c.PaymentTermsType == 2 ? "Prepay"
				: $"Net Due in {c.PaymentTermsNetDueDays} days";

			DateTime? paymentDate;
			if(c.PaymentTermsType == 1)
				paymentDate = shipment.DeliveryDate;
			else if (c.PaymentTermsType == 2)
				paymentDate = co.SubmitDate;
			else
				paymentDate = shipment.InvoiceDate?.AddDays(c.PaymentTermsNetDueDays.Value + 1);

			var subtotal = Math.Round(products.Sum(x => x.Ext), 2);
			var shippingCharge = shipment.ShippingCharge ?? 0;
			var totalAmount = subtotal + shippingCharge;
			var taxes = 0m;

			if(c.SalesTaxCodeID != exemptTaxCodeID && states[csi.StateCodeID ?? 0]?.CodeName == "IL")
				taxes = Decimal.Round(subtotal * (_appSettings.IllinoisTax / 100), 2);

			states.TryGetValue(c.StateCodeID ?? -1, out var customerStateCode);
			var customerState = customerStateCode?.CodeName;

			countries.TryGetValue(c.CountryCodeID, out var customerCountryCode);
			var customerCountry = customerCountryCode?.CodeDescription;
			var customerShipState = "";
			string customerShipCountry = "";

			if (csi != null)
			{
				states.TryGetValue(csi.StateCodeID ?? -1, out var customerShipStateCode);
				customerShipState = customerShipStateCode?.CodeName;

				countries.TryGetValue(csi.CountryCodeID, out var customerShipCountryCode);
				customerShipCountry = customerShipCountryCode?.CodeDescription;
			}

			shipMethods.TryGetValue(shipment.ShipMethodCodeID ?? -1, out var shipMethodCode);
			var shipMethod = shipMethodCode?.CodeDescription;

			totalAmount += taxes;

			var paymentCodes = _codeService.GetCodesByType(CodeTypeEnum.PaymentType);
			var creditCardPaymentType = paymentCodes.First(f => f.CodeName == "CreditCard");
			var creditCardFee = (c.PaymentTypeCodeID == creditCardPaymentType.CodeID) ? Decimal.Round(_appSettings.CreditCardFee / 100 * (subtotal + shippingCharge + taxes), 2) : 0;

			totalAmount += creditCardFee;

			var model = new Invoice {
				MedGynAddress             = _appSettings.MedGynAddress,
				MedGynCity                = _appSettings.MedGynCity,
				MedGynStateOrProvinceCode = _appSettings.MedGynStateOrProvinceCode,
				MedGynPostalCode          = _appSettings.MedGynPostalCode,
				MedGynContactPhone        = Regex.Replace(_appSettings.MedGynContactPhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
				MedGynFax                 = Regex.Replace(_appSettings.MedGynFax, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
				MedGynTollFreePhone       = Regex.Replace(_appSettings.MedGynTollFreePhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"),
				MedGynEmail               = _appSettings.MedGynEmail,
				InvoiceRemittanceAddress  = _appSettings.InvoiceRemittanceAddress,
				InvoiceRemittanceMessage  = _appSettings.InvoiceRemittanceMessage,
				InvoiceFooter             = _appSettings.InvoiceFooter,
				LogoString                = file,
				InvoiceNumber             = shipment.InvoiceNumber,
				InvoiceDate               = shipment.InvoiceDate?.ToString("MM/dd/yyyy"),
				CustomerName              = c.CustomerName,
				CustomerAddress           = c.Address1,
				CustomerAddress2          = c.Address2,
				CustomerCity              = c.City,
				CustomerState             = customerState,
				CustomerCountry           = customerCountry,
				CustomerZip               = c.ZipCode,
				CustomerShipName          = csi?.Name,
				CustomerShipAddress       = csi?.Address,
				CustomerShipAddress2      = csi?.Address2,
				CustomerShipCity          = csi?.City,
				CustomerShipState         = customerShipState,
				CustomerShipCountry       = customerShipCountry,
				CustomerShipZip           = csi?.ZipCode,
				CustomerOrderNumber       = co.CustomerOrderCustomID,
				CustomerOrderDate         = co.SubmitDate?.ToString("MM/dd/yyyy"),
				SalesRep                  = $"{salesRep?.SalesRepID} {salesRep?.FullName}",
				ShipCompany               = shipCompanies[shipment.ShipCompanyType ?? 0]?.CodeDescription,
				ShipMethod                = shipMethod,
				ShipDate                  = shipment.InvoiceDate?.ToString("MM/dd/yyyy"),
				IsPartialShip             = co.IsPartialShipAcceptable ? "Yes" : "No",
				CustomerID                = c.CustomerCustomID,
				CustomerPONumber          = co.PONumber,
				PaymentTerms              = paymentTerms,
				PaymentDate               = paymentDate?.ToString("MM/dd/yyyy") ?? "N/A",
				TrackingNumber            = shipment.MasterTrackingNumber,
				Instructions              = co.Instructions,
				Subtotal                  = subtotal,
				Taxes                     = taxes,
				ShippingCharge            = shippingCharge,
				TotalAmount               = totalAmount,
				Products                  = products,
				ISName                    = co.IntermediaryShippingName,
				ISAddress                 = co.IntermediaryShippingAddress,
				ISContactName             = co.IntermediaryShippingContactName,
				ISContactNumber           = co.IntermediaryShippingContactNumber,
				ISContactEmail            = co.IntermediaryShippingContactEmail,
				IsInternational           = c.IsInternational,
				ShippingCustomerName	  = String.IsNullOrWhiteSpace(co.ShippingCustomerName) ? c.CustomerName : co.ShippingCustomerName,
				CreditCardFee			  = creditCardFee.ToString("0.00")
			};

			return model;
		}

		public async Task<ArchivedInvoiceListViewModel> GetArchivedInvoiceListViewModel(SearchCriteriaViewModel sc)
		{
			var showAll = _authenticationService.HasAnyClaim(new List<int>(){(int)SecurityKeyEnum.CustomerOrderCustomersSeeAll,
				(int)SecurityKeyEnum.ArchiveSeeAll,
				(int)SecurityKeyEnum.ArchiveSeeAllNoTotals});
			var showInternational = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				(int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderInternationalVPApproval,
				
			});
			var showDomesticDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
			});
			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticVPApproval,

			});
			var results = await _customerOrderService.GetArchivedInvoiceList(sc.Search, sc.SortColumn, sc.SortAsc, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);
			var customerOrderVMs = results.Select(x => new ArchivedInvoiceViewModel(x)).ToList();

			return new ArchivedInvoiceListViewModel(sc, customerOrderVMs.ToList());
		}

		public async Task<InvoiceActivityListViewModel> GetInvoiceActivtyViewModel(SearchCriteriaViewModel sc, DateTime StartDate, DateTime EndDate)
		{
			var showAll = _authenticationService.HasAnyClaim(new List<int>(){(int)SecurityKeyEnum.CustomerOrderCustomersSeeAll,
				(int)SecurityKeyEnum.ArchiveSeeAll,
				(int)SecurityKeyEnum.ArchiveSeeAllNoTotals});
			var showInternational = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				(int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderInternationalVPApproval,

			});
			var showDomesticDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
			});
			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticVPApproval,

			});
			var results = await _customerOrderService.GetInvoiceActivityList(sc.Search, sc.SortColumn, sc.SortAsc, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors, StartDate, EndDate);
			var customerOrderVMs = results.Select(x => new InvoiceActivityViewModel(x)).ToList();

			return new InvoiceActivityListViewModel(sc, customerOrderVMs.ToList());
		}

		public async Task<byte[]> GetInvoiceActivtyExport(SearchCriteriaViewModel sc, DateTime StartDate, DateTime EndDate)
		{
			var showAll = _authenticationService.HasAnyClaim(new List<int>(){(int)SecurityKeyEnum.CustomerOrderCustomersSeeAll,
				(int)SecurityKeyEnum.ArchiveSeeAll,
				(int)SecurityKeyEnum.ArchiveSeeAllNoTotals});
			var showInternational = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				(int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderInternationalVPApproval,

			});
			var showDomesticDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
			});
			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticVPApproval,

			});
			var results = await _customerOrderService.GetInvoiceActivityList(sc.Search, sc.SortColumn, sc.SortAsc, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors, StartDate, EndDate);
			var invoices = results.Select(x => new InvoiceActivityViewModel(x)).ToList();


			var table = new System.Data.DataTable("Invoice Activity List");


			//create a class based on the properties of the view model
			var properties = new List<(string name, string label)>() {
				( nameof(InvoiceActivityViewModel.InvoiceNumber), "Invoice Number" ),
			   (nameof(InvoiceActivityViewModel.InvoiceDate), "Invoice Date"),
			   (nameof(InvoiceActivityViewModel.CustomerOrderCustomID), "Order Number"),
			   (nameof(InvoiceActivityViewModel.Company), "Company"),
			   (nameof(InvoiceActivityViewModel.Contact), "Primary Contact"),
			   (nameof(InvoiceActivityViewModel.Email), "Primary Email"),
			   (nameof(InvoiceActivityViewModel.Phone), "Primary Phone"),
			   (nameof(InvoiceActivityViewModel.Practice), "Practice Type"),
			};

			using (var excel = new ExcelPackage())
			{
				var sheet = excel.Workbook.Worksheets.Add("Invoice Activity List");
				for (var col = 1; col < properties.Count + 1; col++)
				{
					sheet.Cells[1, col].Value = properties[col - 1].label;
					sheet.Cells[1, col].Style.Font.Bold = true;
				}

				for (var row = 2; row < invoices.Count + 2; row++)
				{
					for (var col = 1; col < properties.Count + 1; col++)
					{
						var value = typeof(InvoiceActivityViewModel)
							.GetProperty(properties[col - 1].name)
							.GetValue(invoices[row - 2]);
						if (value != null && !string.IsNullOrEmpty(value.ToString()))
							sheet.Cells[row, col].Value = value.ToString();
					}
				}
				return excel.GetAsByteArray();
			}
		}

		public byte[] GetPeachTreeInvoiceExportByBatchId(int BatchId)
		{
			var invoiceItems = _customerOrderService.GetPeachTreeInvoiceExportByBatchId(BatchId);
			using (var ms = new MemoryStream())
			using (var writer = new StreamWriter(ms))
			{
				using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
				{
					csv.WriteRecords(invoiceItems);
				}

				return ms.ToArray();
			}
		}

		public async Task<IList<dynamic>> GetFilteredProductShippedViewModel(SearchCriteriaViewModel sc)
		{
			var showAll = _authenticationService.HasAnyClaim(new List<int>(){(int)SecurityKeyEnum.CustomerOrderCustomersSeeAll,
				(int)SecurityKeyEnum.ProductShippedSeeAllNoTotals,
				(int)SecurityKeyEnum.ProductShippedSeeAllWithTotals});

			var showInternational = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				(int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderInternationalVPApproval
			});
			var showDomesticDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
			});
			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticVPApproval,
			});

            var results = await _customerOrderService.GetFilteredProductShippedList(sc.Search, sc.SortColumn, sc.SortAsc, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);
			return results;
		}

		public async Task<ProductShippedListViewModel> ConvertFilteredListForDisplay(SearchCriteriaViewModel sc, IList<dynamic> results)
        {
            List<UserContract> users = await _userService.GetAllUsersAsync();
            var customerOrderVMs = results.Select(x => new ProductShippedViewModel(x, users));
			return new ProductShippedListViewModel(sc, customerOrderVMs.ToList());
		}


		// method to build a Back Ordered Invoice number connecting
		// original Invoice Number
		static String GetBOInvoiceNumber(string orgInvoiceNumber, int iCount)
        {
			StringBuilder invoiceNumber = new StringBuilder();
			StringBuilder updatedInvoiceNumber = new StringBuilder();
			invoiceNumber.Append(orgInvoiceNumber);
			int bInvoiceNumber = invoiceNumber.ToString().IndexOf('B');
			if (bInvoiceNumber > 0)
			{
				updatedInvoiceNumber.Append(invoiceNumber.ToString().Substring(0, bInvoiceNumber + 1));
				//updatedInvoiceNumber.Append(bInvoiceNumber > 0 ? (invoiceNumber.ToString().Substring(bInvoiceNumber + 1, invoiceNumber.Length - bInvoiceNumber - 1)).ToInt() + 1 : "B1");
				updatedInvoiceNumber.Append(iCount);
			}
			else
			{
				updatedInvoiceNumber.Append(invoiceNumber);
				updatedInvoiceNumber.Append("B1");
			}

			return updatedInvoiceNumber.ToString();
		}

		static String GetLetterSequence(string result)
		{
			int resultLength = 0;
			char lastChar = ' ';
			if (result.Length <= 0)
			{
				result = "";
			}
			else
			{ 
				resultLength = result.Length;
			}
//			for (int i = 65; i <= 90; i++)
//			{
//				for (int j = 65; j <= 90; j++)
//				{
//					result += new String(new char[] { (char)i, (char)j });
//				}
//			}

			for (int i = 65; i <= 90; i++)
			{
				result += new String(new char[] { (char)i});
			}
		
			if (lastChar == 'Z' || lastChar == ' ')
				return result.Substring(0, resultLength + 1);
			else
				return result.Substring(0, resultLength);
		}

		static char IncrementCharacter(char input)
		{
			return (input == 'Z' ? 'A' : (char)(input + 1));
		}

		IEnumerable<string> GetSequence(string start = "")
		{
			StringBuilder chars = start == null ? new StringBuilder() : new StringBuilder(start);

			while (true)
			{
				int i = chars.Length - 1;
				while (i >= 0 && chars[i] == 'Z')
				{
					chars[i] = 'A';
					i--;
				}
				if (i == -1)
					chars.Insert(0, 'A');
				else
					chars[i]++;
				yield return chars.ToString();
			}
		}

		public async Task<byte[]> ExportProductListExcel(SearchCriteriaViewModel sc, int type, CustomerOrderStatusEnum status, DateRangeEnum dateOption)
		{
			var showAll = _authenticationService.HasClaim((int)SecurityKeyEnum.CustomerOrderCustomersSeeAll);
			var showInternational = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				(int)SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderInternationalVPApproval
			});
			var showDomesticDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval
			});
			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int>{
				(int)SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				(int)SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				(int)SecurityKeyEnum.CustomerOrderDomesticVPApproval
			});
			var results = type == 1 ? await _customerOrderService.GetBackOrderList(sc.Search, sc.SortColumn, sc.productCustomID, sc.SortAsc, status, dateOption, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors) : await _customerOrderService.GetFilteredProductShippedList(sc.Search, sc.SortColumn, sc.SortAsc, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);

			if(type == 2)
            {
				CultureInfo format = new CultureInfo("en-US");
				for (var i = 0; i < results.Count; i++)
                {
					IDictionary<string, object> propertyValues = results[i];
					int quantity = int.Parse(propertyValues[nameof(CustomerOrderProductViewModel.Quantity)].ToString());
					double price = double.Parse(propertyValues[nameof(CustomerOrderProductViewModel.Price)].ToString());
					propertyValues["Total"] = "$" + (Math.Round((quantity * price), 2)).ToString("N", format); ;
					results[i] = propertyValues;
				}
			}

			var properties = type == 1 ? new List<(string name, string label)>() {
				(nameof(CustomerOrderBriefViewModel.SubmitDate), "Order Date"),
				(nameof(CustomerOrderBriefViewModel.PONumber), "Cust PO #"),
				(nameof(CustomerOrderBriefViewModel.CustomerOrderCustomID), "Order #"),
				(nameof(CustomerOrderBriefViewModel.CustomerCustomID), "Cust ID"),
				(nameof(CustomerOrderBriefViewModel.CustomerName), "Cust Name"),
				(nameof(CustomerOrderProductFillViewModel.ProductCustomID), "Product ID"),
				(nameof(CustomerOrderProductFillViewModel.ProductName), "Product Name"),
				(nameof(CustomerOrderProductViewModel.Quantity), "Quantity"),
			}: new List<(string name, string label)>() {
				("InvoiceDate", "Invoice Date"),
				("InvoiceNumber", "Invoice #"),
				(nameof(CustomerOrderBriefViewModel.CustomerOrderCustomID), "Order #"),
				(nameof(CustomerOrderBriefViewModel.PONumber), "Cust PO #"),
				(nameof(CustomerOrderBriefViewModel.CustomerCustomID), "Cust ID"),
				(nameof(CustomerOrderProductFillViewModel.ProductCustomID), "Product ID"),
				(nameof(CustomerOrderProductViewModel.Quantity), "Quantity"),
				("Total", "Total"),
                (nameof(CustomerOrderProductViewModel.FilledBy), "Packed By"),
                (nameof(CustomerOrderProductViewModel.ShippedBy), "Shipped By"),
            };

			var users = _userService.GetAllUsers();
			using (var excel = new ExcelPackage())
			{
				var sheet = excel.Workbook.Worksheets.Add(type == 1 ? "BackOrder List" : "Products Shipped");
				for (var col = 1; col < properties.Count + 1; col++)
				{
					sheet.Cells[1, col].Value = properties[col - 1].label;
					sheet.Cells[1, col].Style.Font.Bold = true;
				}

				for (var row = 2; row < results.Count + 2; row++)
				{
					for (var col = 1; col < properties.Count + 1; col++)
					{
						var prop = properties[col - 1].name;
						var obj = results[row - 2];
						IDictionary<string, object> propertyValues = obj;
						var val = propertyValues[prop];
						if (nameof(CustomerOrderProductViewModel.FilledBy) == prop)
						{
							if (val != null && !String.IsNullOrEmpty(val.ToString()))
							{ 
									var user = users.FirstOrDefault(f => f.UserId == int.Parse(val.ToString()));
								if(user!=default)
									sheet.Cells[row, col].Value = user.FullName;
		
							}
                        }
                        else if (nameof(CustomerOrderProductViewModel.ShippedBy) == prop)
                        {
                            if (val != null && !String.IsNullOrEmpty(val.ToString()))
                            {
                                var user = users.FirstOrDefault(f => f.UserId == int.Parse(val.ToString()));
                                if (user != default)
                                    sheet.Cells[row, col].Value = user.FullName;

                            }
                        }
                        else
						{
                            sheet.Cells[row, col].Value = val;
                        }
                        
					}
				}
				return excel.GetAsByteArray();
			}
		}

		public class Invoice

		{
			public string MedGynAddress { get; set; }
			public string MedGynCity { get; set; }
			public string MedGynStateOrProvinceCode { get; set; }
			public string MedGynPostalCode { get; set; }
			public string MedGynContactPhone { get; set; }
			public string MedGynFax { get; set; }
			public string MedGynTollFreePhone { get; set; }
			public string MedGynEmail { get; set; }
			public string InvoiceRemittanceAddress { get; set; }
			public string InvoiceRemittanceMessage { get; set; }
			public string InvoiceFooter { get; set; }
			public string LogoString { get; set; }
			public string InvoiceNumber { get; set; }
			public string InvoiceDate { get; set; }
			public string CustomerName { get; set; }
			public string CustomerAddress { get; set; }
			public string CustomerAddress2 { get; set; }
			public string CustomerCity { get; set; }
			public string CustomerState { get; set; }
			public string CustomerCountry { get; set; }
			public string CustomerZip { get; set; }
			public string CustomerShipName { get; set; }
			public string CustomerShipAddress { get; set; }
			public string CustomerShipAddress2 { get; set; }
			public string CustomerShipCity { get; set; }
			public string CustomerShipState { get; set; }
			public string CustomerShipCountry { get; set; }
			public string CustomerShipZip { get; set; }
			public string CustomerOrderNumber { get; set; }
			public string CustomerOrderDate { get; set; }
			public string SalesRep { get; set; }
			public string ShipCompany { get; set; }
			public string ShipMethod { get; set; }
			public string ShipDate { get; set; }
			public string IsPartialShip { get; set; }
			public string CustomerID { get; set; }
			public string CustomerPONumber { get; set; }
			public string PaymentTerms { get; set; }
			public string PaymentDate { get; set; }
			public string TrackingNumber { get; set; }
			public string Instructions { get; set; }
			public decimal Subtotal { get; set; }
			public decimal Taxes { get; set; }
			public decimal ShippingCharge { get; set; }
			public decimal TotalAmount { get; set; }
			public List<InvoiceProduct> Products { get; set; }
			public string ISName { get; set; }
			public string ISAddress { get; set; }
			public string ISContactName { get; set; }
			public string ISContactNumber { get; set; }
			public string ISContactEmail { get; set; }
			public bool IsInternational { get; set; }
            public string ShippingCustomerName { get; set; }
			public string CreditCardFee { get; set; }

		}
	}
}