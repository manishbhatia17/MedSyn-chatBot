
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CustomerShippingInfoContract
	{
		public CustomerShippingInfoContract() { }

		public CustomerShippingInfoContract(CustomerShippingInfo customerShippingInfo)
		{
			CustomerShippingInfoID    = customerShippingInfo.CustomerShippingInfoID;
			CustomerID                = customerShippingInfo.CustomerID;
			Name                      = customerShippingInfo.Name;
			Address                   = customerShippingInfo.Address;
			Address2                  = customerShippingInfo.Address2;
			City                      = customerShippingInfo.City;
			StateCodeID               = customerShippingInfo.StateCodeID;
			ZipCode                   = customerShippingInfo.ZipCode;
			CountryCodeID             = customerShippingInfo.CountryCodeID;
			RepUserID                 = customerShippingInfo.RepUserID;
			ShipCompany1CodeID        = customerShippingInfo.ShipCompany1CodeID;
			ShipCompany1AccountNumber = customerShippingInfo.ShipCompany1AccountNumber;
			ShipCompany2CodeID        = customerShippingInfo.ShipCompany2CodeID;
			ShipCompany2AccountNumber = customerShippingInfo.ShipCompany2AccountNumber;
			SearchField               = customerShippingInfo.SearchField;
			IsDisabled                = customerShippingInfo.IsDisabled;
			IsDeleted = customerShippingInfo.IsDeleted.HasValue ? customerShippingInfo.IsDeleted.Value : false;
		}

		public int CustomerShippingInfoID { get; set; }
		public int CustomerID { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public int? StateCodeID { get; set; }
		public string ZipCode { get; set; }
		public int CountryCodeID { get; set; }
		public int RepUserID { get; set; }
		public int? ShipCompany1CodeID { get; set; }
		public string ShipCompany1AccountNumber { get; set; }
		public int? ShipCompany2CodeID { get; set; }
		public string ShipCompany2AccountNumber { get; set; }
		public string SearchField { get; set; }
		public bool IsDisabled { get; set; }
		public bool IsDeleted { get; set; }
		public CustomerShippingInfo ToModel()
		{
			return new CustomerShippingInfo()
			{
				CustomerShippingInfoID    = CustomerShippingInfoID,
				CustomerID                = CustomerID,
				Name                      = Name,
				Address                   = Address,
				Address2                  = Address2,
				City                      = City,
				StateCodeID               = StateCodeID,
				ZipCode                   = ZipCode,
				CountryCodeID             = CountryCodeID,
				RepUserID                 = RepUserID,
				ShipCompany1CodeID        = ShipCompany1CodeID,
				ShipCompany1AccountNumber = ShipCompany1AccountNumber,
				ShipCompany2CodeID        = ShipCompany2CodeID,
				ShipCompany2AccountNumber = ShipCompany2AccountNumber,
				SearchField               = SearchField,
				IsDisabled                = IsDisabled,
				IsDeleted = IsDeleted,
			};
		}
	}
}
