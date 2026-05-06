using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Service.Contracts
{
    public class CustomerOrderActivityContract:ActivityContract
    {
        public int CustomerOrderProductFillID { get; set; }

		public CustomerOrderActivityContract(dynamic customerOrderActivity)
        {
            ProductID = customerOrderActivity.ProductID;
            AdjustmentType = "Sales Order";
            //this is quantity leaving inventory so we want it to be negative
            Quantity = customerOrderActivity.QuantityPacked * -1;
            OrderNumber = customerOrderActivity.CustomerOrderCustomID;
            Reason = "SO Filled";
            AuthorizedPerson = customerOrderActivity.CreatedBy != null ? customerOrderActivity.CreatedBy : -1;
            ActivityDate = customerOrderActivity.CreatedOn != null ? customerOrderActivity.CreatedOn : new DateTime(2023, 4,11,12,0,0);
			CustomerOrderProductFillID = customerOrderActivity.CustomerOrderProductFillID;
        }
    }
}
