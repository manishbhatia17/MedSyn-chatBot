using System;
using System.Linq;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class ProductContract
	{
		public ProductContract() { }

		public ProductContract(Product product)
		{
			ProductID                      = product.ProductID;
			ProductName                    = product.ProductName;
			ProductCustomID                = product.ProductCustomID;
			InternationalOnly              = product.InternationalOnly;
			SpecialOrderOnly               = product.SpecialOrderOnly;
			UnitOfMeasureCodeID            = product.UnitOfMeasureCodeID;
			Color                          = product.Color;
			ShipWeight                     = product.ShipWeight;
			Length                         = product.Length;
			Width                          = product.Width;
			Depth                          = product.Depth;
			PrimaryVendorID                = product.PrimaryVendorID;
			Description                    = product.Description;
			Manufacturer                   = product.Manufacturer;
			PriceDomesticList              = product.PriceDomesticList;
			PriceDomesticDistribution      = product.PriceDomesticDistribution;
			PriceDomesticAfaxys            = product.PriceDomesticAfaxys;
			PriceInternationalDistribution = product.PriceInternationalDistribution;
			PriceDomesticPremier		   = product.PriceDomesticPremier;
            PriceMainDistributor		   = product.PriceMainDistributor;
			Cost                           = product.Cost;
			Notes                          = product.Notes;
			ReorderPoint                   = product.ReorderPoint;
			ReorderQuantity                = product.ReorderQuantity;
			IsDeleted                      = product.IsDeleted;
			UpdatedOn                      = product.UpdatedOn;
			UpdatedBy                      = product.UpdatedBy;
			IsDiscontinued                 = product.IsDiscontinued;
			ShipWeightUnitsCodeID          = product.ShipWeightUnitsCodeID;
			ShipDimensionUnitsCodeID       = product.ShipDimensionUnitsCodeID;
			AdditionalVendor1ID            = product.AdditionalVendor1ID;
			AdditionalVendor2ID            = product.AdditionalVendor2ID;
			AdditionalVendor3ID            = product.AdditionalVendor3ID;
			AdditionalVendor4ID            = product.AdditionalVendor4ID;
			AdditionalVendor5ID            = product.AdditionalVendor5ID;
			AdditionalVendor6ID            = product.AdditionalVendor6ID;
			PrimaryImageURI                = product.PrimaryImageURI;
			ExtraImage1URI                 = product.ExtraImage1URI;
			ExtraImage2URI                 = product.ExtraImage2URI;
			ExtraImage3URI                 = product.ExtraImage3URI;
			ExtraImage4URI                 = product.ExtraImage4URI;
			ExtraImage5URI                 = product.ExtraImage5URI;
			ExtraImage6URI                 = product.ExtraImage6URI;
			ExtraImage7URI                 = product.ExtraImage7URI;
			ExtraImage8URI                 = product.ExtraImage8URI;
		}

		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public string ProductCustomID { get; set; }
		public bool? InternationalOnly { get; set; }
		public bool? SpecialOrderOnly { get; set; }
		public int? UnitOfMeasureCodeID { get; set; }
		public string Color { get; set; }
		public decimal? ShipWeight { get; set; }
		public int? ShipWeightUnitsCodeID { get; set; }
		public decimal? Length { get; set; }
		public decimal? Width { get; set; }
		public decimal? Depth { get; set; }
		public int? ShipDimensionUnitsCodeID { get; set; }
		public int? PrimaryVendorID { get; set; }
		public int? AdditionalVendor1ID { get; set; }
		public int? AdditionalVendor2ID { get; set; }
		public int? AdditionalVendor3ID { get; set; }
		public int? AdditionalVendor4ID { get; set; }
		public int? AdditionalVendor5ID { get; set; }
		public int? AdditionalVendor6ID { get; set; }
		public string Description { get; set; }
		public string Manufacturer { get; set; }
		public decimal? PriceDomesticList { get; set; }
		public decimal? PriceDomesticDistribution { get; set; }
		public decimal? PriceDomesticAfaxys { get; set; }
		public decimal? PriceInternationalDistribution { get; set; }
        public decimal? PriceDomesticPremier { get; set; }
        public decimal? PriceMainDistributor { get; set; }
		public decimal? Cost { get; set; }
		public string Notes { get; set; }
		public int? ReorderPoint { get; set; }
		public int? ReorderQuantity { get; set; }
		public bool? IsDeleted { get; set; }
		public DateTime UpdatedOn { get; set; }
		public int UpdatedBy { get; set; }
		public bool IsDiscontinued { get; set; }
		public string PrimaryImageURI { get; set; }
		public string ExtraImage1URI { get; set; }
		public string ExtraImage2URI { get; set; }
		public string ExtraImage3URI { get; set; }
		public string ExtraImage4URI { get; set; }
		public string ExtraImage5URI { get; set; }
		public string ExtraImage6URI { get; set; }
		public string ExtraImage7URI { get; set; }
		public string ExtraImage8URI { get; set; }
		public Product ToModel()
		{
			return new Product()
			{
				ProductID                      = ProductID,
				ProductName                    = ProductName,
				ProductCustomID                = ProductCustomID,
				InternationalOnly              = InternationalOnly,
				SpecialOrderOnly               = SpecialOrderOnly,
				UnitOfMeasureCodeID            = UnitOfMeasureCodeID,
				Color                          = Color,
				ShipWeight                     = ShipWeight,
				Length                         = Length,
				Width                          = Width,
				Depth                          = Depth,
				PrimaryVendorID                = PrimaryVendorID,
				Description                    = Description,
				Manufacturer                   = Manufacturer,
				PriceDomesticList              = PriceDomesticList,
				PriceDomesticDistribution      = PriceDomesticDistribution,
				PriceDomesticAfaxys            = PriceDomesticAfaxys,
				PriceInternationalDistribution = PriceInternationalDistribution,
				PriceMainDistributor		   = PriceMainDistributor,
				PriceDomesticPremier		   = PriceDomesticPremier,
				Cost                           = Cost,
				Notes                          = Notes,
				ReorderPoint                   = ReorderPoint,
				ReorderQuantity                = ReorderQuantity,
				IsDeleted                      = IsDeleted,
				UpdatedOn                      = UpdatedOn,
				UpdatedBy                      = UpdatedBy,
				IsDiscontinued                 = IsDiscontinued,
				ShipWeightUnitsCodeID          = ShipWeightUnitsCodeID,
				ShipDimensionUnitsCodeID       = ShipDimensionUnitsCodeID,
				AdditionalVendor1ID            = AdditionalVendor1ID,
				AdditionalVendor2ID            = AdditionalVendor2ID,
				AdditionalVendor3ID            = AdditionalVendor3ID,
				AdditionalVendor4ID            = AdditionalVendor4ID,
				AdditionalVendor5ID            = AdditionalVendor5ID,
				AdditionalVendor6ID            = AdditionalVendor6ID,
				PrimaryImageURI                = PrimaryImageURI,
				ExtraImage1URI                 = ExtraImage1URI,
				ExtraImage2URI                 = ExtraImage2URI,
				ExtraImage3URI                 = ExtraImage3URI,
				ExtraImage4URI                 = ExtraImage4URI,
				ExtraImage5URI                 = ExtraImage5URI,
				ExtraImage6URI                 = ExtraImage6URI,
				ExtraImage7URI                 = ExtraImage7URI,
				ExtraImage8URI                 = ExtraImage8URI,
			};
		}
	}
}
