using System;
using System.Collections.Generic;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class FedexRateQuoteRequest
	{
		public FedexRateQuoteRequest()
		{
			Packages = new List<Package>();
		}

		public string ServiceType { get; set; }
		public string FedExKey { get; set; }
		public string FedExPass { get; set; }
		public string FedExAccountNumber { get; set; }
		public string FedExMeterNumber { get; set; }
		public Address Recipient { get; set; }
		public Address Origin { get; set; }

		public List<Package> Packages { get; set; }
	}
}
