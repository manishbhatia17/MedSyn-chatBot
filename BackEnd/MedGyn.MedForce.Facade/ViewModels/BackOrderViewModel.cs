using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Service.Contracts;
using NHibernate.Linq.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class BackOrderViewModel
    {
        public BackOrderViewModel() { }
        public BackOrderViewModel(
        dynamic cofi, Dictionary<int, UserContract> users
         )
        {
            CustomerOrderCustomID = cofi.CustomerOrderCustomID;
            SubmitDate = cofi.SubmitDate==null?null:cofi.SubmitDate.ToString();
            CustomerCustomID = cofi.CustomerCustomID;
            ProductCustomID = cofi.ProductCustomID;
            CustomerName = cofi.CustomerName;
            PONumber = cofi.PONumber;
            Quantity = cofi.Quantity;
            ProductName = cofi.ProductName;
            CustomerOrderID = cofi.CustomerOrderID;
            Price = cofi.Price;
            IsFilling = cofi.IsFilling;
            Total = 0.0M;//cofi.Total;

            cofi.FilledBy = cofi.FilledBy ?? -1;
            if (users.TryGetValue(cofi.FilledBy, out UserContract filledBy))
                FilledBy = filledBy.FullName;
        } 
        public string CustomerCustomID { get; set; }
        public int CustomerOrderID { get; set; }
        public string? SubmitDate { get; set; }
        public string CustomerOrderCustomID { get; set; }
        public string ProductCustomID { get; set; }
        public string CustomerName { get; set; }
        public string PONumber { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public bool IsFilling { get; set; }
        public string FilledBy { get; set; }
        public decimal Total { get; set; }

    }

    public class BackOrderListViewModel : BaseListViewModel<BackOrderViewModel>
    {
        public double TotalAmount { get; set; }
        public int TotalQty { get; set; }
        public BackOrderListViewModel(SearchCriteriaViewModel sc, List<BackOrderViewModel> results) : base(sc, results) { }
       
    }
}
