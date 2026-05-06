using MedGyn.MedForce.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class ProductShippedViewModel
    {
        private dynamic x;

        public ProductShippedViewModel() { }

        public ProductShippedViewModel(dynamic results, List<UserContract> Users)
        {
            CultureInfo format = new CultureInfo("en-US");
            InvoiceDate = results.InvoiceDate;
            InvoiceNumber = results.InvoiceNumber;
            InvoiceTotal = results.InvoiceTotal;
            CustomerOrderID = results.CustomerOrderCustomID;
            PONumber = results.PONumber;
            CustomerID = results.CustomerCustomID;
            ProductID = results?.ProductCustomID;
            Quantity = results.Quantity.ToString("N", format).Split('.')[0];
            LineTotal = "$"+(Math.Round((results.Quantity * results.Price), 2)).ToString("N",format);

            UserContract shippedByuser = Users.Find(x => x.UserId == results.ShippedBy);
            
            if(shippedByuser != null)
            {
                ShippedBy = shippedByuser?.FullName;
            }
            else
            {
                ShippedBy = "";
            }

            ShippedByOn = results.ShippedByOn;

            UserContract packedByuser = Users.Find(x => x.UserId == results.FilledBy);
            if(packedByuser != null)
            {
				PackedBy = packedByuser?.FullName;
			}
			else
            {
				PackedBy = "";
			}

            PackedByOn = results.FilledByOn;
        }

        public dynamic CustomerOrderID { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public dynamic InvoiceNumber { get; set; }
        public dynamic InvoiceTotal { get; private set; }
        public dynamic PONumber { get; set; }
        public dynamic CustomerID { get; private set; }
        public dynamic ProductID { get; private set; }
        public dynamic Quantity { get; private set; }
        public dynamic LineTotal { get; private set; }
        public string ShippedBy { get; private set; }
        public DateTime? ShippedByOn { get; private set; } 
        public string PackedBy { get; private set; }
        public DateTime? PackedByOn { get; private set; }
    }

    public class ProductShippedListViewModel : BaseListViewModel<ProductShippedViewModel>
    {
        public ProductShippedListViewModel(SearchCriteriaViewModel sc, List<ProductShippedViewModel> results) : base(sc, results) { }

    }
}
