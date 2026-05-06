using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Facade.ViewModels.CustomerOrder;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface ICustomerOrderFacade
	{
		Task<CustomerOrderListViewModel> GetCustomerOrderListViewModel(SearchCriteriaViewModel sc, CustomerOrderStatusEnum status, DateRangeEnum dateOption);
		Task<BackOrderListViewModel> GetBackOrderListViewModel(SearchCriteriaViewModel sc, CustomerOrderStatusEnum status, DateRangeEnum dateOption);
		CustomerOrderDetailsViewModel GetCustomerOrderDetails(int customerOrderID, int? customerID);
		CustomerOrderFillViewModel GetCustomerOrderFill(int customerOrderID, int? boxID);
		CustomerOrderShipViewModel GetCustomerOrderShip(int customerOrderID, int shipmentID, int? boxID);
		bool CanManagerApproveOrder(int customerOrderId);
		bool CanVpApproveOrder(int customerOrderId);
		Task<CustomerOrderViewModel> CreateCustomerOrderFromFile(string File);

        Task<CustomerOrderViewModel> SaveCustomerOrder(CustomerOrderViewModel customerOrder, bool submit);
		Task<OrderProductsViewModel> GetCustomerOrderHistoryForProduct(int productID, int customerID);
		SaveResults ApproveCustomerOrder(int customerOrderID, bool isVPApproval);
		SaveResults ApproveCustomerFinancing(int customerOrderID);
		bool RescindCustomerOrder(int customerOrderID);
		bool RescindFillingCustomerOrder(int customerOrderID);
		bool RescindCustomerOrderShipment(int customerOrderShipmentID);
		bool DeleteCustomerOrder(int customerOrderID);
		PriceReconciliationListViewModel GetPriceReconciliationList(SearchCriteriaViewModel searchCriteriaViewModel, DateTime startTime, DateTime EndTime);
		byte[] ExportPriceReconciliationExcel(SearchCriteriaViewModel searchCriteriaViewModel, DateTime startTime, DateTime EndTime);

        SaveResults FillComplete(int customerOrderID, CustomerOrderProductFillCompleteViewModel data);
		SaveResults AddBox(int customerOrderID, CustomerOrderProductFillCompleteViewModel data);
		SaveResults RemoveShipmentBox(int boxID);
		SaveResults AddAnotherShippingBox(int customerOrderID, int? CustomerOrderShipmentID);
		SaveResults UpdateBox(int customerOrderID, int boxID, CustomerOrderProductFillCompleteViewModel data);
		SaveResults UpdateBoxDims(int customerOrderID, int boxID, CustomerOrderShipViewModel ship);
		string GetRateQuote(int customerOrderID, int shipmentID, int shippingCompany, int shippingMethod, CustomerOrderShipBoxViewModel curBox);
		SaveResults CompleteShipment(int customerOrderID, int shipmentID, CustomerOrderShipViewModel ship);
		SaveResults CreateOrder(int customerOrderID, int shipmentID, CustomerOrderShipViewModel ship);
		string GetCustomerOrderReportHtml(int customerOrderID, string documentTitle);
		byte[] GetCustomerOrderReportPdf(int customerOrderID, string documentTitle);
		byte[] GetInvoice(int shiptmentID);
		string GetInvoiceHtml(int shiptmentID);
		IList<PeachtreeInvoiceListViewModel> GetPreviousPeachTreeInvoiceList(int TopResults = 10);
		byte[] GetPreviousPeachTreeInvoicesByDate(PeachTreeDateSearchDTO DateSearch);
		byte[] GetPeachTreeInvoiceContents();
		byte[] GetPackingSlipPdf(int shiptmentID, bool? generateMultiplePackingSlip);
		string GetPackingSlipHtml(int shiptmentID, bool? generateMultiplePackingSlip);
		SaveResults SendInvoice(int shiptmentID);
		void MarkOrderAsFilling(int customerOrderID);
		Task<ArchivedInvoiceListViewModel> GetArchivedInvoiceListViewModel(SearchCriteriaViewModel sc);
		Task<InvoiceActivityListViewModel> GetInvoiceActivtyViewModel(SearchCriteriaViewModel sc, DateTime StartDate, DateTime EndDate);
		Task<byte[]> GetInvoiceActivtyExport(SearchCriteriaViewModel sc, DateTime StartDate, DateTime EndDate);
		Task<IList<dynamic>> GetFilteredProductShippedViewModel(SearchCriteriaViewModel searchCriteria);
		Task<ProductShippedListViewModel> ConvertFilteredListForDisplay(SearchCriteriaViewModel searchCriteria, IList<dynamic> results);
		Task<byte[]> ExportProductListExcel(SearchCriteriaViewModel searchCriteria, int type, CustomerOrderStatusEnum status, DateRangeEnum dateOption);
		byte[] GetPeachTreeInvoiceExportByBatchId(int BatchId);
		void SetCustomerOrderToDoNotFill(int customerOrderID, DoNotFillViewModel doNotFillViewModel);
	}
}
