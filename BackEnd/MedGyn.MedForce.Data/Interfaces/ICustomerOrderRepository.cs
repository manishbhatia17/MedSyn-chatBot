using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Interfaces
{
	public interface ICustomerOrderRepository
	{
		Task<IList<CustomerOrderListItem>> GetCustomerOrderList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);

		Task<IList<dynamic>> GetBackOrderList(string search, string sortCol, string SearchProductID, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		Task<IList<dynamic>> GetWaitingOnSubmissionList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		Task<IList<dynamic>> GetShippingInfo(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
	int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		Task<IList<dynamic>> GetWaitingManagerApproval(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
	int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		Task<IList<dynamic>> GetCustomerOrderFillStatusList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
	int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		CustomerOrder GetCustomerOrder(int customerOrderID);
		CustomerOrder GetCustomerOrder(int customerOrderID, int currentUserID, bool showAll, bool showInternational,
			bool showDomesticDistributors, bool showDomesticNonDistributors);
		CustomerOrder GetCustomerOrderByPONumber(string poNumber);
		CustomerOrder GetCustomerOrderByCONumber(string customCustomerOrderNumber);
		List<CustomerOrder> GetCustomerOrderByCustomerId(int customerId);
		Task<IList<dynamic>> GetMyOrdersList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		Task<IList<dynamic>> GetsInvoicedOrderList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		CustomerOrderFillInfo GetCustomerOrderFillInfo(int customerOrderID);
		IList<dynamic> GetAllFillProducts(string search, string sortCol, bool sortAsc, DateTime startTime, DateTime EndTime);
		Task<IList<dynamic>> GetWaitingVPApproval(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
	int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);

		IList<dynamic> GetCustomerOrderFillProducts(int customerOrderID, int? boxID);
		IEnumerable<dynamic> GetCustomerOrderProducts(int customerOrderID);
		IEnumerable<ReportProduct> GetCustomerOrderReportProducts(int customerOrderID);
		Task<IList<dynamic>> GetHistoryForProduct(int customerID, int productID);
		CustomerOrder SaveCustomerOrder(CustomerOrder customerOrder);
		bool UpdateCustomerOrder(CustomerOrder customerOrder);
		CustomerOrder GetCustomerOrderByPONumber(int customerId, string poNumber);
		bool SaveCustomerOrderProducts(List<CustomerOrderProduct> customerOrderProducts);
		int RescindCustomerOrderToBeFilled(int customerOrderID);
		bool DeleteCustomerOrder(int customerOrderID);
		int DeleteCustomerOrderShipment(int shipmentID);
		int DeleteCustomerOrderProduct(List<int> CustomerOrderProductIDs);
		int ApproveCustomerOrder(int customerOrderID, int userID, bool isVPApproval);
		int ApproveCustomerOrderFinancing(int customerOrderID, int userID);
		DailyCustomerOrderCount GetDailyCustomerOrderCount(int customerID);
		DailyCustomerOrderCount SaveDailyCustomerOrderCount(int customerID, DateTime now, int count);
		void UpdateDailyCustomerOrderCount(DailyCustomerOrderCount count);
		CustomerOrderShipment SaveCustomerOrderShipment(CustomerOrderShipment model);
		CustomerOrderShipmentBox SaveCustomerOrderShipmentBox(CustomerOrderShipmentBox model);
		int MoveFillsIntoShipmentBoxes(int customerOrderID, IEnumerable<CustomerOrderProductFill> model);
		List<int> GetBoxesForFill(int customerOrderID);
		IEnumerable<CustomerOrderShipmentBox> GetBoxesForShip(int customerOrderShipmentID);
		CustomerOrderShipment GetCustomerOrderShipment(int shipmentID);
		List<CustomerOrderShipment> GetCustomerOrderShipments(int customerOrderID);
		CustomerOrderShipmentBox GetCustomerOrderShipmentBox(int boxID);
		IEnumerable<CustomerOrderProductFill> GetShipmentBoxFillInfo(int shipmentBoxId);
		int MoveBoxesIntoShipment(int customerOrderID, int customerOrderShipmentID);
		int UpdateFillBoxes(int customerOrderID, int boxID, IEnumerable<CustomerOrderProductFill> models);
		int UpdateShipment(int customerOrderID, CustomerOrderShipment shipment);
		int UpdateShipmentInvoiceStatus(CustomerOrderShipment shipment);
		void RemoveCustomerOrderShipmentBox(CustomerOrderShipmentBox model);
		int UpdateBoxDims(int customerOrderID, int boxID, CustomerOrderShipmentBox box);
		int CompleteShipment(int customerOrderShipmentID, string invoiceNumber, string trackingNumber, DateTime? deliveryDate, decimal invoiceTotal);
		IEnumerable<InvoiceProduct> GetInvoiceProducts(int shipmentID);
		IList<dynamic> GetPreviousePeachtreeInvoices(int top = 10);
		IList<dynamic> GetPeachTreeInvoiceContents();
		int SetFillingStatus(int customerOrderID,int currentUserId, bool isFilling);
		int SetShipmentStatus(int customerOrderID, int currentUserId);

        Task<IList<dynamic>> GetArchivedInvoiceList(string search, string sortCol, bool sortAsc,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		Task<IList<dynamic>> GetInvoiceActivity(string search, string sortCol, bool sortAsc, int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors, DateTime startDate, DateTime endDate);

		Task<IList<dynamic>> GetFilteredProductShippedList(string search, string sortCol, bool sortAsc, int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);

		IList<dynamic> GetPreviousPeachtreeInvoicesByBatchId(int BatchId);
		IList<dynamic> GetPeachTreeInvoiceTaxByBatchId(int BatchId);
		IList<dynamic> GetPeachTreeInvoiceShippingByBatchId(int BatchId);
		IList<dynamic> GetPeachTreeInvoiceCreditCardFeeByBatchId(int BatchId);
		IList<dynamic> GetPreviousPeachTreeInvoicesByDate(string StartDate, string EndDate);

	}
}
