
namespace MedGyn.MedForce.Common.SharedModels
{
    public class CreateLabelRequest
    {
        public string orderId { get; set; }
        public string carrierCode { get; set; }
        public string serviceCode { get; set; }
        public string packageCode { get; set; }
        public string confirmation { get; set; }
        public string shipDate { get; set; }
        public Weight weight { get; set; }
        public Dimensions dimensions { get; set; }
       // public Address shipFrom { get; set; }
        //public Address shipTo { get; set; }
        public bool testLabel { get; set; }
		//public AdvancedOptions advancedOptions { get; set; }
		public InternationalOptions internationalOptions { get; set; }

	}
}
