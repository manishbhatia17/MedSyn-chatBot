using MedGyn.MedForce.Data.Models;
using FluentNHibernate.Mapping;

namespace MedGyn.MedForce.Data.Mappings
{
	public class ProductMapping : ClassMap<Product>
	{
		public ProductMapping()
		{
			Table("[Product]");
			Id(x => x.ProductID)
				.Column("ProductID")
				.GeneratedBy.Identity();

			Map(x => x.ProductName);
			Map(x => x.ProductCustomID);
			Map(x => x.UnitOfMeasureCodeID);
			Map(x => x.ReorderPoint);
			Map(x => x.ReorderQuantity);
			Map(x => x.PrimaryVendorID);
			Map(x => x.Cost);
			Map(x => x.InternationalOnly);
			Map(x => x.SpecialOrderOnly);
			Map(x => x.Color);
			Map(x => x.ShipWeight);
			Map(x => x.Length);
			Map(x => x.Width);
			Map(x => x.Depth);
			Map(x => x.Description);
			Map(x => x.Manufacturer);
			Map(x => x.PriceDomesticList);
			Map(x => x.PriceDomesticDistribution);
			Map(x => x.PriceDomesticAfaxys);
			Map(x => x.PriceInternationalDistribution);
			Map(x => x.PriceDomesticPremier);
            Map(x => x.PriceMainDistributor);
			Map(x => x.Notes);
			Map(x => x.IsDeleted);
			Map(x => x.UpdatedOn);
			Map(x => x.UpdatedBy);
			Map(x => x.IsDiscontinued);
			Map(x => x.ShipWeightUnitsCodeID);
			Map(x => x.ShipDimensionUnitsCodeID);
			Map(x => x.AdditionalVendor1ID);
			Map(x => x.AdditionalVendor2ID);
			Map(x => x.AdditionalVendor3ID);
			Map(x => x.AdditionalVendor4ID);
			Map(x => x.AdditionalVendor5ID);
			Map(x => x.AdditionalVendor6ID);
			Map(x => x.PrimaryImageURI);
			Map(x => x.ExtraImage1URI);
			Map(x => x.ExtraImage2URI);
			Map(x => x.ExtraImage3URI);
			Map(x => x.ExtraImage4URI);
			Map(x => x.ExtraImage5URI);
			Map(x => x.ExtraImage6URI);
			Map(x => x.ExtraImage7URI);
			Map(x => x.ExtraImage8URI);
		}
	}
}
