using System;

namespace MedGyn.MedForce.Common.SharedModels
{
    public class CreateLabelResponse
    {
        public int shipmentId { get; set; }
        public DateTime? createDate { get; set; }
        public string shipDate { get; set; }
        public float shipmentCost { get; set; }
        public float insuranceCost { get; set; }
        public string trackingNumber { get; set; }
        public bool isReturnLabel { get; set; }
        public string carrierCode { get; set; }
        public string serviceCode { get; set; }
        public string packageCode { get; set; }
        public string confirmation { get; set; }
        public bool voided { get; set; }
        public string labelData { get; set; }
        public string errorMessage { get; set; }
        public bool success { get; set; }
    }
}
