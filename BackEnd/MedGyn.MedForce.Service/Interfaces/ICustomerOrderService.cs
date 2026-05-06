using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface ICustomerOrderService
	{
		Task<IList<CustomerOrderListItem>> GetCustomerOrderList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		Task<IList<dynamic>> GetBackOrderList(string search, string sortCol, string searchColumn, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		CustomerOrderContract GetCustomerOrder(int customerOrderID);
		CustomerOrderContract GetCustomerOrder(int customerOrderID, int currentUserID, bool showAll, bool showInternational,
			bool showDomesticDistributors, bool showDomesticNonDistributors);
		CustomerOrderContract GetCustomerOrderByCONumber(string customCustomerOrderNumber);
		List<CustomerOrderContract> GetCustomerOrdersByCustomerId(int customerId);
		CustomerOrderContract GetCustomerOrderByPO(string poNumber);
		CustomerOrderFillInfo GetCustomerOrderFillInfo(int customerOrderID);
		IList<dynamic> GetCustomerOrderFillProducts(int customerOrderID, int? boxID);
		IEnumerable<CustomerOrderProductContract> GetCustomerOrderProducts(int customerOrderID);
		IEnumerable<ReportProduct> GetCustomerOrderReportProducts(int customerOrderID);
		Task<IList<dynamic>> GetHistoryForProduct(int customerID, int productID);
		CustomerOrderContract SaveCustomerOrder(CustomerOrderContract customerOrder, bool submit);
		bool SaveCustomerOrderProducts(List<CustomerOrderProductContract> customerOrderProducts);
		bool DeleteCustomerOrder(int customerOrderID);
		SaveResults DeleteCustomerOrderProduct(List<int> customerOrderProductIDs);
		SaveResults ApproveCustomerOrder(int customerOrderID, bool isVPApproval);
		SaveResults ApproveCustomerFinancing(int customerOrderID);
		bool RescindCustomerOrder(int customerOrderID, bool canRevoke, bool canRescindFill);
		bool RescindFillingCustomerOrder(int customerOrderID);
		bool RescindCustomerOrderShipment(int customerOrderShipmentID, bool canRescindShip);
		IList<PriceReconciliationContract> GetAllFillProducts(string search, string sortCol, bool sortAsc, DateTime startTime, DateTime EndTime);

        SaveResults FillComplete(int customerOrderID, int fillOption, int numberOfSameBoxes, int numberOfPackingSlips, List<CustomerOrderProductFillContract> data);
		List<int> GetBoxesForFill(int customerOrderID);
		IEnumerable<CustomerOrderShipmentBoxContract> GetBoxesForShip(int customerOrderShipmentID);
		CustomerOrderShipmentBox SaveShipmentBox(int CustomerOrderID, int? CustomerOrderShipmentID);
		CustomerOrderShipmentContract GetCustomerOrderShipment(int shipmentID);
		CustomerOrderShipmentBoxContract GetCustomerOrderShipmentBox(int boxID);
		List<CustomsItem> GetBoxFillContentsForCustoms(int CustomerOrderID, int CustomerOrderBoxFillID);
		SaveResults AddBox(int customerOrderID, List<CustomerOrderProductFillContract> data);
		SaveResults UpdateBox(int customerOrderID, int boxID, List<CustomerOrderProductFillContract> data);
		SaveResults UpdateShipment(int customerOrderID, CustomerOrderShipmentContract shipment);
		SaveResults RemoveShipmentBox(int boxId);
		SaveResults UpdateShipmentInvoiceStatus(CustomerOrderShipmentContract shipment);
		SaveResults UpdateBoxDims(int customerOrderID, int boxID, CustomerOrderShipmentBoxContract box);
		ShippingOrderRequest CreateShippingOrderRequest(string carrierCode, string serviceCode, CustomerOrderShipmentBoxContract package,
			CustomerShippingInfoContract customerShippingInfo, string customerAccount, string shippingCustomerName, string customerOrderNumber, string customCustomerOrderNumber, string poNumber, string boxCount);
		SaveResults CompleteShipment(CustomerShippingInfoContract customerShippingInfo, int shipmentID, decimal invoiceTotal,
			List<CustomerOrderShipmentBoxContract> boxes, string carrierCode, string serviceCode, string invoiceNumber, string shippingAccount, string masterTrackingNumber, string shippingCustomerName, string PONumber);
		void CompleteShipment(int shipmentID, int customerOrderId, string invoiceNumber, string trackingNumber, DateTime? deliveryDate, decimal invoiceTotal);
		IEnumerable<InvoiceProduct> GetInvoiceProducts(int shipmentID);
		IList<dynamic> GetPreviousPeachTreeInvoiceList(int TopResults = 10);
		List<PeachTreeInvoiceContract> GetPeachTreeInvoiceContents();
		void MarkOrderAsFilling(int customerOrderID);
		int CompleteNonFedexNonUPSShipment(int shipmentID, string invoiceNumber, string trackingNumber, DateTime? deliveryDate, decimal invoiceTotal);
		Task<IList<dynamic>> GetArchivedInvoiceList(string search, string sortCol, bool sortAsc,
			bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		Task<IList<dynamic>> GetInvoiceActivityList(string search, string sortCol, bool sortAsc, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors, DateTime startDate, DateTime endDate);
		Task<IList<dynamic>> GetFilteredProductShippedList(string search, string sortCol, bool sortAsc,
			bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
        String GetCustomerOrderLastShipment(int customerOrderID);
		int GetCustomerOrderShipmentCount(int customerOrderID);
		IList<PeachTreeInvoiceContract> GetPeachTreeInvoiceExportByBatchId(int BatchId);
		IList<PeachTreeInvoiceContract> GetPreviousPeachTreeInvoicesByDate(DateTime StartDate, DateTime EndDate);

	}
}
