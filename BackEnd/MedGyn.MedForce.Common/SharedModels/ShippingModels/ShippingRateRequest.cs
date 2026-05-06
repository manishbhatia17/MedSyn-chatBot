
namespace MedGyn.MedForce.Common.SharedModels
{
    public class ShippingRateRequest
    {

        public string carrierCode { get; set; }
        public string serviceCode { get; set; }
        public string packageCode { get; set; }
        public string fromPostalCode { get; set; }
        public string toState { get; set; }
        public string toCountry { get; set; }
        public string toPostalCode { get; set; }
        public string toCity { get; set; }
        public Weight weight { get; set; }
        public Dimensions dimensions { get; set; }
        public string confirmation { get; set; }
        public bool residential { get; set; }

    }
}
