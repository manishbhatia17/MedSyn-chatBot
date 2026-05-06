using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Data.Models
{
    public class ProductAdjustmentActivity
    {
        public int ProductID { get; set; }
        public string ProductCustomID { get; set; }
        public string ProductName { get; set; }
        public string initQty { get; set; }
        public string endQty { get; set; }

        public ProductAdjustmentActivity()
        {
            ProductID = 0;
            ProductCustomID = "";
            ProductName = "";
            initQty = "0";
            endQty = "0";
        }

        public ProductAdjustmentActivity(dynamic results)
        {
            initQty = results.initQty != null ? results.initQty.ToString() : "0";
            endQty = results.endQty != null ? results.endQty.ToString() : "0";
            ProductID = results.ProductID;
            ProductCustomID = results.ProductCustomID;
            ProductName = results.ProductName;
        }
    }
}
