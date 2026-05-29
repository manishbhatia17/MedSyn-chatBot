using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetProductByNameCustomerChatBotCommandHandler : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType => CustomerChatBotCommandType.GetProductByName;

        private readonly IProductService _productService;

        public GetProductByNameCustomerChatBotCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<CustomerChatResponseDTO> HandleAsync(string[] parameters, CustomerChatRequestDTO request)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters[0]);

            if (!dict.TryGetValue("product_name", out string productName) || string.IsNullOrWhiteSpace(productName))
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Please provide a product name to search for." };

            var product = _productService.SearchProductByName(productName);

            if (product == null)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Product not found." };

            var sb = new StringBuilder();
            sb.AppendLine($"Here is the information for **{product.ProductName}** (Product ID: {product.ProductCustomID}):");
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(product.Description))
            {
                sb.AppendLine(product.Description);
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(product.Manufacturer))
                sb.AppendLine($"**Manufacturer:** {product.Manufacturer}");

            if (!string.IsNullOrWhiteSpace(product.Color))
                sb.AppendLine($"**Color:** {product.Color}");

            if (product.PriceDomesticList.HasValue)
                sb.AppendLine($"**List Price:** ${product.PriceDomesticList:F2}");

            sb.AppendLine($"**Status:** {(product.IsDiscontinued ? "Discontinued" : "Available")}");

            if (product.SpecialOrderOnly == true)
                sb.AppendLine("**Special Order:** This product is available by special order only.");

            if (product.InternationalOnly == true)
                sb.AppendLine("**Availability:** International markets only.");

            if (!string.IsNullOrWhiteSpace(product.Notes))
                sb.AppendLine($"**Notes:** {product.Notes}");

            if (!string.IsNullOrWhiteSpace(product.PrimaryImageURI))
                sb.AppendLine($"**Product Image:** {product.PrimaryImageURI}");

            sb.AppendLine();
            sb.AppendLine("Would you like to **place an order online** at [www.medgyn.com](https://www.medgyn.com), or would you prefer to **speak with your sales representative** for personalized assistance?");

            return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = sb.ToString(), Data = product };
        }
    }
}
