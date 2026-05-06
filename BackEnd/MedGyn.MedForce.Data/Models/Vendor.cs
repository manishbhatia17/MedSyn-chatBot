using System;

namespace MedGyn.MedForce.Data.Models
{
	public class Vendor
	{
		public virtual int VendorID { get; set; }
		public virtual string VendorName { get; set; }
		public virtual string VendorCustomID { get; set; }
		public virtual int VendorStatusCodeID { get; set; }
		public virtual string Address1 { get; set; }
		public virtual string Address2 { get; set; }
		public virtual string City { get; set; }
		public virtual int? StateCodeID { get; set; }
		public virtual string ZipCode { get; set; }
		public virtual int CountryCodeID { get; set; }
		public virtual string Website { get; set; }
		public virtual string PrimaryContact { get; set; }
		public virtual string PrimaryEmail { get; set; }
		public virtual string PrimaryPhone { get; set; }
		public virtual string PrimaryFax { get; set; }
		public virtual string TaxID { get; set; }
		public virtual string AdditionalContact1Name { get; set; }
		public virtual string AdditionalContact1Email { get; set; }
		public virtual string AdditionalContact1Phone { get; set; }
		public virtual string AdditionalContact2Name { get; set; }
		public virtual string AdditionalContact2Email { get; set; }
		public virtual string AdditionalContact2Phone { get; set; }
		public virtual string AdditionalContact3Name { get; set; }
		public virtual string AdditionalContact3Email { get; set; }
		public virtual string AdditionalContact3Phone { get; set; }
		public virtual int PaymentTermsType { get; set; }
		public virtual int? PaymentTermsNetDueDays { get; set; }
		public virtual string BankRoutingNumber { get; set; }
		public virtual string BankAccountNumber { get; set; }
		public virtual string BankAddress { get; set; }
		public virtual string BankCity { get; set; }
		public virtual int? BankStateCodeID { get; set; }
		public virtual string BankZipCode { get; set; }
		public virtual decimal? MinOrderAmount { get; set; }
		public virtual string CreditLimit { get; set; }
		public virtual int? GLPurchaseCodeID { get; set; }
		public virtual int? GLFreightChargeCodeID { get; set; }
		public virtual int? GLAccountsPayableCodeID { get; set; }
		public virtual bool IsDomestic { get; set; }
		public virtual bool IsDomesticAfaxys { get; set; }
		public virtual bool IsDomesticDistributor { get; set; }
		public virtual bool IsInternational { get; set; }
		public virtual string Notes { get; set; }
		public virtual string CertificationStatus { get; set; }
		public virtual string QualityInformation { get; set; }
		public virtual string Components { get; set; }
		public virtual int UpdatedBy { get; set; }
		public virtual DateTime UpdatedOn { get; set; }
	}
}
