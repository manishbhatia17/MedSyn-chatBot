using System;

namespace MedGyn.MedForce.Data.Models
{
	public class Product
	{
		public virtual int ProductID { get; set; }
		public virtual string ProductName { get; set; }
		public virtual string ProductCustomID { get; set; }
		public virtual int? UnitOfMeasureCodeID { get; set; }
		public virtual int? ReorderPoint { get; set; }
		public virtual int? ReorderQuantity { get; set; }
		public virtual int? PrimaryVendorID { get; set; }
		public virtual decimal? Cost { get; set; }
		public virtual bool? InternationalOnly { get; set; }
		public virtual bool? SpecialOrderOnly { get; set; }
		public virtual string Color { get; set; }
		public virtual decimal? ShipWeight { get; set; }
		public virtual decimal? Length { get; set; }
		public virtual decimal? Width { get; set; }
		public virtual decimal? Depth { get; set; }
		public virtual string Description { get; set; }
		public virtual string Manufacturer { get; set; }
		public virtual decimal? PriceDomesticList { get; set; }
		public virtual decimal? PriceDomesticDistribution { get; set; }
		public virtual decimal? PriceDomesticAfaxys { get; set; }
		public virtual decimal? PriceInternationalDistribution { get; set; }
		public virtual decimal? PriceDomesticPremier { get; set; }
		public virtual decimal? PriceMainDistributor { get; set; }
		public virtual string Notes { get; set; }
		public virtual bool? IsDeleted { get; set; }
		public virtual DateTime UpdatedOn { get; set; }
		public virtual int UpdatedBy { get; set; }
		public virtual bool IsDiscontinued { get; set; }
		public virtual int? ShipWeightUnitsCodeID { get; set; }
		public virtual int? ShipDimensionUnitsCodeID { get; set; }
		public virtual int? AdditionalVendor1ID { get; set; }
		public virtual int? AdditionalVendor2ID { get; set; }
		public virtual int? AdditionalVendor3ID { get; set; }
		public virtual int? AdditionalVendor4ID { get; set; }
		public virtual int? AdditionalVendor5ID { get; set; }
		public virtual int? AdditionalVendor6ID { get; set; }
		public virtual string PrimaryImageURI { get; set; }
		public virtual string ExtraImage1URI { get; set; }
		public virtual string ExtraImage2URI { get; set; }
		public virtual string ExtraImage3URI { get; set; }
		public virtual string ExtraImage4URI { get; set; }
		public virtual string ExtraImage5URI { get; set; }
		public virtual string ExtraImage6URI { get; set; }
		public virtual string ExtraImage7URI { get; set; }
		public virtual string ExtraImage8URI { get; set; }

		public virtual Vendor PrimaryVendor { get; set; }

		public Product()
		{

		}

		public Product(dynamic result)
		{
			ProductID = result.ProductID;
			ProductName = result.ProductName;
			ProductCustomID = result.ProductCustomID;
		}
	}
}