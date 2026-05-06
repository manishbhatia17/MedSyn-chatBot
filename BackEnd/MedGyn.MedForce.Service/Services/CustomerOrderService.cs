using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Common.Constants;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Options;
using OpenHtmlToPdf;
using static System.Net.WebRequestMethods;
using NHibernate.Linq.Functions;
using NHibernate.Linq;

namespace MedGyn.MedForce.Service.Services
{
	public class CustomerOrderService : ICustomerOrderService
	{

		private readonly ICustomerOrderRepository _customerOrderRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IProductRepository _productRepository;
		private readonly IUserRepository _userRepository;
		private readonly IAuthenticationService _authenticationService;
		private readonly ICodeService _codeService;
		private readonly IShipStationAPIService _shipStationAPIService;
		private readonly ShipStationAPISettings _shipStationAPISettings;

		public CustomerOrderService(
			ICustomerOrderRepository customerOrderRepository,
			ICustomerRepository customerRepository,
			IProductRepository productRepository,
			IAuthenticationService authenticationService,
			IUserRepository userRepository,
			ICodeService codeService,
			IShipStationAPIService shipStationAPIService,
			IOptions<ShipStationAPISettings> shipStationAPISettings
		)
		{
			_customerOrderRepository = customerOrderRepository;
			_customerRepository      = customerRepository;
			_productRepository       = productRepository;
			_userRepository          = userRepository;
			_authenticationService   = authenticationService;
			_codeService             = codeService;
			_shipStationAPIService   = shipStationAPIService;
			_shipStationAPISettings  = shipStationAPISettings.Value;
		}

		public async Task<IList<CustomerOrderListItem>> GetCustomerOrderList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var currentUserID = _authenticationService.GetUserID();
			IList<dynamic> orderItems = null;
			switch(status)
			{
				case CustomerOrderStatusEnum.OnBackOrder:
					throw new NotImplementedException();
				case CustomerOrderStatusEnum.WaitingSubmission:
					orderItems = await _customerOrderRepository.GetWaitingOnSubmissionList(search, sortCol, sortAsc, status, dateOption, currentUserID, showAll,
												showInternational, showDomesticDistributors, showDomesticNonDistributors);
					break;
				case CustomerOrderStatusEnum.WaitingManagerApproval:
					orderItems = await _customerOrderRepository.GetWaitingManagerApproval(search, sortCol, sortAsc, status, dateOption, currentUserID, showAll,
																		showInternational, showDomesticDistributors, showDomesticNonDistributors);
					break;
				case CustomerOrderStatusEnum.WaitingVPApproval:
					orderItems = await _customerOrderRepository.GetWaitingVPApproval(search, sortCol, sortAsc, status, dateOption, currentUserID, showAll,
																								showInternational, showDomesticDistributors, showDomesticNonDistributors);
					break;
				case CustomerOrderStatusEnum.ToBeFilled:
				case CustomerOrderStatusEnum.DoNotFill:
				case CustomerOrderStatusEnum.Filling:
					orderItems = await _customerOrderRepository.GetCustomerOrderFillStatusList(search, sortCol, sortAsc, status, dateOption, currentUserID, showAll,
																		showInternational, showDomesticDistributors, showDomesticNonDistributors);

					break;

				case CustomerOrderStatusEnum.ToBeShipped:
					orderItems = await _customerOrderRepository.GetShippingInfo(search, sortCol, sortAsc, status, dateOption, currentUserID, showAll,
												showInternational, showDomesticDistributors, showDomesticNonDistributors);
					break;
				case CustomerOrderStatusEnum.ToBeInvoiced:
				case CustomerOrderStatusEnum.HasBeenInvoiced:
					orderItems = await _customerOrderRepository.GetsInvoicedOrderList(search, sortCol, sortAsc, status, dateOption, currentUserID, showAll,
																		showInternational, showDomesticDistributors, showDomesticNonDistributors);
					break;
				case CustomerOrderStatusEnum.ShowMyOrders:
					orderItems = await _customerOrderRepository.GetMyOrdersList(search, sortCol, sortAsc, status, dateOption, currentUserID, showAll,
																								showInternational, showDomesticDistributors, showDomesticNonDistributors);
					break;
				default:
					return await _customerOrderRepository.GetCustomerOrderList(search, sortCol, sortAsc, status, dateOption, currentUserID, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);
			}
			
				return  orderItems.Select(s => new CustomerOrderListItem(s)).ToList();
		}

		public Task<IList<dynamic>> GetBackOrderList(string search, string sortCol, string searchColumn, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var currentUserID = _authenticationService.GetUserID();
			return _customerOrderRepository.GetBackOrderList(search, sortCol, searchColumn, sortAsc, status, dateOption, currentUserID, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);
		}

		public CustomerOrderContract GetCustomerOrder(int customerOrderID)
		{
			var model = _customerOrderRepository.GetCustomerOrder(customerOrderID);
			return new CustomerOrderContract(model);
		}

		public CustomerOrderContract GetCustomerOrderByCONumber(string customCustomerOrderNumber)
		{
			var model = _customerOrderRepository.GetCustomerOrderByCONumber(customCustomerOrderNumber);
			if (model == null)
				return null;
			return new CustomerOrderContract(model);
		}

		public CustomerOrderContract GetCustomerOrderByPO(string poNumber)
		{
			var model = _customerOrderRepository.GetCustomerOrderByPONumber(poNumber);
			if(model == null)
				return null;
			return new CustomerOrderContract(model);
		}

		public List<CustomerOrderContract> GetCustomerOrdersByCustomerId(int customerId)
		{
			var models = _customerOrderRepository.GetCustomerOrderByCustomerId(customerId);
			if (models == null || models.Count == 0)
				return new List<CustomerOrderContract>();

			return models.Select(x => new CustomerOrderContract(x)).ToList();
		}

		public CustomerOrderContract GetCustomerOrder(int customerOrderID, int currentUserID, bool showAll, bool showInternational,
			bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var model = _customerOrderRepository.GetCustomerOrder(customerOrderID, currentUserID, showAll, showInternational,
				showDomesticDistributors, showDomesticNonDistributors);
			return new CustomerOrderContract(model);
		}

		public CustomerOrderFillInfo GetCustomerOrderFillInfo(int customerOrderID)
		{
			return _customerOrderRepository.GetCustomerOrderFillInfo(customerOrderID);
		}

		public IList<PriceReconciliationContract> GetAllFillProducts(string search, string sortCol, bool sortAsc, DateTime startTime, DateTime EndTime)
        {
			PriceReconciliationContract priceReconciliationContract = new PriceReconciliationContract();
             //create a data model to replace the dynamic
             var fillProducts = _customerOrderRepository.GetAllFillProducts(search, sortCol, sortAsc, startTime, EndTime);

			return priceReconciliationContract.ListContract(fillProducts);
		}


        public IList<dynamic> GetCustomerOrderFillProducts(int customerOrderID, int? boxID)
		{
			return _customerOrderRepository.GetCustomerOrderFillProducts(customerOrderID, boxID);
		}


		public IEnumerable<CustomerOrderProductContract> GetCustomerOrderProducts(int customerOrderID)
		{
			return _customerOrderRepository.GetCustomerOrderProducts(customerOrderID).Select(x => new CustomerOrderProductContract(x));
		}

		public IEnumerable<ReportProduct> GetCustomerOrderReportProducts(int customerOrderID)
		{
			return _customerOrderRepository.GetCustomerOrderReportProducts(customerOrderID);
		}

		public Task<IList<dynamic>> GetHistoryForProduct(int customerID, int productID)
		{
			return _customerOrderRepository.GetHistoryForProduct(customerID, productID);
		}

		public CustomerOrderContract SaveCustomerOrder(CustomerOrderContract customerOrder, bool submit)
		{
			var curModel = _customerOrderRepository.GetCustomerOrder(customerOrder.CustomerOrderID);

			var customerOrderModel = customerOrder.ToModel(curModel);
			customerOrderModel.UpdatedBy = _authenticationService.GetUserID();
			customerOrderModel.UpdatedOn = DateTime.UtcNow;

			if(submit) {
				customerOrderModel.SubmitDate = DateTime.UtcNow;
			}

			if (customerOrderModel.CustomerOrderID > 0)
			{
				CustomerOrder existingOrder = _customerOrderRepository.GetCustomerOrderByPONumber(customerOrder.CustomerID, customerOrder.PONumber);
                if (existingOrder!= default && customerOrder.IsDoNotFill == false)
                {
					if(existingOrder.CustomerOrderID != customerOrderModel.CustomerOrderID && existingOrder.IsDoNotFill == false)
						throw new Exception("PO Number already exists for this customer. Please enter a new PO Number to continue.");
                }

                _customerOrderRepository.UpdateCustomerOrder(customerOrderModel);
			}
			else
			{
				if(_customerOrderRepository.GetCustomerOrderByPONumber(customerOrder.CustomerID, customerOrder.PONumber) != default)
				{
					throw new Exception("PO Number already exists for this customer. Please enter a new PO Number to continue.");
				}

				var dailyPOCount = _customerOrderRepository.GetDailyCustomerOrderCount(customerOrder.CustomerID);
				var now = DateTime.UtcNow.Date;
				if (dailyPOCount == null)
				{
					dailyPOCount = _customerOrderRepository.SaveDailyCustomerOrderCount(customerOrder.CustomerID, now, 1);
				}
				else
				{
					if(dailyPOCount.LastCreated.ToString("MMddyyyy") == now.ToString("MMddyyyy"))
						dailyPOCount.DailyCount++;
					else
					{
						dailyPOCount.DailyCount = 1;
						dailyPOCount.LastCreated = now;
					}

					_customerOrderRepository.UpdateDailyCustomerOrderCount(dailyPOCount);
				}

				var rep = _userRepository.GetRepForCustomerShippingInfo(customerOrderModel.CustomerShippingInfoID);
				customerOrderModel.CustomerOrderCustomID = $"{now:MMddyyyy}-{customerOrder.CustomerID}-{rep?.SalesRepID}-{dailyPOCount.DailyCount.ToString().PadLeft(3, '0')}";
				customerOrderModel.CreatedBy = customerOrderModel.UpdatedBy;
				customerOrderModel.CreatedOn = customerOrderModel.UpdatedOn;
				customerOrderModel = _customerOrderRepository.SaveCustomerOrder(customerOrderModel);
			}

			return new CustomerOrderContract(customerOrderModel);
		}

		public bool SaveCustomerOrderProducts(List<CustomerOrderProductContract> customerOrderProducts)
		{
			return _customerOrderRepository.SaveCustomerOrderProducts(customerOrderProducts.Select(x => x.ToModel()).ToList());
		}

		public SaveResults ApproveCustomerOrder(int customerOrderID, bool isVPApproval)
		{
			var user = _authenticationService.GetUserID();
			var results = _customerOrderRepository.ApproveCustomerOrder(customerOrderID, user, isVPApproval);
			if(results == 1)
				return new SaveResults();
			return new SaveResults("Error Approving Order");
		}

		public SaveResults ApproveCustomerFinancing(int customerOrderID)
        {
			var user = _authenticationService.GetUserID();
			var results = _customerOrderRepository.ApproveCustomerOrderFinancing(customerOrderID, user);
			if (results == 1)
				return new SaveResults();
			return new SaveResults("Error Approving Order");
		}

		public bool RescindCustomerOrder(int customerOrderID, bool canRevoke, bool canRescindFill)
		{
			var customerOrder = _customerOrderRepository.GetCustomerOrder(customerOrderID);
			var customerOrderShipments = _customerOrderRepository.GetCustomerOrderShipments(customerOrderID);
			if(customerOrderShipments.Any())
			{
				return false;
			}
			else if (customerOrder.VPApprovedBy.HasValue && canRescindFill)
			{
				return _customerOrderRepository.RescindCustomerOrderToBeFilled(customerOrderID) > 0;
			}
			else if(customerOrder.MGApprovedBy.HasValue && canRevoke)
			{
				customerOrder.MGApprovedBy = null;
				customerOrder.MGApprovedOn = null;
				_customerOrderRepository.SaveCustomerOrder(customerOrder);
				return true;
			}
			else if (customerOrder.SubmitDate.HasValue && canRevoke)
			{
				customerOrder.SubmitDate = null;
				_customerOrderRepository.SaveCustomerOrder(customerOrder);
				return true;
			}
			return false;
		}

		public bool RescindFillingCustomerOrder(int customerOrderID) {
			return _customerOrderRepository.SetFillingStatus(customerOrderID, 0, false) == 1;
		}

		public bool RescindCustomerOrderShipment(int customerOrderShipmentID, bool canRescindShip)
		{
			return _customerOrderRepository.DeleteCustomerOrderShipment(customerOrderShipmentID) > 0;
		}

		public bool DeleteCustomerOrder(int customerOrderID)
		{
			return _customerOrderRepository.DeleteCustomerOrder(customerOrderID);
		}

		public SaveResults DeleteCustomerOrderProduct(List<int> customerOrderProductIDs)
		{
			var results = _customerOrderRepository.DeleteCustomerOrderProduct(customerOrderProductIDs);
			if(results > 0)
				return new SaveResults();
			return new SaveResults("Error Deleting Product");
		}

		public String GetCustomerOrderLastShipment(int customerOrderID)
        {

			List<CustomerOrderShipment> myList = _customerOrderRepository.GetCustomerOrderShipments(customerOrderID);
			if (myList != null && myList.Count > 0)
            {
				foreach(var elem in myList)
                {
					if (elem.InvoiceNumber != null)
						return elem.InvoiceNumber;

                }

            }

			//return _customerOrderRepository.GetCustomerOrderShipments(customerOrderID).Count >= 1 ? _customerOrderRepository.GetCustomerOrderShipments(customerOrderID)[0].InvoiceNumber : "";
			return "";
        }

		public int GetCustomerOrderShipmentCount(int customerOrderID)
		{

			List<CustomerOrderShipment> myList = _customerOrderRepository.GetCustomerOrderShipments(customerOrderID);


			return myList.Where(x => !string.IsNullOrWhiteSpace(x.InvoiceNumber)).ToArray().Length;
		}

		public SaveResults FillComplete(int customerOrderID, int fillOption, int numberOfSameBoxes, int numberOfPackingSlips, List<CustomerOrderProductFillContract> data)
		{
			try {
				var co   = _customerOrderRepository.GetCustomerOrder(customerOrderID);
				var cofi = _customerOrderRepository.GetCustomerOrderFillInfo(customerOrderID);

				var shipmentModel = new CustomerOrderShipment() {
					CustomerOrderID      = co.CustomerOrderID,
					ShipCompanyType      = co.ShipCompanyType,
					ShipMethodCodeID     = co.ShipChoiceCodeID,
					ShipAccountNumber    = cofi.ShipAccountNumber,
					FillOption           = fillOption,
					NumberOfSameBoxes    = fillOption == 1 ? numberOfSameBoxes : (int?) null,
					NumberOfPackingSlips = fillOption == 1 ? numberOfPackingSlips : (int?) null,
					ShippingCharge       = (co.ShippingCharge ?? 0) + (co.InsuranceCharge ?? 0) + (co.HandlingCharge ?? 0),
					CreatedBy            = _authenticationService.GetUserID(),
					CreatedOn            = DateTime.UtcNow,
				};

				if(shipmentModel.ShippingCharge == 0)
					shipmentModel.ShippingCharge = null;

				var savedShipment = _customerOrderRepository.SaveCustomerOrderShipment(shipmentModel);

				var box = SaveShipmentBox(customerOrderID, savedShipment.CustomerOrderShipmentID);

				data.ForEach(x => x.CustomerOrderShipmentBoxID = box.CustomerOrderShipmentBoxID);
				var models = data.Where(x => x.QuantityPacked > 0).Select(x => x.ToModel());

				if(models.Any())
					_customerOrderRepository.MoveFillsIntoShipmentBoxes(customerOrderID, models);

				_customerOrderRepository.MoveBoxesIntoShipment(customerOrderID, shipmentModel.CustomerOrderShipmentID);
				_customerOrderRepository.SetFillingStatus(customerOrderID, shipmentModel.CreatedBy, false);

				var res = new SaveResults();
				res.Payload = new CustomerOrderShipmentContract(savedShipment);

				return res;
			}
			catch(Exception e)
			{
				return new SaveResults(e);
			}
		}

		public CustomerOrderShipmentBox SaveShipmentBox(int CustomerOrderID, int? CustomerOrderShipmentID)
		{
			var box = new CustomerOrderShipmentBox()
			{
				CustomerOrderID = CustomerOrderID,
				CustomerOrderShipmentID = CustomerOrderShipmentID
			};

			_customerOrderRepository.SaveCustomerOrderShipmentBox(box);

			return box;
		}

		//Remove CustomerOrderShipmentBox from CustomerOrderShipment by passing CustomerOrderShipmentBox model
		public SaveResults RemoveShipmentBox(int boxId)
		{
			SaveResults saveResults = new SaveResults();
			CustomerOrderShipmentBox box = _customerOrderRepository.GetCustomerOrderShipmentBox(boxId);
			if (box != null)
			{
				_customerOrderRepository.RemoveCustomerOrderShipmentBox(box);
			}
			else
			{ 
				saveResults.ErrorMessage = "Box does not exist";
			}

			return saveResults;
		}


		public List<int> GetBoxesForFill(int customerOrderID)
		{
			return _customerOrderRepository.GetBoxesForFill(customerOrderID);
		}

		public IEnumerable<CustomerOrderShipmentBoxContract> GetBoxesForShip(int customerOrderShipmentID)
		{
			return _customerOrderRepository.GetBoxesForShip(customerOrderShipmentID)
				.Select(x => new CustomerOrderShipmentBoxContract(x));
		}

		public CustomerOrderShipmentContract GetCustomerOrderShipment(int shipmentID)
		{
			var model = _customerOrderRepository.GetCustomerOrderShipment(shipmentID);
			return new CustomerOrderShipmentContract(model);
		}

		public CustomerOrderShipmentBoxContract GetCustomerOrderShipmentBox(int shipmentID)
		{
			var model = _customerOrderRepository.GetCustomerOrderShipmentBox(shipmentID);
			return new CustomerOrderShipmentBoxContract(model);
		}

		public SaveResults AddBox(int customerOrderID, List<CustomerOrderProductFillContract> data)
		{
			if(!data.Any(x => x.QuantityPacked > 0))
				return new SaveResults("No items were packed");

			try {
				var box = new CustomerOrderShipmentBox() { CustomerOrderID = customerOrderID };

				_customerOrderRepository.SaveCustomerOrderShipmentBox(box);

				data.ForEach(x => x.CustomerOrderShipmentBoxID = box.CustomerOrderShipmentBoxID);
				var models = data.Where(x => x.QuantityPacked > 0).Select(x => x.ToModel());

				if (models.Any())
					_customerOrderRepository.MoveFillsIntoShipmentBoxes(customerOrderID, models);

				return new SaveResults();
			}
			catch(Exception e)
			{
				return new SaveResults(e);
			}
		}

		public SaveResults UpdateBox(int customerOrderID, int boxID, List<CustomerOrderProductFillContract> data)
		{
			try
			{
				var models = data.Select(x => x.ToModel());
				_customerOrderRepository.UpdateFillBoxes(customerOrderID, boxID, models);
				return new SaveResults();
			}
			catch(Exception e)
			{
				return new SaveResults(e);
			}
		}

		public SaveResults UpdateBoxDims(int customerOrderID, int boxID, CustomerOrderShipmentBoxContract box)
		{
			try{
				var result = _customerOrderRepository.UpdateBoxDims(customerOrderID, boxID, box.ToModel());
				if(result == 1)
					return new SaveResults();
				return new SaveResults("Could not save");
			}
			catch(Exception e)
			{
				return new SaveResults(e);
			}
		}
		public SaveResults UpdateShipment(int customerOrderID, CustomerOrderShipmentContract shipment)
		{
			try{
				var result = _customerOrderRepository.UpdateShipment(customerOrderID, shipment.ToModel());
				if(result == 1)
					return new SaveResults();
				return new SaveResults("Could not save");
			}
			catch(Exception e)
			{
				return new SaveResults(e);
			}
		}

		public SaveResults UpdateShipmentInvoiceStatus(CustomerOrderShipmentContract shipment)
		{
			try{
				var result = _customerOrderRepository.UpdateShipmentInvoiceStatus(shipment.ToModel());
				if(result == 1)
					return new SaveResults();
				return new SaveResults("Could not save");
			}
			catch(Exception e)
			{
				return new SaveResults(e);
			}
		}

		[Obsolete("Need to Complete Order Now")]
		public SaveResults CompleteShipment(CustomerShippingInfoContract customerShippingInfo, int shipmentID, decimal invoiceTotal,
			List<CustomerOrderShipmentBoxContract> boxes, string carrierCode, string serviceCode, string invoiceNumber, string shippingAccount, string masterTrackingNumber, string shippingCustomerName, string poNumber)
		{
			var shippingCost = 0f;
			var trackingNumber = "";
			DateTime? deliveryDate = null;
			var labelBytes = new List<byte[]>();

			if (!string.IsNullOrEmpty(masterTrackingNumber))
			{
				trackingNumber = masterTrackingNumber;				
			}
			else
			{
				foreach (var package in boxes)
				{
					var request = CreateNewLabelRequest(carrierCode, serviceCode, package, customerShippingInfo, shippingAccount, shippingCustomerName, poNumber);
					var response = _shipStationAPIService.CreateLabel(request);
					if (response.success)
					{
						DateTime shipDate;
						shippingCost += response.shipmentCost;
						if (!trackingNumber.IsNullOrEmpty())
							trackingNumber += ";";
						trackingNumber += response.trackingNumber;
						if (DateTime.TryParse(response.shipDate, out shipDate))
							deliveryDate = shipDate;
						labelBytes.Add(Convert.FromBase64String(response.labelData));
					}
					else
					{
						return new SaveResults(response.errorMessage);
					}
				}
			}
			_customerOrderRepository.CompleteShipment(shipmentID, invoiceNumber, trackingNumber, deliveryDate, invoiceTotal);

			var res = new SaveResults();

			if (string.IsNullOrEmpty(masterTrackingNumber))
			{
				using (var memoryStream = new MemoryStream())
				{
					using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
					{
						var box = 1;
						foreach (var image in labelBytes)
						{
							var zipEntry = archive.CreateEntry($"{invoiceNumber}-{box++}.pdf");
							using (var entryStream = new MemoryStream(image))
							using (var zipEntryStream = zipEntry.Open())
								entryStream.CopyTo(zipEntryStream);
						}
					}

					var bytes = memoryStream.ToArray();
					res.Payload = new { ZipFile = Convert.ToBase64String(bytes), Filename = $"{invoiceNumber}_Labels" };
				}
			}

			return res;
		}

		public void CompleteShipment(int shipmentID, int customerOrderId, string invoiceNumber, string trackingNumber, DateTime? deliveryDate, decimal invoiceTotal)
        {
			_customerOrderRepository.CompleteShipment(shipmentID, invoiceNumber, trackingNumber, deliveryDate, invoiceTotal);
			_customerOrderRepository.SetShipmentStatus(customerOrderId, _authenticationService.GetUserID());
		}

		public IEnumerable<InvoiceProduct> GetInvoiceProducts(int shipmentID)
		{
			return _customerOrderRepository.GetInvoiceProducts(shipmentID);
		}

		public IList<PeachTreeInvoiceContract> GetPeachTreeInvoiceExportByBatchId(int BatchId)
		{
			IList<dynamic> results = _customerOrderRepository.GetPreviousPeachtreeInvoicesByBatchId(BatchId);
			List<PeachTreeInvoiceContract> invoices = results.Select(result => new PeachTreeInvoiceContract(((IDictionary<string, object>)result).ToDictionary(x => x.Key, x => x.Value))).ToList();
			return invoices;
		}

		public IList<dynamic> GetPreviousPeachTreeInvoiceList(int TopResults = 10)
		{
			return _customerOrderRepository.GetPreviousePeachtreeInvoices(TopResults);
		}
		public List<PeachTreeInvoiceContract> GetPeachTreeInvoiceContents()
		{
			var results = _customerOrderRepository.GetPeachTreeInvoiceContents();
			var invoices = results.Select(result => new PeachTreeInvoiceContract(((IDictionary<string, object>)result).ToDictionary(x => x.Key, x => x.Value))).ToList();
			//invoices = invoices.Where(x => !String.IsNullOrEmpty(x.Amount)).OrderBy(o => o.CustomerID).ThenBy(tb => tb.InvoiceCMNumber).ThenBy(tb => tb.TaxType).ThenBy(tb => tb.InvoiceCMDistribution).ToList();
			
			invoices = invoices.Where(x => !String.IsNullOrEmpty(x.Amount)).OrderBy(o => o.sortSequence).ToList();
			
			return invoices;
		}

		public IList<PeachTreeInvoiceContract> GetPreviousPeachTreeInvoicesByDate(DateTime StartDate, DateTime EndDate)
		{
			string startDate = StartDate.ToString("yyyy-MM-dd 00:00:00.000");
			string endDate = EndDate.ToString("yyyy-MM-dd 23:59:59.999");
			List<PeachTreeInvoiceContract> invoices = new List<PeachTreeInvoiceContract>();
			//TODO get date
			IList<dynamic> invoiceDates = _customerOrderRepository.GetPreviousPeachTreeInvoicesByDate(startDate, endDate);
			foreach(var invoice in invoiceDates)
			{
				IList<dynamic> results = _customerOrderRepository.GetPreviousPeachtreeInvoicesByBatchId(invoice.AccountingExportBatchID);
				invoices.AddRange(results.Select(s => new PeachTreeInvoiceContract(((IDictionary<string, object>)s).ToDictionary(x => x.Key, x => x.Value))).ToList());
			}

			return invoices;
		}

		public void MarkOrderAsFilling(int customerOrderID)
		{
			_customerOrderRepository.SetFillingStatus(customerOrderID, _authenticationService.GetUserID(), true);
		}
		public int CompleteNonFedexNonUPSShipment(int shipmentID, string invoiceNumber, string trackingNumber, DateTime? deliveryDate, decimal invoiceTotal)
		{
			_customerOrderRepository.SetShipmentStatus(shipmentID, _authenticationService.GetUserID());
			return _customerOrderRepository.CompleteShipment(shipmentID, invoiceNumber, trackingNumber, deliveryDate, invoiceTotal);
		}

		public ShippingOrderRequest CreateShippingOrderRequest(string carrierCode, string serviceCode, CustomerOrderShipmentBoxContract package,
			CustomerShippingInfoContract customerShippingInfo, string customerAccount, string shippingCustomerName, string customerOrderNumber, string customCustomerOrderNumber, string poNumber, string boxCount)
        {
			var customer = _customerRepository.GetCustomer(customerShippingInfo.CustomerID);
			var countries = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var states = _codeService.GetCodeLookupByType(CodeTypeEnum.States);
			var dimLookup = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipDimensionsUnit);
			var weightLookup = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipWeightUnit);

			var dimUnits = dimLookup[package.DimensionUnitCodeID ?? 0].CodeName;
			var weightModel = new Weight();
			var weightUnit = weightLookup[package.WeightUnitCodeID ?? 0];
			var shipCountryCode = CountryCodeLookup.Lookup(countries[customerShippingInfo.CountryCodeID].CodeName);
			if (weightUnit?.CodeName == "LB")
			{
				weightModel.units = "ounces";
				weightModel.value = (package.Weight ?? 0) * 16;
			}
			if (weightUnit?.CodeName == "KG")
			{
				weightModel.units = "grams";
				weightModel.value = (package.Weight ?? 0) * 1000;
			}
			var orderRequest = new ShippingOrderRequest()
			{
				orderNumber = customerOrderNumber,
				orderKey = customerOrderNumber,
				carrierCode = carrierCode,
				packageCode = "package",
				serviceCode = serviceCode,
				dimensions = new Dimensions()
				{
					height = package.Depth ?? 0,
					width = package.Width ?? 0,
					length = package.Length ?? 0,
					units = dimUnits
				},
				weight = weightModel,
				orderDate = DateTime.Today.ToString(),
				shipDate = DateTime.Today.ToString(),
				orderStatus = "awaiting_shipment",
				billTo = new Address()
				{
					street1 = customer.Address1,
					city = customer.City,
					company = customerShippingInfo.Name,
					state = states[customer.StateCodeID ?? 0]?.CodeName,
					postalCode = customer.ZipCode,
					name = String.IsNullOrWhiteSpace(shippingCustomerName) ? customer.CustomerName : shippingCustomerName,
					country = CountryCodeLookup.Lookup(countries[customer.CountryCodeID].CodeName),
					phone = customer.PrimaryPhone,
					residential = false
				},
				shipTo = new Address()
				{
					street1 = customerShippingInfo.Address,
					street2 = customerShippingInfo.Address2,
					city = customerShippingInfo.City,
					company = customerShippingInfo.Name,
					state = states[customerShippingInfo.StateCodeID ?? 0]?.CodeName,
					postalCode = customerShippingInfo.ZipCode ?? "",
					name = String.IsNullOrWhiteSpace(shippingCustomerName) ? customer.CustomerName : shippingCustomerName,
					country = shipCountryCode,
					phone = "", //customer.PrimaryPhone,
					residential=false
				}
			};

			orderRequest.advancedOptions = new AdvancedOptions
			{
				customField1 = carrierCode == ShippingCarrierCodes.UpsCarrierCode ? poNumber : customCustomerOrderNumber,
				customField2 = boxCount,
				customField3 = carrierCode == ShippingCarrierCodes.FedExCarrierCode ? poNumber : ""
			};

			if (!customerAccount.IsNullOrEmpty())
			{
				orderRequest.advancedOptions.billToParty = "third_party";
				orderRequest.advancedOptions.billToAccount = customerAccount;
				orderRequest.advancedOptions.billToPostalCode = customerShippingInfo.ZipCode;
				orderRequest.advancedOptions.billToCountryCode = shipCountryCode;

			}
			if (shipCountryCode != "US")
			{

				orderRequest.internationalOptions = new InternationalOptions
				{
					contents = "merchandise",
					customsItems = new List<CustomsItem>()
				};

				orderRequest.internationalOptions.customsItems = GetBoxFillContentsForCustoms(package.CustomerOrderID, package.CustomerOrderShipmentBoxID);
			}

			return orderRequest;
		}

		public List<CustomsItem> GetBoxFillContentsForCustoms(int CustomerOrderID, int CustomerOrderBoxFillID)
		{
			List<CustomsItem> contents = new List<CustomsItem>();

			var orderProducts = GetCustomerOrderProducts(CustomerOrderID).ToList();
			var fills = _customerOrderRepository.GetShipmentBoxFillInfo(CustomerOrderBoxFillID);

			foreach (var fill in fills)
			{
					var orderProduct = orderProducts.Where(p => p.CustomerOrderProductID == fill.CustomerOrderProductID).FirstOrDefault();
				if (orderProduct != null)
				{
					var product = _productRepository.GetProduct(orderProduct.ProductID);
					contents.Add(new CustomsItem
					{
						countryOfOrigin = "US",
						quantity = fill.QuantityPacked,
						value = orderProduct.Price * fill.QuantityPacked,
						customsItemId = product.ProductID.ToString(),
						description = product.Description.IsNullOrEmpty() ? "Medical equipment/supplies" : product.Description
					});
				}
			}

			return contents;
		}

		private CreateLabelRequest CreateNewLabelRequest(string carrierCode, string serviceCode, CustomerOrderShipmentBoxContract package,
			CustomerShippingInfoContract customerShippingInfo, string customerAccount, string shippingCustomerName, string poNumber)
		{
			var customer     = _customerRepository.GetCustomer(customerShippingInfo.CustomerID);
			var countries    = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries);
			var states       = _codeService.GetCodeLookupByType(CodeTypeEnum.States);
			var dimLookup    = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipDimensionsUnit);
			var weightLookup = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipWeightUnit);

			var dimUnits = dimLookup[package.DimensionUnitCodeID ?? 0].CodeName;
			var weightModel = new Weight();
			var weightUnit = weightLookup[package.WeightUnitCodeID ?? 0];
			var shipCountryCode = CountryCodeLookup.Lookup(countries[customerShippingInfo.CountryCodeID].CodeName);
			if (weightUnit?.CodeName == "LB")
			{
				weightModel.units = "ounces";
				weightModel.value = (package.Weight ?? 0) * 16;
			}
			if (weightUnit?.CodeName == "KG")
			{
				weightModel.units = "grams";
				weightModel.value = (package.Weight ?? 0) * 1000;
			}
			var labelRequest = new CreateLabelRequest
			{
				carrierCode = carrierCode,
				packageCode = "package",
				serviceCode = serviceCode,
				dimensions = new Dimensions()
				{
					height = package.Depth ?? 0,
					width  = package.Width ?? 0,
					length = package.Length ?? 0,
					units  = dimUnits
				},
				weight = weightModel,
				shipDate = DateTime.Today.ToString(),
				
				//shipFrom = new Address()
				//{
				//	street1    = _shipStationAPISettings.ShipFromAddress,
				//	city       = _shipStationAPISettings.ShipFromCity,
				//	company    = _shipStationAPISettings.ShipFromName,
				//	state      = _shipStationAPISettings.ShipFromState,
				//	postalCode = _shipStationAPISettings.ShipFromZip,
				//	name       = _shipStationAPISettings.ShipFromName,
				//	country    = _shipStationAPISettings.ShipFromCountry,
				//	phone      = _shipStationAPISettings.ShipFromContactPhone
				//},
				//shipTo = new Address()
				//{
				//	street1    = customerShippingInfo.Address,
				//	street2    = customerShippingInfo.Address2,
				//	city       = customerShippingInfo.City,
				//	company    = customerShippingInfo.Name,
				//	state      = states[customerShippingInfo.StateCodeID ?? 0]?.CodeName,
				//	postalCode = customerShippingInfo.ZipCode ?? "",
				//	name       = String.IsNullOrWhiteSpace(shippingCustomerName) ? customer.CustomerName : shippingCustomerName,
				//	country    = shipCountryCode,
				//	phone      = customer.PrimaryPhone,
				//}
			};

			//labelRequest.advancedOptions = new AdvancedOptions
			//{
			//	customField1 = poNumber
			//};

			//if (!customerAccount.IsNullOrEmpty())
			//{
			//	labelRequest.advancedOptions.billToParty = "third_party";
			//	labelRequest.advancedOptions.billToAccount = customerAccount;
			//	labelRequest.advancedOptions.billToPostalCode = customerShippingInfo.ZipCode;
			//	labelRequest.advancedOptions.billToCountryCode = shipCountryCode;

			//}
			if (shipCountryCode != "US")
			{
				var fills = _customerOrderRepository.GetShipmentBoxFillInfo(package.CustomerOrderShipmentBoxID);
				var orderProducts = GetCustomerOrderProducts(package.CustomerOrderID).ToList();
				labelRequest.internationalOptions = new InternationalOptions
				{
					contents = "merchandise",
					customsItems = new List<CustomsItem> ()
				};
				foreach (var fill in fills)
				{
					var orderProduct = orderProducts.Where(p => p.CustomerOrderProductID == fill.CustomerOrderProductID).FirstOrDefault();
					if (orderProduct != null)
					{
						var product = _productRepository.GetProduct(orderProduct.ProductID);
						labelRequest.internationalOptions.customsItems.Add(new CustomsItem
						{
							countryOfOrigin = "US",
							quantity        = fill.QuantityPacked,
							value           = orderProduct.Price * fill.QuantityPacked,
							customsItemId   = product.ProductID.ToString(),
							description     = product.Description.IsNullOrEmpty() ? "Medical equipment/supplies" : product.Description
						});
					}
				}
			}

			return labelRequest;
		}

        public Task<IList<dynamic>> GetArchivedInvoiceList(string search, string sortCol, bool sortAsc, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
        {
			var currentUserID = _authenticationService.GetUserID();
			return _customerOrderRepository.GetArchivedInvoiceList(search, sortCol, sortAsc, currentUserID, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);
		}

		public Task<IList<dynamic>> GetInvoiceActivityList(string search, string sortCol, bool sortAsc, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors, DateTime startDate, DateTime endDate)
		{
			var currentUserID = _authenticationService.GetUserID();
			return _customerOrderRepository.GetInvoiceActivity(search, sortCol, sortAsc, currentUserID, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors, startDate, endDate);
		}

		public Task<IList<dynamic>> GetFilteredProductShippedList(string search, string sortCol, bool sortAsc, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var currentUserID = _authenticationService.GetUserID();
			Task<IList<dynamic>> output = _customerOrderRepository.GetFilteredProductShippedList(search, sortCol, sortAsc, currentUserID, showAll,
				showInternational, showDomesticDistributors, showDomesticNonDistributors);
			return output;
		}
	}
}
