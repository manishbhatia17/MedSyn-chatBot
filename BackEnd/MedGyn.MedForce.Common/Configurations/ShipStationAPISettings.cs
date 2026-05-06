using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Common.Configurations
{
	public class ShipStationAPISettings
	{
		public string ShipStationApiUrl { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string ShipStationGetRatesApiUrl { get; set; }
		public string ShipStationCreateLabelApiUrl { get; set; }
		public string ShipStationVoidLabelApiUrl { get; set; }
		public string ShipStationGetCarriersApiUrl { get; set; }
		public string ShipStationGetCarrierServicesApiUrl { get; set; }
		public string ShipStationGetPackagesApiUrl { get; set; }
		public string ShipStationCreateOrderApiUrl { get; set; }
		public string ShipStationCreateOrderLabelApiUrl { get; set; }
		public string ShipFromAddress { get; set; }
		public string ShipFromZip { get; set; }
		public string ShipFromState { get; set; }
		public string ShipFromCity { get; set; }
		public string ShipFromName { get; set; }
		public string ShipFromCountry { get; set; }
		public string ShipFromContactPhone { get; set; }
	}
}
