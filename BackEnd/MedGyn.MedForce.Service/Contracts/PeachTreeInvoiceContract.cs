using Newtonsoft.Json;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Text.RegularExpressions;

namespace MedGyn.MedForce.Service.Contracts
{
	[Delimiter(",")]
	public class PeachTreeInvoiceContract
	{

		public PeachTreeInvoiceContract(Dictionary<string, object> InvoiceLine)
		{
			
			CustomerID = InvoiceLine["Customer ID"] != null ? InvoiceLine["Customer ID"].ToString() : "";
			InvoiceCMNumber = InvoiceLine["Invoice/CM #"] != null ? InvoiceLine["Invoice/CM #"].ToString() : "";
			Date = InvoiceLine["Date"] != null ? Convert.ToDateTime(InvoiceLine["Date"]).ToString("MM/dd/yyyy") : DateTime.Today.ToString("MM/dd/yyyy");
			ShipToName = InvoiceLine["Ship to Name"] != null ? InvoiceLine["Ship to Name"].ToString().Replace(",","") : "";
			ShipToAddressLineOne = InvoiceLine["Ship to Address-Line One"] != null ? InvoiceLine["Ship to Address-Line One"].ToString().ToString().Replace(",", "") : "";
			ShipToAddressLineTwo = InvoiceLine["Ship to Address-Line Two"] != null ?InvoiceLine["Ship to Address-Line Two"].ToString() : "";
			ShipToCity = InvoiceLine["Ship to City"] != null ? InvoiceLine["Ship to City"].ToString() : "";
			ShipToState = InvoiceLine["Ship to State"] != null ? InvoiceLine["Ship to State"].ToString() :  "";
			ShipToZipcode = InvoiceLine["Ship to Zipcode"] != null ? InvoiceLine["Ship to Zipcode"].ToString() : "";
			ShipToCountry = InvoiceLine["Ship to Country"] != null ? InvoiceLine["Ship to Country"].ToString() : "";
			CustomerPO = InvoiceLine["Customer PO"] !=  null ? InvoiceLine["Customer PO"].ToString() : "";
			ShipVia = InvoiceLine["Ship Via"] != null ? InvoiceLine["Ship Via"].ToString() : "";
			ShipDate = InvoiceLine["Ship Date"] != null ? Convert.ToDateTime(InvoiceLine["Ship Date"]).ToString("MM/dd/yyyy") : DateTime.Today.ToString("MM/dd/yyyy");
			DateDue = InvoiceLine["Date Due"] != null ? Convert.ToDateTime(InvoiceLine["Date Due"]).ToString("MM/dd/yyyy") : DateTime.Today.ToString("MM/dd/yyyy");
			DisplayedTerms = InvoiceLine["Displayed Terms"] != null ? InvoiceLine["Displayed Terms"].ToString() : "";
			SalesRepresentativeID = InvoiceLine["Sales Representative ID"] != null ? InvoiceLine["Sales Representative ID"].ToString() : "";
			AccountsReceivableAccount = InvoiceLine["Accounts Receivable Account"] != null ? InvoiceLine["Accounts Receivable Account"].ToString() : "";
			InvoiceNote = InvoiceLine["Invoice Note"] != null ? InvoiceLine["Invoice Note"].ToString().Replace(",", "") : "";
			NotePrintsAfterLineItems = InvoiceLine["Note Prints After Line Items"] != null ? Convert.ToBoolean(InvoiceLine["Note Prints After Line Items"]) : false;
			NumberOfDistributions = InvoiceLine["Number of Distributions"] != null ? Convert.ToInt32(InvoiceLine["Number of Distributions"]) : 0;
			InvoiceCMDistribution = InvoiceLine["Invoice/CM Distribution"] != null ? InvoiceLine["Invoice/CM Distribution"].ToString() : "0";
			Quantity = InvoiceLine["Quantity"] != null ? Convert.ToInt32(InvoiceLine["Quantity"]) : 0;

			string sn = InvoiceLine["Serial Number"] != null ? Regex.Replace(InvoiceLine["Serial Number"].ToString(), "[^a-zA-Z0-9 -]", "") : "";
			SerialNumber = sn.Length > 30 ? sn.Substring(0, 30) : sn;

			Description = InvoiceLine["Description"] != null ? InvoiceLine["Description"].ToString() : "";
			GLAccount = InvoiceLine["G/L Account"] != null ? InvoiceLine["G/L Account"].ToString() : "";
			UnitPrice = InvoiceLine["Unit Price"] != null ? InvoiceLine["Unit Price"].ToString() : "";
			Amount = InvoiceLine["Amount"] != null ? InvoiceLine["Amount"].ToString() : "";
			UMID = InvoiceLine["U/M ID"] != null ? InvoiceLine["U/M ID"].ToString() : "";
			UMNOofStockingUnits = InvoiceLine["U/M No. of Stocking Units"] != null ? InvoiceLine["U/M No. of Stocking Units"].ToString() : "";
			TaxType = InvoiceLine["Tax Type"] != null ? InvoiceLine["Tax Type"].ToString() : "";
			SalesTax = InvoiceLine["Sales Tax ID"] != null ? InvoiceLine["Sales Tax ID"].ToString() : "";
			SalesTaxAgency = InvoiceLine["Sales Tax Agency ID"] != null ? InvoiceLine["Sales Tax Agency ID"].ToString() : "";
			ItemID = InvoiceLine["Item ID"] != null ? InvoiceLine["Item ID"].ToString() : "";
			sortSequence = "00000000000000000000" + InvoiceCMNumber;
			sortSequence = sortSequence.Substring(sortSequence.Length - 20) + "-" + (InvoiceCMDistribution == "0" ? "999999" : "000000" + InvoiceCMDistribution.ToString().PadLeft(6, '0'));
		}

		[Ignore]
		public string sortSequence { get; set; }

		[Name("Customer ID")]
		public string CustomerID { get; set; }

		[Name("Invoice/CM #")]
		public string InvoiceCMNumber { get; set; }
		[Name("Date")]
		public string Date { get; set; }
		[Name("Ship to Name")]
		public string ShipToName { get; set; }

		[Name("Ship to Address-Line One")]
		public string ShipToAddressLineOne { get; set; }

		[Name("Ship to Address-Line Two")]
		public string ShipToAddressLineTwo { get; set; }
		[Name("Ship to City")]
		public string ShipToCity { get; set; }

		[Name("Ship to State")]
		public string ShipToState { get; set; }

		[Name("Ship to Zipcode")]
		public string ShipToZipcode { get; set; }

		[Name("Ship to Country")]
		public string ShipToCountry { get; set; }

		[Name("Customer PO")]
		public string CustomerPO { get; set; }
		[Name("Ship Via")]
		public string ShipVia { get; set; }

		[Name("Ship Date")]
		public string ShipDate { get; set; }

		[Name("Date Due")]
		public string DateDue { get; set; }

		[Name("Displayed Terms")]
		public string DisplayedTerms { get; set; }

		[Name("Sales Representative ID")]
		public string SalesRepresentativeID { get; set; }

		[Name("Accounts Receivable Account")]
		public string AccountsReceivableAccount { get; set; }

		[Name("Invoice Note")]
		public string InvoiceNote { get; set; }

		[Name("Note Prints After Line Items")]
		public bool NotePrintsAfterLineItems { get; set; }

		[Name("Number of Distributions")]
		public int NumberOfDistributions { get; set; }

		[Name("Invoice/CM Distribution")]
		public string InvoiceCMDistribution { get; set; }
		[Name("Quantity")]
		public int Quantity { get; set; }
		[Name("Item ID")]
		public string ItemID { get; set; }
		[Name("Serial Number")]
		public string SerialNumber { get; set; }
		[Name("Description")]
		public string Description { get; set; }
		[Name("G/L Account")]
		public string GLAccount { get; set; }
		[Name("Unit Price")]
		public string UnitPrice { get; set; }
		[Name("Amount")]
		public string Amount { get; set; }
		[Name("U/M ID")]
		public string UMID { get; set; }
		[Name("U/M No. of Stocking Units")]
		public string UMNOofStockingUnits { get; set; }
		[Name("Tax Type")]
		public string TaxType { get; set; }
		[Name("Sales Tax ID")]
		public string SalesTax { get; set; }
		[Name("Sales Tax Agency ID")]
		public string SalesTaxAgency { get; set; }

	}
}
