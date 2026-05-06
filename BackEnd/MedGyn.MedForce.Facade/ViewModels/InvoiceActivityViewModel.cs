using NHibernate.Linq.Functions;
using System;
using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class InvoiceActivityViewModel
	{
        public InvoiceActivityViewModel() { }

        public InvoiceActivityViewModel(dynamic results) {
            CustomerOrderID = results.CustomerOrderID;
            CustomerCustomID = results.CustomerCustomID;
            InvoiceDate = results.InvoiceDate;
            InvoiceNumber = results.InvoiceNumber;
            CustomerOrderShipmentID = results.CustomerOrderShipmentID;
            CustomerOrderCustomID = results.CustomerOrderCustomID;
            PONumber = results.PONumber;
            AttachmentURI = results.AttachmentURI;
            InvoiceTotal = results.InvoiceTotal;
            Contact = results.Contact;
            Email = results.PrimaryEmail;
            Phone = results.PrimaryPhone;
            Practice = results.Practice;
            Company = results.CustomerName;
		}

        public int CustomerOrderID { get; set; }
        public string CustomerCustomID { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public int? CustomerOrderShipmentID { get; set; }
        public string CustomerOrderCustomID { get; set; }
        public string PONumber { get; set; }
        public string AttachmentURI { get; set; }
        public decimal InvoiceTotal { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Practice { get; set; }
        public string Company { get; set; }

	}

    public class InvoiceActivityListViewModel : BaseListViewModel<InvoiceActivityViewModel>
    {
        public InvoiceActivityListViewModel(SearchCriteriaViewModel sc, List<InvoiceActivityViewModel> results) : base(sc, results) { }

    }
}
