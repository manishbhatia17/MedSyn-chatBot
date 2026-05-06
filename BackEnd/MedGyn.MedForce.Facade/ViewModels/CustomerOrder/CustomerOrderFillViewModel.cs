using System;
using System.Collections.Generic;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderFillViewModel
	{
		public CustomerOrderFillViewModel() { }
		public CustomerOrderFillViewModel(
			CustomerOrderFillInfo cofi,
			Dictionary<int, UserContract> users
		)
		{
			CustomerOrderID                   = cofi.CustomerOrderID;
			CustomerOrderCustomID             = cofi.CustomerOrderCustomID;
			PONumber                          = cofi.PONumber;
			SubmitDate                        = cofi.SubmitDate;
			ShipCompany                       = cofi.ShipCompany;
			ShipMethod                        = cofi.ShipMethod;
			ShipAccountNumber                 = cofi.ShipAccountNumber;
			IsPartialShipAcceptable           = cofi.IsPartialShipAcceptable;
			CustomerCustomID                  = cofi.CustomerCustomID;
			CustomerName                      = cofi.CustomerName;
			IsInternationalCustomer           = cofi.IsInternationalCustomer;
			CustomerShipName                  = cofi.CustomerShipName;
			CustomerShipAddress               = cofi.CustomerShipAddress;
			CustomerShipAddress2 = cofi.CustomerShipAddress2;			   
			CustomerShipCityStateZip          = string.Join(", ", cofi.CustomerShipCity, cofi.CustomerShipState, cofi.CustomerShipZip);
			CustomerShipCountry               = cofi.CustomerShipCountry;
			Notes                             = cofi.Notes;
			Instructions                      = cofi.Instructions;
			UpdatedOn                         = cofi.UpdatedOn;
			FillOption                        = 1;
			NumberOfSameBoxes                 = 1;
			NumberOfPackingSlips              = 1;
			IntermediaryShippingName          = cofi.IntermediaryShippingName;
			IntermediaryShippingAddress       = cofi.IntermediaryShippingAddress;
			IntermediaryShippingContactName   = cofi.IntermediaryShippingContactName;
			IntermediaryShippingContactNumber = cofi.IntermediaryShippingContactNumber;
			IntermediaryShippingContactEmail  = cofi.IntermediaryShippingContactEmail;
			GenerateMultiplePackingSlip		  = cofi.GenerateMultiplePackingSlip;
			IsFilling                         = cofi.IsFilling;
			FilledByOn                        = cofi.FilledByOn;
			FilledById						  = cofi.FilledBy;
			FinancingApproved = cofi.FinancingApproved;
			FinancingApprovedOn = cofi.FinancingApprovedOn;
			FinancingApprovedById = cofi.FinancingApprovedBy;

			if (users.TryGetValue(cofi.SalesRepID, out var salesRep))
				SalesRep = $"{salesRep.SalesRepID} {salesRep.FullName}";

			if(users.TryGetValue(cofi.UpdatedBy, out var updatedBy))
				UpdatedBy = updatedBy.FullName;

			if(users.TryGetValue(cofi.FilledBy, out var filledBy))
				FilledBy = filledBy.FullName;

			if (users.TryGetValue(cofi.FinancingApprovedBy, out var financingApprovedBy))
				FinancingApprovedBy = financingApprovedBy.FullName;
		}

		public int CustomerOrderID { get; set; }
		public string CustomerOrderCustomID { get; set; }
		public string PONumber { get; set; }
		public DateTime? SubmitDate { get; set; }
		public string ShipCompany { get; set; }
		public string ShipMethod { get; set; }
		public string ShipAccountNumber { get; set; }
		public bool IsPartialShipAcceptable { get; set; }
		public string SalesRep { get; set; }
		public string CustomerCustomID { get; set; }
		public string CustomerName { get; set; }
		public bool IsInternationalCustomer { get; set; }
		public string CustomerShipName { get; set; }
		public string CustomerShipAddress { get; set; }
		public string CustomerShipAddress2 { get; set; }
		public string CustomerShipCityStateZip { get; set; }
		public string CustomerShipCountry { get; set; }
		public string Notes { get; set; }
		public string Instructions { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public int FillOption { get; set; }
		public int? NumberOfSameBoxes { get; set; }
		public int? NumberOfPackingSlips { get; set; }
		public string IntermediaryShippingName { get; set; }
		public string IntermediaryShippingAddress { get; set; }
		public string IntermediaryShippingContactName { get; set; }
		public string IntermediaryShippingContactNumber { get; set; }
		public string IntermediaryShippingContactEmail { get; set; }
		public bool IsFilling { get; set; }
		public string FilledBy { get; set; }
		public DateTime FilledByOn { get; set; }
		public int FilledById { get; set; }
		public bool FinancingApproved { get;set; }
		public DateTime FinancingApprovedOn { get; set; }
		public int FinancingApprovedById { get; set; }
		public string FinancingApprovedBy { get; set; }
		public List<CustomerOrderProductFillViewModel> Products { get; set; }
		public List<int> Boxes { get; set; }
		public bool GenerateMultiplePackingSlip { get; set; }
	}
}
