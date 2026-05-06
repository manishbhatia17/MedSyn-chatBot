using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class CustomerShippingInfoMapping : ClassMap<CustomerShippingInfo>
	{
		public CustomerShippingInfoMapping()
		{
			Table("[CustomerShippingInfo]");
			Id(x => x.CustomerShippingInfoID)
				.Column("CustomerShippingInfoID")
				.GeneratedBy.Identity();

			Map(x => x.CustomerID);
			Map(x => x.Name);
			Map(x => x.Address);
			Map(x => x.Address2);
			Map(x => x.City);
			Map(x => x.StateCodeID);
			Map(x => x.ZipCode);
			Map(x => x.CountryCodeID);
			Map(x => x.RepUserID);
			Map(x => x.ShipCompany1CodeID);
			Map(x => x.ShipCompany1AccountNumber);
			Map(x => x.ShipCompany2CodeID);
			Map(x => x.ShipCompany2AccountNumber);
			Map(x => x.SearchField);
			Map(x => x.IsDisabled);
			Map(x => x.IsDeleted);
		}
	}
}
