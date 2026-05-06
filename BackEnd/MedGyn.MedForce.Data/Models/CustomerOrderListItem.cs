using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Data.Models
{
    public  class CustomerOrderListItem
    {
        public CustomerOrderListItem() { }
        public CustomerOrderListItem(dynamic OrderItem) 
        {
            CustomerCustomID = OrderItem.CustomerCustomID.ToString();
			CustomerName = OrderItem.CustomerName.ToString();
			CustomerOrderCustomID = OrderItem.CustomerOrderCustomID.ToString();
			CustomerOrderID = OrderItem.CustomerOrderID;
			InvoiceDate = (DateTime?)OrderItem.InvoiceDate;
			InvoiceNumber = OrderItem.InvoiceNumber.ToString();
			InvoiceTotal =(decimal?)OrderItem.InvoiceTotal;
			LocationCity = OrderItem.LocationCity.ToString();
			LocationName = OrderItem.LocationName.ToString();
			PONumber = OrderItem.PONumber.ToString();
			SalesRep = OrderItem.SalesRep.ToString();
            SubmitDate = OrderItem.SubmitDate;
			Subtotal = OrderItem.Subtotal;
			CustomerOrderShipmentID =  (int?)OrderItem.CustomerOrderShipmentID;
			HasFilled = (bool)OrderItem.HasFilled;
			HasShipped =  (bool)OrderItem.HasShipped;
			AttachmentURI =  OrderItem.AttachmentURI.ToString();
			MGApprovedOn = OrderItem.MGApprovedOn;
			VPApprovedOn =OrderItem.VPApprovedOn;
			IsDoNotFill =OrderItem.IsDoNotFill;
			Quantity = OrderItem.Quantity;
			QuantityPacked = OrderItem.QuantityPacked;
			FilledBy = OrderItem.FilledBy;
            ItemIDs = OrderItem.ItemIDs;
        }
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
        public int? CustomerOrderShipmentID { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public bool HasFilled { get; set; }
        public bool HasShipped { get; set; }
        public string AttachmentURI { get; set; }
        public DateTime? MGApprovedOn { get; set; }
        public DateTime? VPApprovedOn { get; set; }
        public bool IsDoNotFill { get; set; }
        public int? Quantity { get; set; }
        public int? QuantityPacked { get; set; }
        public int? FilledBy { get; set; }
        public int? OnHandQty { get; set; }
        public string ItemIDs { get; set; }
        
    }
}
