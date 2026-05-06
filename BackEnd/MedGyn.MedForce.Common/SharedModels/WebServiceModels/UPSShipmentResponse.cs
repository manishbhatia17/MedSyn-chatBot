using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class UPSShipmentResponse
	{
		public UPSShipmentResponse()
		{
			LabelImages = new List<byte[]>();
		}
		public string TrackingNumber { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public string ErrorMessage { get; set; }

		public List<byte[]> LabelImages { get; set; }
	}
}