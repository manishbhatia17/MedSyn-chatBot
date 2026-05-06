using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IPurchaseOrderService
	{

		IList<dynamic> GetPurchaseOrderList(string search, string sortCol, bool sortAsc, PurchaseOrderStatusEnum status, DateRangeEnum timeframe,
			bool showall, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors, int userId);
		IList<dynamic> GetHistoryForProduct(int productID);
		PurchaseOrderContract GetPurchaseOrder(int purchaseOrderID);
		PurchaseOrderContract GetPurchaseOrder(int purchaseOrderID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors);
		List<PurchaseOrderContract> GetOpenPO(int productID);
		IEnumerable<PurchaseOrderProductContract> GetPurchaseOrderProducts(int purchaseOrderID);
		IEnumerable<ReportProduct> GetPurchaseOrderReportProducts(int purchaseOrderID);
		Task<IList<dynamic>> GetPurchaseOrderReceiptProducts(int purchaseOrderID);
		PurchaseOrderReceiveInfo GetPurchaseOrderReceiveInfo(int purchaseOrderID);
		SaveResults ReceiptComplete(int purchaseOrderID, IList<PurchaseOrderProductReceiptContract> products);
		PurchaseOrderContract SavePurchaseOrder(PurchaseOrderContract purchaseOrder, bool submit);
		bool DeletePurchaseOrder(int purchaseOrderID);
		Task SavePurchaseOrderProducts(List<PurchaseOrderProductContract> purchaseOrderProducts);
		bool DeletePurchaseOrderProduct(List<int> purchaseOrderProductIDs);
		bool ApprovePurchaseOrder(int purchaseOrderID);
		bool RescindPurchaseOrder(int purchaseOrderID);
		List<string> GetPeachTreeReceiptsContents();
	}
}
