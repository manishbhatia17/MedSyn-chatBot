using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Service.Contracts
{
    public class ActivityContract
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string AdjustmentType { get; set; }
        public int Quantity { get; set; }
        public string OrderNumber { get; set; }
        public string Reason { get; set; }
        public int AuthorizedPerson { get; set; }
        public DateTime? ActivityDate { get; set; }
    }
}
