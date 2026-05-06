using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Common.SharedModels
{
    public class ShippingOrderResponse:ShippingOrderRequest
    {
        public string orderId { get; set; }
    }
}
