using System;
using System.Collections.Generic;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class UPSRateQuoteRequest
	{
		public UPSRateQuoteRequest()
		{
			Packages = new List<Package>();
		}

		public string ServiceType { get; set; }
		public string AccessLicenseNumber { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string AccountNumber { get; set; }
		public Address Recipient { get; set; }
		public Address Origin { get; set; }

		public List<Package> Packages { get; set; }
	}
}
