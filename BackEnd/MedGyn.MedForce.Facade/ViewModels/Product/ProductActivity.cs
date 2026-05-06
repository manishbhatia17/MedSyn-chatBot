using MedGyn.MedForce.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MedGyn.MedForce.Facade.ViewModels.Product
{
    public class ProductActivity
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string AdjustmentType { get; set; }
        public string Units { get; set; }
        public string OrderNumber { get; set; }
        public string Reason { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string AuthorizedPerson { get; set; }

        public ProductActivity(ActivityContract Activity, Dictionary<int, UserContract> Users)
        {
            ProductID = Activity.ProductID.ToString();
            AdjustmentType = Activity.AdjustmentType;
            Units = Activity.Quantity.ToString();
            OrderNumber = Activity.OrderNumber;
            Reason = Activity.Reason;
            ActivityDate = Activity.ActivityDate;
            
            if(Activity.AuthorizedPerson > 0)
            {
				UserContract user = Users[Activity.AuthorizedPerson];
				AuthorizedPerson = user.FirstName + " " + user.LastName;
			}

        }
    }
}
