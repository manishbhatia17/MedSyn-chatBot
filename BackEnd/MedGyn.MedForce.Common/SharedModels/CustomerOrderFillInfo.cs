using System;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class CustomerOrderFillInfo
	{
		public CustomerOrderFillInfo() { }

		public CustomerOrderFillInfo(dynamic cofi)
		{
			CustomerOrderID = cofi.CustomerOrderID;
			CustomerOrderCustomID = cofi.CustomerOrderCustomID;
			SubmitDate = cofi.SubmitDate;
			CustomerShippingInfoID = cofi.CustomerShippingInfoID;
			ShipCompany = cofi.ShipCompany;
			ShipMethod = cofi.ShipMethod;
			ShipCompanyType = cofi.ShipCompanyType;
			ShipCompany1CodeID = cofi.ShipCompany1CodeID;
			ShipCompany2CodeID = cofi.ShipCompany2CodeID;
			ShipCompany1AccountNumber = cofi.ShipCompany1AccountNumber;
			ShipCompany2AccountNumber = cofi.ShipCompany2AccountNumber;
			IsPartialShipAcceptable = cofi.IsPartialShipAcceptable;
			SalesRepID = cofi.SalesRepID ?? -1;
			CustomerID = cofi.CustomerID;
			CustomerCustomID = cofi.CustomerCustomID;
			CustomerName = cofi.CustomerName;
			IsInternationalCustomer = cofi.IsInternationalCustomer;
			CustomerShipName = cofi.CustomerShipName;
			CustomerShipAddress = cofi.CustomerShipAddress;
			CustomerShipAddress2 = cofi.CustomerShipAddress2;
			CustomerShipCity = cofi.CustomerShipCity;
			CustomerShipState = cofi.CustomerShipState;
			CustomerShipZip = cofi.CustomerShipZip;
			CustomerShipCountry = cofi.CustomerShipCountry;
			CustomerBillingCountry = cofi.CountryCodeID;
			Notes = cofi.Notes;
			Instructions = cofi.Instructions;
			UpdatedBy = cofi.UpdatedBy;
			UpdatedOn = cofi.UpdatedOn;
			IntermediaryShippingName = cofi.IntermediaryShippingName;
			IntermediaryShippingAddress = cofi.IntermediaryShippingAddress;
			IntermediaryShippingContactName = cofi.IntermediaryShippingContactName;
			IntermediaryShippingContactNumber = cofi.IntermediaryShippingContactNumber;
			IntermediaryShippingContactEmail = cofi.IntermediaryShippingContactEmail;
			GenerateMultiplePackingSlip = false;
			PONumber = cofi.PONumber;
			IsFilling = cofi.IsFilling;
			FilledBy = cofi.FilledBy ?? -1;
			FilledByOn = cofi.FilledByOn ?? DateTime.Now;
			FinancingApproved = cofi.FinancingApproved;
			FinancingApprovedBy = cofi.FinancingApprovedBy ?? -1;
			FinancingApprovedOn = cofi.FinancingApprovedOn ?? DateTime.Now;

		}

		public int CustomerOrderID { get; set; }
		public string CustomerOrderCustomID { get; set; }
		public DateTime? SubmitDate { get; set; }
		public int CustomerShippingInfoID { get; set; }
		public string ShipCompany { get; set; }
		public string ShipMethod { get; set; }
		private int? ShipCompanyType { get; set; }
		private int? ShipCompany1CodeID { get; set; }
		private int? ShipCompany2CodeID { get; set; }
		private string ShipCompany1AccountNumber { get; set; }
		private string ShipCompany2AccountNumber { get; set; }
		public bool IsPartialShipAcceptable { get; set; }
		public int SalesRepID { get; set; }
		public int CustomerID { get; set; }
		public string CustomerCustomID { get; set; }
		public string CustomerName { get; set; }
		public bool IsInternationalCustomer { get; set; }
		public string CustomerShipName { get; set; }
		public string CustomerShipAddress { get; set; }
		public string CustomerShipAddress2 { get; set; }
		public string CustomerShipCity { get; set; }
		public string CustomerShipState { get; set; }
		public string CustomerShipZip { get; set; }
		public string CustomerShipCountry { get; set; }
		public int CustomerBillingCountry { get; set; }
		public string Notes { get; set; }
		public string Instructions { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public string IntermediaryShippingName { get; set; }
		public string IntermediaryShippingAddress { get; set; }
		public string IntermediaryShippingContactName { get; set; }
		public string IntermediaryShippingContactNumber { get; set; }
		public string IntermediaryShippingContactEmail { get; set; }
		public string PONumber { get; set; }
		public bool IsFilling { get; set; }
		public int FilledBy { get; set; }
		public DateTime FilledByOn {get;set;}
		public bool FinancingApproved { get; set; }
		public int FinancingApprovedBy { get; set; }
		public DateTime FinancingApprovedOn { get; set; }

		public string ShipAccountNumber
		{
			get
			{
				if (ShipCompanyType == ShipCompany1CodeID)
					return ShipCompany1AccountNumber;
				else if (ShipCompanyType == ShipCompany2CodeID)
					return ShipCompany2AccountNumber;
				return null;
			}
		}
		public bool GenerateMultiplePackingSlip { get; set; }
	}
}
