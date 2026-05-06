using System;
using System.Collections.Generic;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class VendorViewModel
	{
		public VendorViewModel()
		{
			Products = new List<string>();
		}

		public VendorViewModel(VendorContract vendor) : this()
		{
			VendorID                = vendor.VendorID;
			VendorName              = vendor.VendorName;
			VendorCustomID          = vendor.VendorCustomID;
			VendorStatusCodeID      = vendor.VendorStatusCodeID;
			Address1                = vendor.Address1;
			Address2                = vendor.Address2;
			City                    = vendor.City;
			StateCodeID             = vendor.StateCodeID;
			ZipCode                 = vendor.ZipCode;
			CountryCodeID           = vendor.CountryCodeID;
			Website                 = vendor.Website;
			PrimaryContact          = vendor.PrimaryContact;
			PrimaryEmail            = vendor.PrimaryEmail;
			PrimaryPhone            = vendor.PrimaryPhone;
			PrimaryFax              = vendor.PrimaryFax;
			TaxID                   = vendor.TaxID;
			AdditionalContact1Name  = vendor.AdditionalContact1Name;
			AdditionalContact1Email = vendor.AdditionalContact1Email;
			AdditionalContact1Phone = vendor.AdditionalContact1Phone;
			AdditionalContact2Name  = vendor.AdditionalContact2Name;
			AdditionalContact2Email = vendor.AdditionalContact2Email;
			AdditionalContact2Phone = vendor.AdditionalContact2Phone;
			AdditionalContact3Name  = vendor.AdditionalContact3Name;
			AdditionalContact3Email = vendor.AdditionalContact3Email;
			AdditionalContact3Phone = vendor.AdditionalContact3Phone;
			PaymentTermsType        = vendor.PaymentTermsType;
			PaymentTermsNetDueDays  = vendor.PaymentTermsNetDueDays;
			BankRoutingNumber       = vendor.BankRoutingNumber;
			BankAccountNumber       = vendor.BankAccountNumber;
			BankAddress             = vendor.BankAddress;
			BankCity                = vendor.BankCity;
			BankStateCodeID         = vendor.BankStateCodeID;
			BankZipCode             = vendor.BankZipCode;
			MinOrderAmount          = vendor.MinOrderAmount;
			CreditLimit             = vendor.CreditLimit;
			GLPurchaseCodeID        = vendor.GLPurchaseCodeID;
			GLFreightChargeCodeID   = vendor.GLFreightChargeCodeID;
			GLAccountsPayableCodeID = vendor.GLAccountsPayableCodeID;
			Type                    = vendor.IsDomestic ? VendorTypeEnum.Domestic
									: vendor.IsDomesticAfaxys ? VendorTypeEnum.DomesticAfaxys
									: vendor.IsDomesticDistributor ? VendorTypeEnum.DomesticDistributor
									: vendor.IsInternational ? VendorTypeEnum.International
									: VendorTypeEnum.None;
			Notes                   = vendor.Notes;
			CertificationStatus     = vendor.CertificationStatus;
			QualityInformation      = vendor.QualityInformation;
			Components              = vendor.Components;
			UpdatedOn               = vendor.UpdatedOn;
		}

		public int VendorID { get; set; }
		public string VendorName { get; set; }
		public string VendorCustomID { get; set; }
		public int VendorStatusCodeID { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public int? StateCodeID { get; set; }
		public string ZipCode { get; set; }
		public int? CountryCodeID { get; set; }
		public string Website { get; set; }
		public string PrimaryContact { get; set; }
		public string PrimaryEmail { get; set; }
		public string PrimaryPhone { get; set; }
		public string PrimaryFax { get; set; }
		public string TaxID { get; set; }
		public string AdditionalContact1Name { get; set; }
		public string AdditionalContact1Email { get; set; }
		public string AdditionalContact1Phone { get; set; }
		public string AdditionalContact2Name { get; set; }
		public string AdditionalContact2Email { get; set; }
		public string AdditionalContact2Phone { get; set; }
		public string AdditionalContact3Name { get; set; }
		public string AdditionalContact3Email { get; set; }
		public string AdditionalContact3Phone { get; set; }
		public int? PaymentTermsType { get; set; }
		public int? PaymentTermsNetDueDays { get; set; }
		public string BankRoutingNumber { get; set; }
		public string BankAccountNumber { get; set; }
		public string BankAddress { get; set; }
		public string BankCity { get; set; }
		public int? BankStateCodeID { get; set; }
		public string BankZipCode { get; set; }
		public decimal? MinOrderAmount { get; set; }
		public string CreditLimit { get; set; }
		public int? GLPurchaseCodeID { get; set; }
		public int? GLFreightChargeCodeID { get; set; }
		public int? GLAccountsPayableCodeID { get; set; }
		public VendorTypeEnum Type { get; set; }
		public string Notes { get; set; }
		public string CertificationStatus { get; set; }
		public string QualityInformation { get; set; }
		public string Components { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }

		public List<string> Products { get; set; }

		public VendorContract ToContract()
		{
			return new VendorContract
			{
				VendorID                = VendorID,
				VendorName              = VendorName,
				VendorCustomID          = VendorCustomID,
				VendorStatusCodeID      = VendorStatusCodeID,
				Address1                = Address1,
				Address2                = Address2,
				City                    = City,
				StateCodeID             = StateCodeID,
				ZipCode                 = ZipCode,
				CountryCodeID           = CountryCodeID.Value,
				Website                 = Website,
				PrimaryContact          = PrimaryContact,
				PrimaryEmail            = PrimaryEmail,
				PrimaryPhone            = PrimaryPhone,
				PrimaryFax              = PrimaryFax,
				TaxID                   = TaxID,
				AdditionalContact1Name  = AdditionalContact1Name,
				AdditionalContact1Email = AdditionalContact1Email,
				AdditionalContact1Phone = AdditionalContact1Phone,
				AdditionalContact2Name  = AdditionalContact2Name,
				AdditionalContact2Email = AdditionalContact2Email,
				AdditionalContact2Phone = AdditionalContact2Phone,
				AdditionalContact3Name  = AdditionalContact3Name,
				AdditionalContact3Email = AdditionalContact3Email,
				AdditionalContact3Phone = AdditionalContact3Phone,
				PaymentTermsType        = PaymentTermsType.Value,
				PaymentTermsNetDueDays  = PaymentTermsNetDueDays,
				BankRoutingNumber       = BankRoutingNumber,
				BankAccountNumber       = BankAccountNumber,
				BankAddress             = BankAddress,
				BankCity                = BankCity,
				BankStateCodeID         = BankStateCodeID,
				BankZipCode             = BankZipCode,
				MinOrderAmount          = MinOrderAmount,
				CreditLimit             = CreditLimit,
				GLPurchaseCodeID        = GLPurchaseCodeID,
				GLFreightChargeCodeID   = GLFreightChargeCodeID,
				GLAccountsPayableCodeID = GLAccountsPayableCodeID,
				IsDomestic              = Type == VendorTypeEnum.Domestic,
				IsDomesticAfaxys        = Type == VendorTypeEnum.DomesticAfaxys,
				IsDomesticDistributor   = Type == VendorTypeEnum.DomesticDistributor,
				IsInternational         = Type == VendorTypeEnum.International,
				Notes                   = Notes,
				CertificationStatus     = CertificationStatus,
				QualityInformation      = QualityInformation,
				Components              = Components,
			};
		}
	}
}
