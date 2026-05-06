using System;
using System.Collections.Generic;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class FedexShipmentRequest
	{
		public string InvoiceNumber { get; set; }
		public FedexRateQuoteRequest Request { get; set; }
	}
}
