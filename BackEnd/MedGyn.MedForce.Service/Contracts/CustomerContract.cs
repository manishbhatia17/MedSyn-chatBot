using MedGyn.MedForce.Common.Helpers;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CustomerContract
	{
		public CustomerContract() { }

		public CustomerContract(Customer customer)
		{
			CustomerID                 = customer.CustomerID;
			CustomerName               = customer.CustomerName;
			CustomerCustomID           = customer.CustomerCustomID;
			CustomerStatusCodeID       = customer.CustomerStatusCodeID;
			Address1                   = customer.Address1;
			Address2                   = customer.Address2;
			City                       = customer.City;
			StateCodeID                = customer.StateCodeID;
			ZipCode                    = customer.ZipCode;
			CountryCodeID              = customer.CountryCodeID;
			Website                    = customer.Website;
			PrimaryContact             = customer.PrimaryContact;
			PrimaryEmail               = customer.PrimaryEmail;
			PrimaryPhone               = customer.PrimaryPhone;
			PrimaryFax                 = customer.PrimaryFax;
			PracticeTypeCodeID         = customer.PracticeTypeCodeID;
			PracticeTypeOther          = customer.PracticeTypeOther;
			AdditionalContact1Name     = customer.AdditionalContact1Name;
			AdditionalContact1Email    = customer.AdditionalContact1Email;
			AdditionalContact1Phone    = customer.AdditionalContact1Phone;
			AdditionalContact2Name     = customer.AdditionalContact2Name;
			AdditionalContact2Email    = customer.AdditionalContact2Email;
			AdditionalContact2Phone    = customer.AdditionalContact2Phone;
			AdditionalContact3Name     = customer.AdditionalContact3Name;
			AdditionalContact3Email    = customer.AdditionalContact3Email;
			AdditionalContact3Phone    = customer.AdditionalContact3Phone;
			PaymentTermsType           = customer.PaymentTermsType;
			PaymentTermsNetDueDays     = customer.PaymentTermsNetDueDays;
			BankRoutingNumber          = customer.BankRoutingNumber;
			BankAccountNumber          = EncryptionHelper.DecryptString(customer.BankAccountNumber);
			BankAddress                = customer.BankAddress;
			BankCity                   = customer.BankCity;
			BankStateCodeID            = customer.BankStateCodeID;
			BankZipCode                = customer.BankZipCode;
			MinOrderAmount             = customer.MinOrderAmount;
			CreditLimit                = customer.CreditLimit;
			Notes                      = customer.Notes;
			SalesTaxCodeID             = customer.SalesTaxCodeID;
			PaymentTypeCodeID          = customer.PaymentTypeCodeID;
			IsACHEDI                   = customer.IsACHEDI;
			IsDomestic                 = customer.IsDomestic;
			IsDomesticAfaxys           = customer.IsDomesticAfaxys;
			IsDomesticDistributor      = customer.IsDomesticDistributor;
			IsInternational            = customer.IsInternational;
			GLSalesCodeID              = customer.GLSalesCodeID;
			GLShippingChargeCodeID     = customer.GLShippingChargeCodeID;
			GLAccountsReceivableCodeID = customer.GLAccountsReceivableCodeID;
			CreditCardName             = customer.CreditCardName;
			CreditCardNumber           = EncryptionHelper.DecryptString(customer.CreditCardNumber);
			CVV                        = EncryptionHelper.DecryptString(customer.CVV);
			ExpirationDate             = EncryptionHelper.DecryptString(customer.ExpirationDate);
			SearchField                = customer.SearchField;
		}

		public int CustomerID { get; set; }
		public string CustomerName { get; set; }
		public string CustomerCustomID { get; set; }
		public int CustomerStatusCodeID { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public int? StateCodeID { get; set; }
		public string ZipCode { get; set; }
		public int CountryCodeID { get; set; }
		public string Website { get; set; }
		public string PrimaryContact { get; set; }
		public string PrimaryEmail { get; set; }
		public string PrimaryPhone { get; set; }
		public string PrimaryFax { get; set; }
		public int? PracticeTypeCodeID { get; set; }
		public string PracticeTypeOther { get; set; }
		public string AdditionalContact1Name { get; set; }
		public string AdditionalContact1Email { get; set; }
		public string AdditionalContact1Phone { get; set; }
		public string AdditionalContact2Name { get; set; }
		public string AdditionalContact2Email { get; set; }
		public string AdditionalContact2Phone { get; set; }
		public string AdditionalContact3Name { get; set; }
		public string AdditionalContact3Email { get; set; }
		public string AdditionalContact3Phone { get; set; }
		public int PaymentTermsType { get; set; }
		public int? PaymentTermsNetDueDays { get; set; }
		public string BankRoutingNumber { get; set; }
		public string BankAccountNumber { get; set; }
		public string BankAddress { get; set; }
		public string BankCity { get; set; }
		public int? BankStateCodeID { get; set; }
		public string BankZipCode { get; set; }
		public decimal? MinOrderAmount { get; set; }
		public string CreditLimit { get; set; }
		public string Notes { get; set; }
		public int? SalesTaxCodeID { get; set; }
		public int? PaymentTypeCodeID { get; set; }
		public bool IsACHEDI { get; set; }
		public bool IsDomestic { get; set; }
		public bool IsDomesticAfaxys { get; set; }
		public bool IsDomesticDistributor { get; set; }
		public bool IsInternational { get; set; }
		public int? GLSalesCodeID { get; set; }
		public int? GLShippingChargeCodeID { get; set; }
		public int? GLAccountsReceivableCodeID { get; set; }
		public string CreditCardName { get; set; }
		public string CreditCardNumber { get; set; }
		public string CVV { get; set; }
		public string ExpirationDate { get; set; }
		public string SearchField { get; set; }

		public Customer ToModel()
		{
			return new Customer()
			{
				CustomerID                 = CustomerID,
				CustomerName               = CustomerName,
				CustomerCustomID           = CustomerCustomID,
				CustomerStatusCodeID       = CustomerStatusCodeID,
				Address1                   = Address1,
				Address2                   = Address2,
				City                       = City,
				StateCodeID                = StateCodeID,
				ZipCode                    = ZipCode,
				CountryCodeID              = CountryCodeID,
				Website                    = Website,
				PrimaryContact             = PrimaryContact,
				PrimaryEmail               = PrimaryEmail,
				PrimaryPhone               = PrimaryPhone,
				PrimaryFax                 = PrimaryFax,
				PracticeTypeCodeID         = PracticeTypeCodeID,
				PracticeTypeOther          = PracticeTypeOther,
				AdditionalContact1Name     = AdditionalContact1Name,
				AdditionalContact1Email    = AdditionalContact1Email,
				AdditionalContact1Phone    = AdditionalContact1Phone,
				AdditionalContact2Name     = AdditionalContact2Name,
				AdditionalContact2Email    = AdditionalContact2Email,
				AdditionalContact2Phone    = AdditionalContact2Phone,
				AdditionalContact3Name     = AdditionalContact3Name,
				AdditionalContact3Email    = AdditionalContact3Email,
				AdditionalContact3Phone    = AdditionalContact3Phone,
				PaymentTermsType           = PaymentTermsType,
				PaymentTermsNetDueDays     = PaymentTermsNetDueDays,
				BankRoutingNumber          = BankRoutingNumber,
				BankAccountNumber          = EncryptionHelper.EncryptString(BankAccountNumber),
				BankAddress                = BankAddress,
				BankCity                   = BankCity,
				BankStateCodeID            = BankStateCodeID,
				BankZipCode                = BankZipCode,
				MinOrderAmount             = MinOrderAmount,
				CreditLimit                = CreditLimit,
				Notes                      = Notes,
				SalesTaxCodeID             = SalesTaxCodeID,
				PaymentTypeCodeID          = PaymentTypeCodeID,
				IsACHEDI                   = IsACHEDI,
				IsDomestic                 = IsDomestic,
				IsDomesticAfaxys           = IsDomesticAfaxys,
				IsDomesticDistributor      = IsDomesticDistributor,
				IsInternational            = IsInternational,
				GLSalesCodeID              = GLSalesCodeID,
				GLShippingChargeCodeID     = GLShippingChargeCodeID,
				GLAccountsReceivableCodeID = GLAccountsReceivableCodeID,
				CreditCardName             = CreditCardName,
				CreditCardNumber           = EncryptionHelper.EncryptString(CreditCardNumber),
				CVV                        = EncryptionHelper.EncryptString(CVV),
				ExpirationDate             = EncryptionHelper.EncryptString(ExpirationDate),
				SearchField                = SearchField,
			};
		}
	}
}
