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
			ProductID                      = result.ProductID;
			ProductName                    = result.ProductName;
			ProductCustomID                = result.ProductCustomID;
			Description                    = result.Description is DBNull ? null : result.Description;
			Manufacturer                   = result.Manufacturer is DBNull ? null : result.Manufacturer;
			Color                          = result.Color is DBNull ? null : result.Color;
			Notes                          = result.Notes is DBNull ? null : result.Notes;
			PriceDomesticList              = result.PriceDomesticList is DBNull ? (decimal?)null : (decimal?)result.PriceDomesticList;
			PriceDomesticDistribution      = result.PriceDomesticDistribution is DBNull ? (decimal?)null : (decimal?)result.PriceDomesticDistribution;
			PriceDomesticAfaxys            = result.PriceDomesticAfaxys is DBNull ? (decimal?)null : (decimal?)result.PriceDomesticAfaxys;
			PriceInternationalDistribution = result.PriceInternationalDistribution is DBNull ? (decimal?)null : (decimal?)result.PriceInternationalDistribution;
			PriceDomesticPremier           = result.PriceDomesticPremier is DBNull ? (decimal?)null : (decimal?)result.PriceDomesticPremier;
			PriceMainDistributor           = result.PriceMainDistributor is DBNull ? (decimal?)null : (decimal?)result.PriceMainDistributor;
			Cost                           = result.Cost is DBNull ? (decimal?)null : (decimal?)result.Cost;
			IsDiscontinued                 = result.IsDiscontinued is DBNull ? false : result.IsDiscontinued;
			IsDeleted                      = result.IsDeleted is DBNull ? (bool?)null : (bool?)result.IsDeleted;
			InternationalOnly              = result.InternationalOnly is DBNull ? (bool?)null : (bool?)result.InternationalOnly;
			SpecialOrderOnly               = result.SpecialOrderOnly is DBNull ? (bool?)null : (bool?)result.SpecialOrderOnly;
			PrimaryImageURI                = result.PrimaryImageURI is DBNull ? null : result.PrimaryImageURI;
			UnitOfMeasureCodeID            = result.UnitOfMeasureCodeID is DBNull ? (int?)null : (int?)result.UnitOfMeasureCodeID;
			PrimaryVendorID                = result.PrimaryVendorID is DBNull ? (int?)null : (int?)result.PrimaryVendorID;
			ShipWeight                     = result.ShipWeight is DBNull ? (decimal?)null : (decimal?)result.ShipWeight;
			Length                         = result.Length is DBNull ? (decimal?)null : (decimal?)result.Length;
			Width                          = result.Width is DBNull ? (decimal?)null : (decimal?)result.Width;
			Depth                          = result.Depth is DBNull ? (decimal?)null : (decimal?)result.Depth;
			ShipWeightUnitsCodeID          = result.ShipWeightUnitsCodeID is DBNull ? (int?)null : (int?)result.ShipWeightUnitsCodeID;
			ShipDimensionUnitsCodeID       = result.ShipDimensionUnitsCodeID is DBNull ? (int?)null : (int?)result.ShipDimensionUnitsCodeID;
			ReorderPoint                   = result.ReorderPoint is DBNull ? (int?)null : (int?)result.ReorderPoint;
			ReorderQuantity                = result.ReorderQuantity is DBNull ? (int?)null : (int?)result.ReorderQuantity;
			AdditionalVendor1ID            = result.AdditionalVendor1ID is DBNull ? (int?)null : (int?)result.AdditionalVendor1ID;
			AdditionalVendor2ID            = result.AdditionalVendor2ID is DBNull ? (int?)null : (int?)result.AdditionalVendor2ID;
			AdditionalVendor3ID            = result.AdditionalVendor3ID is DBNull ? (int?)null : (int?)result.AdditionalVendor3ID;
			AdditionalVendor4ID            = result.AdditionalVendor4ID is DBNull ? (int?)null : (int?)result.AdditionalVendor4ID;
			AdditionalVendor5ID            = result.AdditionalVendor5ID is DBNull ? (int?)null : (int?)result.AdditionalVendor5ID;
			AdditionalVendor6ID            = result.AdditionalVendor6ID is DBNull ? (int?)null : (int?)result.AdditionalVendor6ID;
			ExtraImage1URI                 = result.ExtraImage1URI is DBNull ? null : result.ExtraImage1URI;
			ExtraImage2URI                 = result.ExtraImage2URI is DBNull ? null : result.ExtraImage2URI;
			ExtraImage3URI                 = result.ExtraImage3URI is DBNull ? null : result.ExtraImage3URI;
			ExtraImage4URI                 = result.ExtraImage4URI is DBNull ? null : result.ExtraImage4URI;
			ExtraImage5URI                 = result.ExtraImage5URI is DBNull ? null : result.ExtraImage5URI;
			ExtraImage6URI                 = result.ExtraImage6URI is DBNull ? null : result.ExtraImage6URI;
			ExtraImage7URI                 = result.ExtraImage7URI is DBNull ? null : result.ExtraImage7URI;
			ExtraImage8URI                 = result.ExtraImage8URI is DBNull ? null : result.ExtraImage8URI;
			UpdatedOn                      = result.UpdatedOn is DBNull ? default(DateTime) : (DateTime)result.UpdatedOn;
			UpdatedBy                      = result.UpdatedBy is DBNull ? 0 : (int)result.UpdatedBy;
		}
	}
}