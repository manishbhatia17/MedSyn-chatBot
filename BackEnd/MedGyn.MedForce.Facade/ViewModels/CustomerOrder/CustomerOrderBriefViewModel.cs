
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Models;

using MedGyn.MedForce.Service.Contracts;

using System;
using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderBriefViewModel
	{

		public CustomerOrderBriefViewModel(CustomerOrderListItem results, Dictionary<int, UserContract> users)
		{
			CustomerOrderID         = results.CustomerOrderID;
			SubmitDate              = results.SubmitDate;
			CustomerOrderCustomID   = results.CustomerOrderCustomID;
			CustomerCustomID        = results.CustomerCustomID;
			CustomerName            = results.CustomerName;
			LocationName            = results.LocationName;
			LocationCity            = results.LocationCity;
			PONumber                = results.PONumber;
			Subtotal                = results.Subtotal;
			SalesRep                = results.SalesRep;
			CustomerOrderShipmentID = results.CustomerOrderShipmentID;
			InvoiceDate             = results.InvoiceDate;
			InvoiceNumber           = results.InvoiceNumber;
			InvoiceTotal            = results.InvoiceTotal;
			HasFilled               = results.HasFilled;
			HasShipped              = results.HasShipped;
			AttachmentURI           = results.AttachmentURI;
			


			if(String.IsNullOrEmpty(SubmitDate))
				Status = "Waiting Submission";
			else if(SubmitDate != null && results.MGApprovedOn == null)
				Status = "Waiting Manager Approval";
			else if(SubmitDate != null && results.MGApprovedOn != null && results.VPApprovedOn == null)
				Status = "Waiting VP Approval";
			else if(results.VPApprovedOn != null && results.QuantityPacked != results.Quantity && results.IsDoNotFill == false)
			{
				if (results.QuantityPacked == null && (results.FilledBy == null || results.FilledBy == 0))
					Status = "To be Filled";
				else if (results.Quantity != results.QuantityPacked)
					Status = "Filling";
				else
					Status = "To be Shipped";
			}
			else if(results.VPApprovedOn != null && results.IsDoNotFill)
				Status = "Do Not Fill";
			else if(results.VPApprovedOn != null && results.QuantityPacked == results.Quantity && results.InvoiceDate == null && results.IsDoNotFill == false)
			{
				Status = "To be Shipped";
			}
			else if(results.InvoiceDate != null)
			{
				Status = "Has Been Invoiced";
			}
			else
			{
				Status = "Unknown";
			}

			//results.FilledBy = results.FilledBy ?? -1;

			if (users.TryGetValue(results.FilledBy.Value, out UserContract filledBy))
				FilledBy = filledBy.FullName;

			//InStockItems = results.OnHandQty >0 ? "Partly Open" : "New";
			NewFill = results.QuantityPacked != null && results.QuantityPacked > 0 ? "Partly Open" : "New";
		}
        public string FillStatus { get; set; }

        public int CustomerOrderID { get; set; }
		public string SubmitDate { get; set; }
		public string CustomerOrderCustomID { get; set; }
		public string CustomerCustomID { get; set; }
		public string CustomerName { get; set; }
		public string LocationName { get; set; }
		public string LocationCity { get; set; }
		public string PONumber { get; set; }
		public decimal? Subtotal { get; set; }
		public string SalesRep { get; set; }
		public string Status { get; set; }
		public int? CustomerOrderShipmentID { get; set; }
		public DateTime? InvoiceDate { get; set; }
		public string InvoiceNumber { get; set; }
		public decimal? InvoiceTotal { get; set; }
		public bool HasFilled { get; set; }
		public bool HasShipped { get; set; }
		public string AttachmentURI { get; set; }
		public string FilledBy { get; set; }
		public string InStockItems { get; set; }
		public string NewFill { get; set; }
	}
}
