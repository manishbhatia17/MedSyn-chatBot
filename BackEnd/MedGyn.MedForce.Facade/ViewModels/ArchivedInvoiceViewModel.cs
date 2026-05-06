using NHibernate.Linq.Functions;
using System;
using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class ArchivedInvoiceViewModel
    {
        public ArchivedInvoiceViewModel() { }

        public ArchivedInvoiceViewModel(dynamic results) {
            CustomerOrderID = results.CustomerOrderID;
            CustomerCustomID = results.CustomerCustomID;
            InvoiceDate = results.InvoiceDate;
            InvoiceNumber = results.InvoiceNumber;
            CustomerOrderShipmentID = results.CustomerOrderShipmentID;
            CustomerOrderCustomID = results.CustomerOrderCustomID;
            PONumber = results.PONumber;
            AttachmentURI = results.AttachmentURI;
            InvoiceTotal = results.InvoiceTotal;
        }

        public int CustomerOrderID { get; set; }
        public string CustomerCustomID { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public int? CustomerOrderShipmentID { get; set; }
        public string CustomerOrderCustomID { get; set; }
        public string PONumber { get; set; }
        public string AttachmentURI { get; set; }
        public string ShippedBy { get; set; }
        public string FilledBy { get; set; }
        public DateTime? ShippedOn { get; set; }
        public DateTime? FilledByOn { get; set; }
        public decimal InvoiceTotal { get; set; }
    }

    public class ArchivedInvoiceListViewModel : BaseListViewModel<ArchivedInvoiceViewModel>
    {
        public ArchivedInvoiceListViewModel(SearchCriteriaViewModel sc, List<ArchivedInvoiceViewModel> results) : base(sc, results) { }

    }
}
