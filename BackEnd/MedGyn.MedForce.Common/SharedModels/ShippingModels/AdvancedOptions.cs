using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class AdvancedOptions
	{
		public string billToParty { get; set; }
		public string billToAccount { get; set; }
		public string billToPostalCode { get; set; }
		public string billToCountryCode { get; set; }
		public string customField1 { get; set; }
		public string customField2 { get; set; }
		public string customField3 { get; set; }
	}
}
