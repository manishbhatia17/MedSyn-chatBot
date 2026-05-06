
namespace MedGyn.MedForce.Common.SharedModels
{
    public class Carrier
    {
        public string name { get; set; }
        public string code { get; set; }
        public string accountNumber { get; set; }
        public bool requiresFundedAccount { get; set; }
        public float balance { get; set; }
        public object nickname { get; set; }
        public int shippingProviderId { get; set; }
        public bool primary { get; set; }
    }
}
