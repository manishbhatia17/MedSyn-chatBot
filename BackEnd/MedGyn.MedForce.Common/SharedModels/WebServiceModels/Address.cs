using System;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class Address1
	{
		public string[] StreetLines { get; set; }

		public string City { get; set; }

		public string StateOrProvinceCode { get; set; }

		public string PostalCode { get; set; }

		public string UrbanizationCode { get; set; }

		public string CountryCode { get; set; }

		public string CountryName { get; set; }

		public bool Residential { get; set; }

		public bool ResidentialSpecified { get; set; }

		public string GeographicCoordinates { get; set; }

		public string ContactName { get; set; }
		public string ContactCompany { get; set; }
		public string ContactPhone { get; set; }

	}
}
