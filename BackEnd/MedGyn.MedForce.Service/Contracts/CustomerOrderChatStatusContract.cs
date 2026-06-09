using System;

namespace MedGyn.MedForce.Service.Contracts
{
    public class CustomerOrderChatStatusContract
    {
        public string Carrier {  get; set; }
        public int CustomerOrderID { get; set; }

        public string CustomerOrderCustomID { get; set; }

        public int? CustomerOrderShipmentID { get; set; }
        public string AttachmentURI { get; set; }

        public string PONumber { get; set; }

        public string Status { get; set; }

        public bool IsApproved { get; set; }

        public bool IsShipped { get; set; }

        public bool IsInvoiced { get; set; }

        public string TrackingNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }
    }
}