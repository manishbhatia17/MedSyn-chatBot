using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Common.SharedModels
{
    public class ShippingOrderRequest
    {
        public ShippingOrderRequest()
        {
            
        
        }
  
        public string orderNumber { get; set; }
        public string orderKey { get; set; }
        public string orderDate { get; set; }
        public string shipDate { get; set; }
        public string orderStatus { get; set; }
        public Address billTo { get; set; }
        public Address shipTo { get; set; }
        public string carrierCode { get; set; }
        public string serviceCode { get; set;}
        public string packageCode { get; set; }
        public string confirmation { get; set; }
        public Weight weight { get; set; }
        public Dimensions dimensions { get; set; }
        public InternationalOptions internationalOptions { get; set; }
        public AdvancedOptions advancedOptions { get; set; }
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public bool testLabel { get; set; } = false;
        
    }
}
