using System;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class PurchaseOrderBriefViewModel
	{
		public PurchaseOrderBriefViewModel() { }
		public PurchaseOrderBriefViewModel(dynamic results, CodeContract illinoisCode, AppSettings _appSettings)
		{
			var taxRate = results.StateCodeID == (illinoisCode?.CodeID ?? 0) ? _appSettings.IllinoisTax : 0;

			PurchaseOrderID        = results.PurchaseOrderID;
			PurchaseOrderCustomID  = results.PurchaseOrderCustomID;
			VendorCustomID         = results.VendorCustomID;
			VendorName             = results.VendorName;
			VendorOrderNumber      = results.VendorOrderNumber;
			ExpectedDate           = ((DateTime?)results.ExpectedDate)?.ToString("MM/dd/yyyy");
			TotalReceived          = results.TotalReceived ?? 0;
			Items                  = results.Items;
			Amount                 = (taxRate/100 + 1) * results.Amount + (results.ShippingCharge ?? 0);
			PrimaryProductCustomID = results.PrimaryProductCustomID;
			PrimaryProductCount    = results.PrimaryProductCount;
			PrimaryProductLotNumber = results.PrimaryLotNumber;
			HasReceipts            = results.TotalReceived != null;
			Status = SetStatus(results);
		}

		public int PurchaseOrderID { get; set; }
		public string PurchaseOrderCustomID { get; set; }
		public string VendorCustomID { get; set; }
		public string VendorName { get; set; }
		public string VendorOrderNumber { get; set; }
		public string ExpectedDate { get; set; }
		public int TotalReceived { get; set; }
		public int? Items { get; set; }
		public decimal? Amount { get; set; }
		public string PrimaryProductCustomID { get; set; }
		public int? PrimaryProductCount { get; set; }
		public string PrimaryProductLotNumber { get; set; }
		public bool HasReceipts { get; set; }
		public string Status { get; set; }


		private string SetStatus(dynamic results)
		{
			if (results.SubmitDate == null)
				return "Waiting Submission";
			else if(results.SubmitDate != null && results.ApprovalDate == null)
                return "Waiting Approval";
            else if(results.ApprovalDate != null && (results.TotalReceived == null || results.TotalReceived < results.Items) && results.IsDoNotReceive == false)
                return "To Be Received";
            else if(results.ApprovalDate != null && results.TotalReceived == results.Items && results.IsDoNotReceive == false)
                return "Has Been Received";
			else if(results.ApprovalDate != null && results.IsDoNotReceive == true)
				return "Do Not Receive";
            else 
                return "Unknown";
		}
	}
}
