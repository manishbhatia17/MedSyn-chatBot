using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Interfaces
{
	public interface IPurchaseOrderRepository
	{
		IList<dynamic> GetPurchaseOrderList(string search, string sortCol, bool sortAsc, PurchaseOrderStatusEnum status, DateRangeEnum timeframe,
			bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors, int userId);
		PurchaseOrder GetPurchaseOrder(int purchaseOrderID);
		PurchaseOrder GetPurchaseOrder(int purchaseOrderID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		IList<OpenPOModel> GetOpenProductPORequests(int ProductId);
		Task<IList<dynamic>> GetPurchaseOrderReceiptProducts(int purchaseOrderID);
		IEnumerable<dynamic> GetPurchaseOrderProducts(int purchaseOrderID);
		IEnumerable<dynamic> GetPurchaseOrderReportProducts(int purchaseOrderID);
		PurchaseOrderReceiveInfo GetPurchaseOrderReceiveInfo(int purchaseOrderID);
		IList<dynamic> GetHistoryForProduct(int productID);
		PurchaseOrder SavePurchaseOrder(PurchaseOrder purchaseOrder);
		bool UpdatePurchaseOrder(PurchaseOrder purchaseOrder);
		bool DeletePurchaseOrder(PurchaseOrder purchaseOrder);
		Task SavePurchaseOrderProducts(List<PurchaseOrderProduct> purchaseOrderProducts);
		int SavePurchaseOrderReceipt(List<PurchaseOrderProductReceipt> purchaseOrderProductReceipts);
		bool DeletePurchaseOrderProduct(List<int> purchaseOrderProductIDs);
		bool ApprovePurchaseOrder(int purchaseOrderID, int userID);
		IList<dynamic> GetPeachTreeReceiptsContents();
		DailyPurchaseOrderCount GetDailyPurchaseOrderCount(int vendorID);
		DailyPurchaseOrderCount SaveDailyPurchaseOrderCount(int vendorID, DateTime now, int count);
		void UpdateDailyPurchaseOrderCount(DailyPurchaseOrderCount count);
	}
}
