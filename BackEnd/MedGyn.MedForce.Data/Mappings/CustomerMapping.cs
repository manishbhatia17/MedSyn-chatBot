using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class CustomerMapping : ClassMap<Customer>
	{
		public CustomerMapping()
		{
			Table("[Customer]");
			Id(x => x.CustomerID)
				.Column("CustomerID")
				.GeneratedBy.Identity();

			Map(x => x.CustomerName);
			Map(x => x.CustomerCustomID);
			Map(x => x.CustomerStatusCodeID);
			Map(x => x.Address1);
			Map(x => x.Address2);
			Map(x => x.City);
			Map(x => x.StateCodeID);
			Map(x => x.ZipCode);
			Map(x => x.CountryCodeID);
			Map(x => x.Website);
			Map(x => x.PrimaryContact);
			Map(x => x.PrimaryEmail);
			Map(x => x.PrimaryPhone);
			Map(x => x.PrimaryFax);
			Map(x => x.PracticeTypeCodeID);
			Map(x => x.PracticeTypeOther);
			Map(x => x.AdditionalContact1Name);
			Map(x => x.AdditionalContact1Email);
			Map(x => x.AdditionalContact1Phone);
			Map(x => x.AdditionalContact2Name);
			Map(x => x.AdditionalContact2Email);
			Map(x => x.AdditionalContact2Phone);
			Map(x => x.AdditionalContact3Name);
			Map(x => x.AdditionalContact3Email);
			Map(x => x.AdditionalContact3Phone);
			Map(x => x.PaymentTermsType);
			Map(x => x.PaymentTermsNetDueDays);
			Map(x => x.BankRoutingNumber);
			Map(x => x.BankAccountNumber);
			Map(x => x.BankAddress);
			Map(x => x.BankCity);
			Map(x => x.BankStateCodeID);
			Map(x => x.BankZipCode);
			Map(x => x.MinOrderAmount);
			Map(x => x.CreditLimit);
			Map(x => x.Notes);
			Map(x => x.SalesTaxCodeID);
			Map(x => x.PaymentTypeCodeID);
			Map(x => x.IsACHEDI);
			Map(x => x.IsDomestic);
			Map(x => x.IsDomesticAfaxys);
			Map(x => x.IsDomesticDistributor);
			Map(x => x.IsInternational);
			Map(x => x.GLSalesCodeID);
			Map(x => x.GLShippingChargeCodeID);
			Map(x => x.GLAccountsReceivableCodeID);
			Map(x => x.CreditCardName);
			Map(x => x.CreditCardNumber);
			Map(x => x.CVV);
			Map(x => x.ExpirationDate);
			Map(x => x.SearchField);
		}
	}
}
