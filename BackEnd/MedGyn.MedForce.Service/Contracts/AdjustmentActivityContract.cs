using MedGyn.MedForce.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Service.Contracts
{
    public class AdjustmentActivityContract: ActivityContract
    {
        public AdjustmentActivityContract(ProductInventoryAdjustment AdjustmentActivity, Dictionary<int, CodeContract> Codes) 
        {
            ProductID = AdjustmentActivity.ProductID;           
            AdjustmentType = "Adjustment";
            Quantity = AdjustmentActivity.Quantity;
            OrderNumber = AdjustmentActivity.ProductInventoryAdjustmentID.ToString();
            Reason = AdjustmentActivity.ReasonCodeOther;
            AuthorizedPerson = AdjustmentActivity.AdjustedBy;
            ActivityDate = AdjustmentActivity.AdjustmentDate;

            if(AdjustmentActivity.ReasonCodeID > 0)
            {
				CodeContract code = Codes[AdjustmentActivity.ReasonCodeID];
				Reason = code.CodeDescription;

                if(!string.IsNullOrEmpty(AdjustmentActivity.ReasonCodeOther))
                {
                    Reason = code.CodeDescription + " - " + AdjustmentActivity.ReasonCodeOther;
                }
			}
        }
    }
}
