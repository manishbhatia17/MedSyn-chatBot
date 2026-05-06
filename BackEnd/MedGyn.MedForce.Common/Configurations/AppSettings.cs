using System;

namespace MedGyn.MedForce.Common.Configurations
{
	public class AppSettings
	{
		public string DatabaseVersion { get; set; }
		public string MigrationFileFolder { get; set; }
		public int SystemOwnerEntityId { get; set; }
		public string BuildVersion { get; set; }
		public DateTime BuildTimestamp { get; set; }
		public string AzureBlobContainer { get; set; }
	

		public string WebServiceAPIEndpoint { get; set; }
		public string WebRootPath { get; set; }

		public string FedExKey { get; set; }
		public string FedExPass { get; set; }
		public string FedExAccountNumber { get; set; }
		public string FedExMeterNumber { get; set; }
		public string FedExOfficeIntegratorID { get; set; }
		public string FedExClientProductID { get; set; }
		public string FedExOfficeProductVersion { get; set; }

		public string UPSUser { get; set; }
		public string UPSPass { get; set; }
		public string UPSLicense { get; set; }


		public string MedGynName { get; set; }
		public string MedGynAddress { get; set; }
		public string MedGynCity { get; set; }
		public string MedGynStateOrProvinceCode { get; set; }
		public string MedGynPostalCode { get; set; }
		public string MedGynCountryCode { get; set; }
		public string MedGynCountryAbbr { get; set; }
		public string MedGynContactCompany { get; set; }
		public string MedGynContactPhone { get; set; }
		public string MedGynFax { get; set; }
		public string MedGynTollFreePhone { get; set; }
		public string MedGynEmail { get; set; }


		public string UPSAccountNumber { get; set; }
		public decimal IllinoisTax { get; set; }
		public decimal CreditCardFee { get; set; }

		public string InvoiceRemittanceAddress { get; set; }
		public string InvoiceRemittanceMessage { get; set; }
		public string InvoiceFooter { get; set; }

		public string PackingSlipRemittanceAddress { get; set; }
		public string PackingSlipRemittanceMessage { get; set; }

		public string Url { get; set; }

		//graph api
		public string Realm { get; set; }
		public string GraphClientId { get; set; }
		public string GraphCertThumbprint { get; set; }
		public string GraphUrl { get; set; }
		public string BaseSharePointUrl { get; set; }
	}
}
