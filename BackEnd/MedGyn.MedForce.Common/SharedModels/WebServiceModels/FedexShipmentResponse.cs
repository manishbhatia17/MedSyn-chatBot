using System;
using System.Collections.Generic;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class FedexShipmentResponse
	{
		public FedexShipmentResponse()
		{
			LabelImages = new List<byte[]>();
		}

		public string TrackingNumber { get; set; }
		public DateTime? DeliveryDate { get;set; }
		public string ErrorMessage { get; set; }

		public List<byte[]> LabelImages { get;set;}
	}
}
