using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class ProductPriceViewModel
    {
        public ProductPriceViewModel()
        {

        }

        public ProductPriceViewModel(ProductContract product)
        {
            ProductID = product.ProductID;
            ProductCustomID = product.ProductCustomID;
            Description = product.Description;
            PriceDomesticList = product.PriceDomesticList;
            PriceDomesticDistribution = product.PriceDomesticDistribution;
            PriceDomesticAfaxys = product.PriceDomesticAfaxys;
            PriceInternationalDistribution = product.PriceInternationalDistribution;
            PriceDomesticPremier = product.PriceDomesticPremier;
            PriceMainDistributor = product.PriceMainDistributor;
            Cost = product.Cost;
        }

        public int ProductID { get; set; }
        public string ProductCustomID { get; set; }
        public string Description { get; set; }
        public decimal? PriceDomesticList { get; set; }
        public decimal? PriceDomesticDistribution { get; set; }
        public decimal? PriceDomesticAfaxys { get; set; }
        public decimal? PriceInternationalDistribution { get; set; }
        public decimal? PriceDomesticPremier { get; set; }
        public decimal? PriceMainDistributor { get; set; }
        public decimal? Cost { get; set; }


        public ProductContract ToContract()
        {
            return new ProductContract
            {
                ProductID = ProductID,
                Description = Description,
                PriceDomesticList = PriceDomesticList,
                PriceDomesticDistribution = PriceDomesticDistribution,
                PriceDomesticAfaxys = PriceDomesticAfaxys,
                PriceInternationalDistribution = PriceInternationalDistribution,
                PriceDomesticPremier = PriceDomesticPremier,
                PriceMainDistributor = PriceMainDistributor,
                Cost = Cost
            };
        }
    }
}
