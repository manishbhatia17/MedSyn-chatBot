using System;
using System.Collections.Generic;
using MedGyn.MedForce.Common.SharedModels;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class PackingSlipViewModel
	{
		public PackingSlipViewModel() { }
		public string CustomerName { get; set; }
		public string CustomerAddress { get; set; }
		public string CustomerAddress2 { get; set; }
		public string CustomerCity { get; set; }
		public string CustomerState { get; set; }
		public string CustomerCountry { get; set; }
		public string CustomerZip { get; set; }
		public string CustomerShipName { get; set; }
		public string CustomerShipAddress { get; set; }
		public string CustomerShipAddress2 { get; set; }
		public string CustomerShipCity { get; set; }
		public string CustomerShipState { get; set; }
		public string CustomerShipCountry { get; set; }
		public string CustomerShipZip { get; set; }
		public string CustomerOrderNumber { get; set; }
		public string CustomerOrderDate { get; set; }
		public string SalesRep { get; set; }
		public string ShipCompany { get; set; }
		public string ShipMethod { get; set; }
		public string IsPartialShip { get; set; }
		public string CustomerID { get; set; }
		public string CustomerPONumber { get; set; }
		public string PaymentTerms { get; set; }
		public string TrackingNumber { get; set; }
		public string Instructions { get; set; }
		public decimal Subtotal { get; set; }
		public decimal Taxes { get; set; }
		public decimal? ShippingCharge { get; set; }
		public decimal TotalAmount { get; set; }
		public IList<InvoiceProduct> Products { get; set; }
		public string ISName { get; set; }
		public string ISAddress { get; set; }
		public string ISContactName { get; set; }
		public string ISContactNumber { get; set; }
		public string ISContactEmail { get; set; }
		public bool IsInternational { get; set; }
        public string ShippingCustomerName { get; set; }
    }
}
