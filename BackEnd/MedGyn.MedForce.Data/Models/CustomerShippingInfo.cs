
namespace MedGyn.MedForce.Data.Models
{
	public class CustomerShippingInfo
	{
		public virtual int CustomerShippingInfoID { get; set; }
		public virtual int CustomerID { get; set; }
		public virtual string Name { get; set; }
		public virtual string Address { get; set; }
		public virtual string Address2 { get; set; }
		public virtual string City { get; set; }
		public virtual int? StateCodeID { get; set; }
		public virtual string ZipCode { get; set; }
		public virtual int CountryCodeID { get; set; }
		public virtual int RepUserID { get; set; }
		public virtual int? ShipCompany1CodeID { get; set; }
		public virtual string ShipCompany1AccountNumber { get; set; }
		public virtual int? ShipCompany2CodeID { get; set; }
		public virtual string ShipCompany2AccountNumber { get; set; }
		public virtual string SearchField { get; set; }
		public virtual bool IsDisabled { get; set; }
		public virtual bool? IsDeleted { get; set; }
	}
}
