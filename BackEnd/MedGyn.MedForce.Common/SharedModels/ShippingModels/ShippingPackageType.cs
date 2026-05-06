
namespace MedGyn.MedForce.Common.SharedModels
{
    public class ShippingPackageType
    {
        public string carrierCode { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public bool domestic { get; set; }
        public bool international { get; set; }

    }
}
