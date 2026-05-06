using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Facade.ViewModels;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface IPurchaseOrderFacade
	{
		PurchaseOrderListViewModel GetPurchaseOrderListViewModel(SearchCriteriaViewModel sc, PurchaseOrderStatusEnum status, DateRangeEnum timeframe);
		PurchaseOrderDetailsViewModel GetPurchaseOrderDetails(int purchaseOrderID, int? productID, int? priVendorID);
		Task<PurchaseOrderReceiveViewModel> GetPurchaseOrderReceipt(int purchaseOrderID);
		bool CanEditOrder(PurchaseOrderViewModel purchaseOrder);
		bool CanApproveOrder(int purchaseOrderId);
		Task<PurchaseOrderViewModel> SavePurchaseOrder(PurchaseOrderViewModel purchaseOrder, bool submit);
		bool DeletePurchaseOrder(int purchaseOrderID);
		SaveResults ReceiptComplete(int purchaseOrderID, IList<PurchaseOrderProductReceiptViewModel> products);
		OrderProductsViewModel GetPurchaseOrderHistoryForProduct(int productID);
		bool ApprovePurchaseOrder(int purchaseOrderID);
		bool RescindPurchaseOrder(int purchaseOrderID);
		SaveResults SendPurchaseOrderReport(int purchaseOrderID);
		List<string> GetPeachTreeReceiptsContents();
	}
}
