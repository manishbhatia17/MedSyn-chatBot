using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Common.Constants;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using OpenHtmlToPdf;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Facades
{
	public class PurchaseOrderFacade : IPurchaseOrderFacade
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly ICodeService _codeService;
		private readonly ICustomerOrderService _customerOrderService;
		private readonly IProductService _productService;
		private readonly IPurchaseOrderService _purchaseOrderService;
		private readonly IUserService _userService;
		private readonly IVendorService _vendorService;
		private readonly IEmailService _emailService;
		private readonly AppSettings _appSettings;

		//used to check environment variable for images
		private readonly IWebHostEnvironment _env;

		private const string tmLogo = "wwwroot/images/TM_Logo.png";
		private const string medGynLogo = "wwwroot/images/Logo.png";
		private const string tmEnv = "PROD-TM";

		public PurchaseOrderFacade(
			IAuthenticationService authenticationService,
			ICodeService codeService,
			ICustomerOrderService customerOrderService,
			IProductService productService,
			IPurchaseOrderService purchaseOrderService,
			IUserService userService,
			IVendorService vendorService,
			IEmailService emailService,
			IOptions<AppSettings> appSettings,
			IWebHostEnvironment env
		)
		{
			_authenticationService = authenticationService;
			_codeService           = codeService;
			_customerOrderService  = customerOrderService;
			_productService        = productService;
			_purchaseOrderService  = purchaseOrderService;
			_userService           = userService;
			_vendorService         = vendorService;
			_emailService          = emailService;
			_appSettings           = appSettings.Value;
			_env = env;
		}

		public PurchaseOrderListViewModel GetPurchaseOrderListViewModel(SearchCriteriaViewModel sc, PurchaseOrderStatusEnum status, DateRangeEnum timeframe)
		{
			int userId = _authenticationService.GetUserID();
			var showAll = _authenticationService.HasAnyClaim(new List<int> {
				(int)SecurityKeyEnum.PurchaseOrderView,
				(int)SecurityKeyEnum.PurchaseOrderEdit
			});
			var showInternational = _authenticationService.HasAnyClaim(new List<int> {
				(int)SecurityKeyEnum.PurchaseOrderInternationalView,
				(int)SecurityKeyEnum.PurchaseOrderInternationalEdit,
				(int)SecurityKeyEnum.PurchaseOrderInternationalVPApproval
			});
			var showDomesticDistributors = _authenticationService.HasAnyClaim(new List<int> {
				(int)SecurityKeyEnum.PurchaseOrderDomesticDistributionView,
				(int)SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				(int)SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval
			});
			var showDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int> {
				(int)SecurityKeyEnum.PurchaseOrderDomesticView,
				(int)SecurityKeyEnum.PurchaseOrderDomesticEdit,
				(int)SecurityKeyEnum.PurchaseOrderDomesticVPApproval
			});

			var illinoisCode = _codeService.GetCodesByType(CodeTypeEnum.States).FirstOrDefault(c => c.CodeName.Equals("IL"));
			var results      = _purchaseOrderService.GetPurchaseOrderList(sc.Search, sc.SortColumn, sc.SortAsc, status, timeframe,
				showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors, userId);

			var purchaseOrderVMs = results.Select(r => new PurchaseOrderBriefViewModel(r, illinoisCode, _appSettings)).ToList();
			return new PurchaseOrderListViewModel(sc, purchaseOrderVMs);
		}

		public PurchaseOrderDetailsViewModel GetPurchaseOrderDetails(int purchaseOrderID, int? productID, int? priVendorID)
		{
			var editAll = _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderView);
			var showAll = editAll || _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderView);
			var editInternational = _authenticationService.HasAnyClaim(new List<int> {
				(int)SecurityKeyEnum.PurchaseOrderInternationalEdit,
				(int)SecurityKeyEnum.PurchaseOrderInternationalVPApproval
			});
			var showInternational = editInternational
				|| _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderInternationalView);
			var editDomesticDistributors = _authenticationService.HasAnyClaim(new List<int> {
				(int)SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				(int)SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval
			});
			var showDomesticDistributors = editDomesticDistributors
				|| _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderDomesticDistributionView);
			var editDomesticNonDistributors = _authenticationService.HasAnyClaim(new List<int> {
				(int)SecurityKeyEnum.PurchaseOrderDomesticEdit,
				(int)SecurityKeyEnum.PurchaseOrderDomesticVPApproval
			});
			var showDomesticNonDistributors = editDomesticNonDistributors
				|| _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderDomesticView);

			if (!(showAll || showInternational || showDomesticDistributors || showDomesticNonDistributors))
				return null;

			var allVendors = _vendorService.GetAllVendors().ToList();
			var accessibleVendors = new List<VendorContract>();
			if (!editAll)
			{
				if (editInternational)
					accessibleVendors.AddRange(allVendors.Where(v => v.IsInternational));
				if (editDomesticDistributors)
					accessibleVendors.AddRange(allVendors.Where(v => v.IsDomesticDistributor));
				if (editDomesticNonDistributors)
					accessibleVendors.AddRange(allVendors.Where(v => v.IsDomestic || v.IsDomesticAfaxys));
			}
			else
			{
				accessibleVendors = allVendors.ToList();
			}

			var inactiveVendorStatusCode = _codeService.GetCodesByType(CodeTypeEnum.VendorStatus).First(c => c.CodeDescription.ToLower().Equals("active"));
			var illinoisCodeID = _codeService.GetCodesByType(CodeTypeEnum.States).FirstOrDefault(x => x.CodeName == "IL")?.CodeID ?? 0;

			var ret = new PurchaseOrderDetailsViewModel(_appSettings) {
				FedExShipMethodCodes  = _codeService.GetCodesByType(CodeTypeEnum.FedExShipMethods).ToDropdownList(),
				UPSShipMethodCodes    = _codeService.GetCodesByType(CodeTypeEnum.UPSShipMethods).ToDropdownList(),
				UMCodes               = _codeService.GetCodesByType(CodeTypeEnum.UnitOfMeasure).ToDictionary(c => c.CodeID.ToString(), c => c.CodeDescription),
				Vendors               = accessibleVendors.Select(x => {
					var visible = x.VendorStatusCodeID == inactiveVendorStatusCode.CodeID;
					return new DropdownDisplayViewModel(x.VendorID, $"{x.VendorCustomID} {x.VendorName}") { Visible = visible};
				}).ToList(),
				VendorTaxes           = accessibleVendors.ToDictionary(x => $"{x.VendorID}", x => x.StateCodeID == illinoisCodeID ? _appSettings.IllinoisTax : 0),
			};

			if(purchaseOrderID == 0) {
				ret.PurchaseOrder = new PurchaseOrderViewModel(){
					IsPartialShipAcceptable = true
				};
				if (priVendorID.HasValue)
				{
					ret.PurchaseOrder.VendorID = priVendorID.Value;
				}
				if (productID.HasValue)
				{
					ret.PurchaseOrder.Products.Add(new PurchaseOrderProductViewModel()
					{
						ProductID = productID.Value,
					});
				}
			}
			else
			{
				var po = _purchaseOrderService.GetPurchaseOrder(purchaseOrderID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);
				var pops = _purchaseOrderService.GetPurchaseOrderProducts(purchaseOrderID);
				var updateUser = _userService.GetUser(po.UpdatedBy);
				var receivedBy = _userService.GetUser(po.ReceivedBy ?? 0);
				ret.PurchaseOrder = new PurchaseOrderViewModel(po)
				{
					Products  = pops.Select(x => new PurchaseOrderProductViewModel(x)).ToList(),
					UpdatedBy = updateUser.FullName,
					ReceivedBy = receivedBy != null ? receivedBy.FullName : ""
				};
				var vendor = _vendorService.GetVendor(po.VendorID);
				if ((vendor.IsDomestic || vendor.IsDomesticAfaxys) && !(showAll || showDomesticNonDistributors))
					return null;
				if (vendor.IsDomesticDistributor && !(showAll || showDomesticDistributors))
					return null;
				if (vendor.IsInternational && !(showAll || showInternational))
					return null;
				ret.PurchaseOrder.IsDomestic = vendor.IsDomestic;
				ret.PurchaseOrder.IsDomesticDistribution = vendor.IsDomesticDistributor;
				ret.PurchaseOrder.IsInternational = vendor.IsInternational;

				if (po.ApprovedBy.HasValue)
				{
					var approvalUser             = _userService.GetUser(po.ApprovedBy.Value);
					ret.PurchaseOrder.ApprovedBy = approvalUser.FullName;
				}
			}

			return ret;
		}

		public bool DeletePurchaseOrder(int purchaseOrderID)
		{
			return _purchaseOrderService.DeletePurchaseOrder(purchaseOrderID);
		}
		public async Task<PurchaseOrderReceiveViewModel> GetPurchaseOrderReceipt(int purchaseOrderID)
		{
			var pori = _purchaseOrderService.GetPurchaseOrderReceiveInfo(purchaseOrderID);
			var user = _userService.GetUser(pori.UpdatedBy);
			var products = await _purchaseOrderService.GetPurchaseOrderReceiptProducts(purchaseOrderID);
			var productModels = products.Select(p => new PurchaseOrderProductReceiptViewModel(p)).ToList();

			return new PurchaseOrderReceiveViewModel(user.FullName, pori) {
				Products = productModels
			};
		}

		public OrderProductsViewModel GetPurchaseOrderHistoryForProduct(int productID)
		{
			var product     = _productService.GetProduct(productID);
			var stockInfo   = _productService.GetProductStockInfo(productID);
			var history     = _purchaseOrderService.GetHistoryForProduct(productID);

			var ret = new OrderProductsViewModel()
			{
				ProductID   = productID,
				UMCodeID    = product.UnitOfMeasureCodeID,
				StockInfo   = stockInfo,
				LowestPrice = history
					.OrderBy(x => x.Price)
					.Where(x => ((DateTime)x.SubmitDate).AddYears(1) > DateTime.UtcNow)
					.Take(1)
					.Select(x => new PurchaseOrderProductsHistoryViewModel(x))
					.FirstOrDefault(),
				POHistory = history
					.OrderByDescending(x => x.SubmitDate)
					.Take(5)
					.Select(x => new PurchaseOrderProductsHistoryViewModel(x))
					.ToList()
			};

			return ret;
		}

		public bool CanEditOrder(PurchaseOrderViewModel purchaseOrder)
		{
			if (_authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderEdit))
				return true;
			var vendor = _vendorService.GetVendor(purchaseOrder.VendorID.Value);
			if (vendor.IsDomestic || vendor.IsDomesticAfaxys)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderDomesticVPApproval);
			if (vendor.IsDomesticDistributor)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval);
			if (vendor.IsInternational)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderInternationalVPApproval);

			return false;
		}

		public bool CanApproveOrder(int purchaseOrderId)
		{
			var order = _purchaseOrderService.GetPurchaseOrder(purchaseOrderId);
			var vendor = _vendorService.GetVendor(order.VendorID);
			if (vendor.IsDomestic || vendor.IsDomesticAfaxys)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderDomesticVPApproval);
			if (vendor.IsDomesticDistributor)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval);
			if (vendor.IsInternational)
				return _authenticationService.HasClaim((int)SecurityKeyEnum.PurchaseOrderInternationalVPApproval);

			return false;
		}

		public async Task<PurchaseOrderViewModel> SavePurchaseOrder(PurchaseOrderViewModel purchaseOrder, bool submit)
		{
			var contract = purchaseOrder.ToContract();
			var oldPO = _purchaseOrderService.GetPurchaseOrder(purchaseOrder.PurchaseOrderID);
			var savedPO = _purchaseOrderService.SavePurchaseOrder(contract, submit);

			var toSave = purchaseOrder.Products.Where(x => !x.MarkedForDelete);
			var toDelete = purchaseOrder.Products.Where(x => x.MarkedForDelete);
			if (toSave.Any())
			{
				var products = toSave.Select(x => x.ToContract(savedPO.PurchaseOrderID)).ToList();
				await _purchaseOrderService.SavePurchaseOrderProducts(products);
			}
			if (toDelete.Any())
			{
				var ids = toDelete.Select(x => x.PurchaseOrderProductID).ToList();
				_purchaseOrderService.DeletePurchaseOrderProduct(ids);
			}
			//if do not receive is differnt from original po we need to adjust the inventory 10.5.2025. If part of PO was received we want it to be part of inventory no matter what. 
			//If do not receive is selected, we are only not receiving the remaining products on the PO.
			//if(savedPO.IsDoNotReceive != oldPO.IsDoNotReceive)
			//{
			//	var receivedProducts = await _purchaseOrderService.GetPurchaseOrderReceiptProducts(savedPO.PurchaseOrderID);
			//	foreach(var product in receivedProducts)
			//	{
			//		if(product.QuantityAlreadyReceived != null)
			//		{
			//			string reason = !string.IsNullOrEmpty(savedPO.DoNotReceiveReason) ? savedPO.DoNotReceiveReason : "DoNotReceive Adjustment";

			//			int qty = savedPO.IsDoNotReceive ? product.QuantityAlreadyReceived * -1 : product.QuantityAlreadyReceived;
			//			ProductInventoryAdjustmentContract productAdjustmentActivity = new ProductInventoryAdjustmentContract()
			//			{
			//				AdjustedBy = _authenticationService.GetUserID(),
			//				AdjustmentDate = DateTime.UtcNow,
			//				ProductID = product.ProductID,
			//				Quantity = qty,
			//				ReasonCodeID = AdjustmentReasonConstants.DoNotReceive,
			//				ReasonCodeOther = $"{savedPO.PurchaseOrderCustomID} - {reason}"
			//			};

			//			await _productService.SaveProductAdjustment(productAdjustmentActivity);
			//		}
			//	}
			//}

			var vendor = _vendorService.GetVendor(savedPO.VendorID);
			EmailApprovers(vendor, submit);

			return new PurchaseOrderViewModel(savedPO);
		}

		public SaveResults ReceiptComplete(int purchaseOrderID, IList<PurchaseOrderProductReceiptViewModel> products)
		{
			products = products.Where(p => p.QuantityReceived > 0).ToList();
			if(!products.Any())
			{
				return new SaveResults("No purchase order product receipts to save");
			}
			var results = _purchaseOrderService.ReceiptComplete(purchaseOrderID, products.Select(p => p.ToContract()).ToList());
			var po = _purchaseOrderService.GetPurchaseOrder(purchaseOrderID);
			po.ReceivedOn = DateTime.UtcNow;
			po.ReceivedBy = _authenticationService.GetUserID();

			_purchaseOrderService.SavePurchaseOrder(po, false);

			return results;
		}

		public bool ApprovePurchaseOrder(int purchaseOrderID)
		{
			return _purchaseOrderService.ApprovePurchaseOrder(purchaseOrderID);
		}

		public bool RescindPurchaseOrder(int purchaseOrderID)
		{
			return _purchaseOrderService.RescindPurchaseOrder(purchaseOrderID);
		}

		public SaveResults SendPurchaseOrderReport(int purchaseOrderID)
		{
			var po = _purchaseOrderService.GetPurchaseOrder(purchaseOrderID);
			var vendor = _vendorService.GetVendor(po.VendorID);
			var contacts = new List<(string Name, string Email)>(){
				(vendor.PrimaryContact, vendor.PrimaryEmail),
				// (vendor.AdditionalContact1Name, vendor.AdditionalContact1Email),
				// (vendor.AdditionalContact2Name, vendor.AdditionalContact2Email),
				// (vendor.AdditionalContact3Name, vendor.AdditionalContact3Email)
			};
			var contact = contacts.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.Email));

			if(string.IsNullOrWhiteSpace(contact.Email))
			{
				return new SaveResults("Vendor has no email primary contact set");
			}

			var html = RenderPurchaseOrderReport(purchaseOrderID);
			var pdf = Pdf
				.From(html)
				.OfSize(PaperSize.A4)
				.Content();

			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(this.GetType())
				.UseMemoryCachingProvider()
				.Build();

			var model = new {
				Recipient = contact.Name,
				PurchaseOrderID = po.PurchaseOrderCustomID
			};

			var body =  engine.CompileRenderAsync("Views.PurchaseOrder.PurchaseOrderReportEmail", model).Result;
			try {
				using(var stream = new MemoryStream(pdf))
				{
					_emailService.SendEmail(
						contact.Email,
						$"MedGyn-Purchase Order {po.PurchaseOrderCustomID}",
						body, null, new Dictionary<string, Stream>() {
							{"PurchaseOrder.pdf", stream}
						}
					);
				}
			}
			catch(Exception)
			{
				return new SaveResults("Failed to send PO-Export Email");
			}

			return new SaveResults();
		}

		public List<string> GetPeachTreeReceiptsContents()
		{
			return _purchaseOrderService.GetPeachTreeReceiptsContents();
		}

		private string RenderPurchaseOrderReport(int purchaseOrderID)
		{
			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(PurchaseOrderDetailsViewModel))
				.UseMemoryCachingProvider()
				.Build();

			var countries       = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var states          = _codeService.GetCodeLookupByType(CodeTypeEnum.States);
			var illinoisCode    = _codeService.GetCodesByType(CodeTypeEnum.States).FirstOrDefault(c => c.CodeName.Equals("IL"));
			var shipMethodCodes = _codeService.GetCodesByType(CodeTypeEnum.FedExShipMethods)
				.Concat(_codeService.GetCodesByType(CodeTypeEnum.UPSShipMethods))
				.Concat(_codeService.GetCodesByType(CodeTypeEnum.OtherShipMethods));
			var po          = _purchaseOrderService.GetPurchaseOrder(purchaseOrderID);
			var pops        = _purchaseOrderService.GetPurchaseOrderReportProducts(purchaseOrderID).ToList();
			var updateUser  = _userService.GetUser(po.UpdatedBy);
			var approveUser = po.ApprovedBy.HasValue ? _userService.GetUser(po.ApprovedBy.Value) : null;
			var vendor      = _vendorService.GetVendor(po.VendorID);

			var shippingAccount = string.Empty;
			if((po.ShipCompanyType ?? 0) == (int)ShippingCompanyEnum.FedEx)
			{
				shippingAccount = _appSettings.FedExAccountNumber;
			}
			if((po.ShipCompanyType ?? 0) == (int)ShippingCompanyEnum.UPS)
			{
				shippingAccount = _appSettings.UPSAccountNumber;
			}

			var paymentTerms = vendor.PaymentTermsType == 1 ? "COD"
				: vendor.PaymentTermsType == 2 ? "Prepay"
				: $"Net Due in {vendor.PaymentTermsNetDueDays} days";

			states.TryGetValue(vendor.StateCodeID ?? -1, out var vendorStateCode);
			var vendorState = vendorStateCode?.CodeName;

			countries.TryGetValue(vendor.CountryCodeID, out var vendorCountryCode);
			var vendorCountry = vendorCountryCode?.CodeDescription;

			var shippingCharge = po.ShippingCharge ?? 0;
			var subTotal       = pops.Sum(p => p.Ext);
			var taxes          = ((vendor.StateCodeID == (illinoisCode?.CodeID ?? 0) ? _appSettings.IllinoisTax : 0))/100 * subTotal;
			var totalAmount    = shippingCharge + subTotal + taxes;

			var bytes = _env.EnvironmentName == tmEnv ? File.ReadAllBytes(tmLogo) : File.ReadAllBytes(medGynLogo);
			var file = Convert.ToBase64String(bytes);

			var model = new {
				MedGynName                = _appSettings.MedGynName,
				MedGynAddress             = _appSettings.MedGynAddress,
				MedGynCity                = _appSettings.MedGynCity,
				MedGynStateOrProvinceCode = _appSettings.MedGynStateOrProvinceCode,
				MedGynCountryAbbr         = _appSettings.MedGynCountryAbbr,
				MedGynPostalCode          = _appSettings.MedGynPostalCode,
				MedGynContactPhone        = Regex.Replace(_appSettings.MedGynContactPhone, @"(\d{3})(\d{3})(\d{4})", "($1)-$2-$3"),
				MedGynFax                 = Regex.Replace(_appSettings.MedGynFax, @"(\d{3})(\d{3})(\d{4})", "($1)-$2-$3"),
				MedGynTollFreePhone       = Regex.Replace(_appSettings.MedGynTollFreePhone, @"(\d{3})(\d{3})(\d{4})", "($1)-$2-$3"),
				MedGynEmail               = _appSettings.MedGynEmail,
				InvoiceRemittanceAddress  = _appSettings.InvoiceRemittanceAddress,
				InvoiceRemittanceMessage  = _appSettings.InvoiceRemittanceMessage,
				InvoiceFooter             = _appSettings.InvoiceFooter,
				LogoString                = file,
				PurchaseOrderID           = po.PurchaseOrderID,
				PurchaseOrderCustomID     = po.PurchaseOrderCustomID,
				SubmitDate                = po.SubmitDate?.ToString("MM/dd/yyyy"),
				VendorOrderNumber         = po.VendorOrderNumber,
				VendorName                = vendor.VendorName,
				VendorID                  = vendor.VendorCustomID,
				VendorAddress             = vendor.Address1 ?? vendor.Address2,
				VendorCity                = vendor.City,
				VendorState               = vendorState,
				VendorCountry             = vendorCountry,
				VendorZip                 = vendor.ZipCode,
				ExpectedDate              = po.ExpectedDate?.ToString("MM/dd/yyyy"),
				ShipCompany               = Enum.IsDefined(typeof(ShippingCompanyEnum), po.ShipCompanyType ?? 0)
					? (ShippingCompanyEnum)po.ShipCompanyType
					: (ShippingCompanyEnum?)null,
				ShipMethod                = shipMethodCodes.FirstOrDefault(c => c.CodeID == po.ShipChoiceCodeID)?.CodeDescription,
				ShippingAccount           = shippingAccount,
				IsPartialShipAcceptable   = po.IsPartialShipAcceptable,
				Products                  = pops,
				PaymentTerms              = paymentTerms,
				ShippingCharge            = (po.ShippingCharge ?? 0).ToString("0.00"),
				Subtotal                  = subTotal.ToString("0.00"),
				Taxes                     = taxes.ToString("0.00"),
				TotalAmount               = totalAmount.ToString("0.00"),
				VpApproved				  = po.ApprovalDate,
				Notes					  = po.Notes
			};

			return engine.CompileRenderAsync("Views.PurchaseOrder.PurchaseOrderReport", model).Result;
		}

		private void EmailApprovers(VendorContract vendor,  bool isRecipientVP)
		{
			var template = "Views.PurchaseOrder.ApprovalRequestEmail";
			var subject  = "Approval Needed for Purchase Order";
			var keys     = new List<SecurityKeyEnum>();
			if(isRecipientVP)
			{
				if(vendor.IsDomestic || vendor.IsDomesticAfaxys)
				{
					keys.Add(SecurityKeyEnum.PurchaseOrderDomesticVPApproval);
				}
				if(vendor.IsDomesticDistributor)
				{
					keys.Add(SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval);
				}
				if(vendor.IsInternational)
				{
					keys.Add(SecurityKeyEnum.PurchaseOrderInternationalVPApproval);
				}
			}

			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(ApprovalEmailViewModel))
				.UseMemoryCachingProvider()
				.Build();

			var model = new ApprovalEmailViewModel
			{
				ApproverName = "Team",
				ApproveeName = vendor.VendorName,
				Url = _appSettings.Url
			};
			var body = engine.CompileRenderAsync(template, model).Result;

			var users = _userService.GetUsersWithKey(keys.ToArray());
			var recipients = "";
			foreach (var user in users)
			{
				recipients += user.Email.Trim() + ';';
				if (users.Count == 1)
				{
					model.ApproverName = user.FirstName;
				}
			}

			if(users.Any())
				_emailService.SendEmail(recipients, subject, body);
		}
	}
}
