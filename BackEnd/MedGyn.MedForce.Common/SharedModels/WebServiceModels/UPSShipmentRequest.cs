using MedGyn.MedForce.Common.SharedModels;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class UPSShipmentRequest
	{
		public string InvoiceNumber { get; set; }
		public UPSRateQuoteRequest Request { get; set; }
	}
}