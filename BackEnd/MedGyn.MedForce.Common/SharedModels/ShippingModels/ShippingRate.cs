
namespace MedGyn.MedForce.Common.SharedModels
{
    public class ShippingRate
    {
        public string serviceName { get; set; }
        public string serviceCode { get; set; }
        public float shipmentCost { get; set; }
        public float otherCost { get; set; }
    }
}
