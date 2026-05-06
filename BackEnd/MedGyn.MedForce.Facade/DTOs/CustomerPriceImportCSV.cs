using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MedGyn.MedForce.Facade.ViewModels;
namespace MedGyn.MedForce.Facade.DTOs
{
	public class CustomerPriceImportCSV
	{
		[Name("Internal Product ID")]
		public int? ProductID { get; set; }
		[Name("Product ID")]
		public string ProductCustomID { get; set; }
		[Name("Description")]
		public string Description { get; set; }
		[Name("Distributor List Price")]
		public decimal? DistributorListPrice { get; set; }
		[Name("Domestic Premier Price")]
		public decimal? DomesticPremierPrice { get; set; }
		[Name("Domestic Afaxsys Price")]
		public decimal? DomesticAfaxisPrice { get; set; }
		[Name("Domestic List Price")]
		public decimal? DomesticListPrice { get; set; }
		[Name("Main Distributor Price")]
		public decimal? MainDistributorPrice { get; set; }
		[Name("International Distribution Price")]
		public decimal? InternationalDistributorPrice { get; set; }
		[Name("Cost")]
		public decimal? Cost { get; set; }


		public ProductPriceViewModel ToProductPriceViewModel()
		{
			if(ProductCustomID == null)
			{
				throw new Exception("Product ID is required");
			}

			return new ProductPriceViewModel
			{
				ProductID = ProductID != null ? ProductID.Value : 0,
				ProductCustomID = ProductCustomID,
				Description = !string.IsNullOrEmpty(Description) ? Description : "",
				PriceDomesticList = DomesticListPrice,
				PriceDomesticDistribution = DistributorListPrice,
				PriceDomesticAfaxys = DomesticAfaxisPrice,
				PriceInternationalDistribution = InternationalDistributorPrice,
				PriceDomesticPremier = DomesticPremierPrice,
				PriceMainDistributor = MainDistributorPrice,
				Cost = Cost
			};
		}
	}
}
