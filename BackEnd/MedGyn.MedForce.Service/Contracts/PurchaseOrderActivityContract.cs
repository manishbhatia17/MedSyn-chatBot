using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Service.Contracts
{
    public class PurchaseOrderActivityContract: ActivityContract
    {
        public PurchaseOrderActivityContract(dynamic purchaseOrderActivity)
        {
            ProductID = purchaseOrderActivity.ProductID;
            ProductName = "";
            AdjustmentType = "Purchase Order";
            Quantity = purchaseOrderActivity.QuantityReceived;
            OrderNumber = purchaseOrderActivity.PurchaseOrderCustomID;
            Reason = "PO Received";
            AuthorizedPerson = purchaseOrderActivity.ReceivedBy != null ? purchaseOrderActivity.ReceivedBy : 0;
            ActivityDate = purchaseOrderActivity.ReceiptDate;
        }
    }
}
