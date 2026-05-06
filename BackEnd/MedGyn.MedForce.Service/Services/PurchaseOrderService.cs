using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
	public class PurchaseOrderService : IPurchaseOrderService
	{
		private readonly IPurchaseOrderRepository _purchaseOrderRepository;
		private readonly IAuthenticationService _authenticationService;

		public PurchaseOrderService(IPurchaseOrderRepository PurchaseOrderRepository, IAuthenticationService authenticationService)
		{
			_purchaseOrderRepository = PurchaseOrderRepository;
			_authenticationService = authenticationService;
		}

		public IList<dynamic> GetPurchaseOrderList(string search, string sortCol, bool sortAsc, PurchaseOrderStatusEnum status, DateRangeEnum timeframe
			, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors, int userId)
		{
			var pos = _purchaseOrderRepository.GetPurchaseOrderList(search, sortCol, sortAsc, status, timeframe,
				showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors, userId);

			return pos.GroupBy(g => g.PurchaseOrderID).Select(g => g.First()).ToList();
		}

		public PurchaseOrderContract GetPurchaseOrder(int purchaseOrderID)
		{
			var model = _purchaseOrderRepository.GetPurchaseOrder(purchaseOrderID);
			return new PurchaseOrderContract(model);
		}

		public PurchaseOrderContract GetPurchaseOrder(int purchaseOrderID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var model = _purchaseOrderRepository.GetPurchaseOrder(purchaseOrderID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);
			return new PurchaseOrderContract(model);
		}

		public List<PurchaseOrderContract> GetOpenPO(int productID)
		{	
			List<PurchaseOrderContract> openPOs = new List<PurchaseOrderContract>();
			IList<OpenPOModel> model = _purchaseOrderRepository.GetOpenProductPORequests(productID);
			model = model.Where(x =>  x.RecievedPOs < x.POs).ToList();

			foreach(OpenPOModel openPO in model)
			{
				PurchaseOrder po = new PurchaseOrder()
				{
					PurchaseOrderID = openPO.PurchaseOrderID,
					PurchaseOrderCustomID = "",
					ExpectedDate = openPO.ExpectedDate,
					VendorOrderNumber = "",					
				};

				//Get what is left to be recieved
				int qty = openPO.POs - openPO.RecievedPOs;

				openPOs.Add(new PurchaseOrderContract(po, qty));
			}

			return openPOs;
			
		}

		public bool DeletePurchaseOrder(int purchaseOrderID)
		{
			PurchaseOrder po = _purchaseOrderRepository.GetPurchaseOrder(purchaseOrderID);

			if(po == null)
				return false;

			return _purchaseOrderRepository.DeletePurchaseOrder(po);
		}

		public IEnumerable<PurchaseOrderProductContract> GetPurchaseOrderProducts(int purchaseOrderID)
		{
			return _purchaseOrderRepository.GetPurchaseOrderProducts(purchaseOrderID).Select(x => new PurchaseOrderProductContract(x));
		}

		public IEnumerable<ReportProduct> GetPurchaseOrderReportProducts(int purchaseOrderID)
		{
			return _purchaseOrderRepository.GetPurchaseOrderReportProducts(purchaseOrderID).Select(x => new ReportProduct(x));
		}

		public Task<IList<dynamic>> GetPurchaseOrderReceiptProducts(int purchaseOrderID)
		{
			return _purchaseOrderRepository.GetPurchaseOrderReceiptProducts(purchaseOrderID);
		}

		public PurchaseOrderReceiveInfo GetPurchaseOrderReceiveInfo(int purchaseOrderID)
		{
			return _purchaseOrderRepository.GetPurchaseOrderReceiveInfo(purchaseOrderID);
		}

		public PurchaseOrderContract SavePurchaseOrder(PurchaseOrderContract purchaseOrder, bool submit)
		{
			var currPo = _purchaseOrderRepository.GetPurchaseOrder(purchaseOrder.PurchaseOrderID);
			var purchaseOrderModel = purchaseOrder.ToModel(currPo);
			purchaseOrderModel.UpdatedBy = _authenticationService.GetUserID();
			purchaseOrderModel.UpdatedOn = DateTime.UtcNow;
			

			if(submit) {
				purchaseOrderModel.SubmitDate = DateTime.UtcNow;
			}

			//updates order
			if (purchaseOrderModel.PurchaseOrderID > 0)
			{
				if(purchaseOrderModel.CreatedOn == null || purchaseOrderModel.CreatedOn == DateTime.MinValue)
				{
					purchaseOrderModel.CreatedOn = purchaseOrderModel.UpdatedOn;
					purchaseOrderModel.CreatedBy = purchaseOrderModel.UpdatedBy;
				}
				_purchaseOrderRepository.UpdatePurchaseOrder(purchaseOrderModel);
			}
			//creates order
			else
			{
				purchaseOrderModel.CreatedBy = purchaseOrderModel.UpdatedBy;
				purchaseOrderModel.CreatedOn = purchaseOrderModel.UpdatedOn;
				var dailyPOCount = _purchaseOrderRepository.GetDailyPurchaseOrderCount(purchaseOrder.VendorID);
				var now = DateTime.UtcNow.Date;
				if (dailyPOCount == null)
				{
					dailyPOCount = _purchaseOrderRepository.SaveDailyPurchaseOrderCount(purchaseOrder.VendorID, now, 1);
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

					_purchaseOrderRepository.UpdateDailyPurchaseOrderCount(dailyPOCount);
				}
				purchaseOrderModel.PurchaseOrderCustomID = $"{now:MMddyyyy}-{purchaseOrder.VendorID}-{dailyPOCount.DailyCount.ToString().PadLeft(3, '0')}";
				purchaseOrderModel = _purchaseOrderRepository.SavePurchaseOrder(purchaseOrderModel);
			}

			return new PurchaseOrderContract(purchaseOrderModel);
		}

		public Task SavePurchaseOrderProducts(List<PurchaseOrderProductContract> purchaseOrderProducts)
		{
			return _purchaseOrderRepository.SavePurchaseOrderProducts(purchaseOrderProducts.Select(x => x.ToModel()).ToList());
		}

		public SaveResults ReceiptComplete(int purchaseOrderID, IList<PurchaseOrderProductReceiptContract> products)
		{
			try {
				var models = products.Select(p => p.ToModel()).ToList();
				models.ForEach(x => x.ReceiptDate = DateTime.UtcNow);
				_purchaseOrderRepository.SavePurchaseOrderReceipt(models);
				return new SaveResults();
			}
			catch(Exception e) {
				return new SaveResults(e);
			}
		}

		public IList<dynamic> GetHistoryForProduct(int productID)
		{
			return _purchaseOrderRepository.GetHistoryForProduct(productID);
		}

		public bool DeletePurchaseOrderProduct(List<int> purchaseOrderProductIDs)
		{
			return _purchaseOrderRepository.DeletePurchaseOrderProduct(purchaseOrderProductIDs);
		}

		public bool ApprovePurchaseOrder(int purchaseOrderID)
		{
			var user = _authenticationService.GetUserID();
			return _purchaseOrderRepository.ApprovePurchaseOrder(purchaseOrderID, user);
		}

		public bool RescindPurchaseOrder(int purchaseOrderID)
		{
			var purchaseOrder = _purchaseOrderRepository.GetPurchaseOrder(purchaseOrderID);
			if(purchaseOrder.ApprovedBy.HasValue)
			{
				purchaseOrder.ApprovedBy = null;
				purchaseOrder.ApprovalDate = null;
				_purchaseOrderRepository.SavePurchaseOrder(purchaseOrder);
			}
			else if (purchaseOrder.SubmitDate.HasValue)
			{
				purchaseOrder.SubmitDate = null;
				_purchaseOrderRepository.SavePurchaseOrder(purchaseOrder);
			}
			return true;
		}
		public List<string> GetPeachTreeReceiptsContents()
		{
			var results = _purchaseOrderRepository.GetPeachTreeReceiptsContents();
			return results.Select(result => (string)result.content.ToString()).ToList();
		}
	}
}
