using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class CustomsItem
	{
		public string customsItemId { get; set; }
		public string description { get; set; }
		public int quantity { get; set; }
		public decimal value { get; set; }
		public string harmonizedTariffCode { get; set; }
		public string countryOfOrigin { get; set; }
	}
}
