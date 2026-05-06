using MedGyn.MedForce.Data.Models;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Service
{
    public class PriceReconciliationContract
    {
        public int? CustomerOrderProductFillID { get; set; }
        public int? CustomerOrderProductID { get; set; }
        public int? ProductID { get; set; }
        public string ProductCustomID { get; set; }
        public string ProductName { get; set; }
        public decimal? PriceInternationalDistribution { get; set; }
        public decimal? PriceDomesticAfaxys { get; set; }
        public decimal? PriceDomesticList { get; set; }
        public int? OrderQuantity { get; set; }
        public int? CustomerOrderShipmentID { get; set; }
        public int? CustomerOrderShipmentBoxID { get; set; }
        public decimal? Price { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public string CustomerCustomID { get; set; }
        public int? QuantityPacked { get; set; }
        public string SerialNumbers { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public IList<PriceReconciliationContract> ListContract(IList<dynamic> Model)
        {
            IList<PriceReconciliationContract> list = new List<PriceReconciliationContract>();
            foreach(dynamic modelItem in Model)
            {
                list.Add(Map(modelItem));
            }

            return list;
        }

        public PriceReconciliationContract Map(dynamic Model)
        {
            return new PriceReconciliationContract()
            {
                CustomerOrderProductFillID = Model?.CustomerOrderProductFillID,
                CustomerOrderProductID = Model?.CustomerOrderProductID,
                ProductID = Model?.ProductID,
                ProductCustomID = Model?.ProductCustomID,
                ProductName = Model?.ProductName,
                PriceInternationalDistribution = Model?.PriceInternationalDistribution,
                PriceDomesticAfaxys = Model?.PriceDomesticAfaxys,
                PriceDomesticList = Model?.PriceDomesticList,
                OrderQuantity = Model?.OrderQuantity,
                CustomerOrderShipmentID = Model?.CustomerOrderShipmentID,
                CustomerOrderShipmentBoxID = Model?.CustomerOrderShipmentBoxID,
                Price = Model?.Price,
                InvoiceTotal = Model?.InvoiceTotal,
                CustomerCustomID = Model?.CustomerCustomID,
                QuantityPacked = Model?.QuantityPacked,
                SerialNumbers = Model?.SerialNumbers,
                InvoiceNumber = Model?.InvoiceNumber,
                InvoiceDate = Model?.InvoiceDate
            };
        }
    }
}
