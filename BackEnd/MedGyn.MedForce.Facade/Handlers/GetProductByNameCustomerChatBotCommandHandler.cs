using Medforce.Graph.Services.Interfaces;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Facade.DTOs;
using System;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Options;
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
        private readonly ISharePointListSearchService _sharePointService;
        private readonly ICustomerService _customerService;
        private readonly ICodeService _codeService;
        private readonly string _baseUrl;

        public GetProductByNameCustomerChatBotCommandHandler(
            IProductService productService,
            ISharePointListSearchService sharePointService,
            ICustomerService customerService,
            ICodeService codeService,
            IOptions<AppSettings> appSettings)
        {
            _productService = productService;
            _sharePointService = sharePointService;
            _customerService = customerService;
            _codeService = codeService;
            _baseUrl = appSettings.Value.Url?.TrimEnd('/');
        }

        public async Task<CustomerChatResponseDTO> HandleAsync(string[] parameters, CustomerChatRequestDTO request)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters[0]);

            if (!dict.TryGetValue("product_name", out string productName) || string.IsNullOrWhiteSpace(productName))
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Please provide a product name to search for." };

            var product = _productService.GetProductByCustomId(productName);

            if (product == null || product.ProductID == 0)
                product = _productService.SearchProductByName(productName);

            if (product == null || product.ProductID == 0)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = $"No product found matching \"{productName}\"." };

            var sb = new StringBuilder();
            sb.AppendLine($"Here is the information for **{product.ProductName}** (Product ID: {product.ProductCustomID}):");
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(product.Description))
            {
                var description = product.Description.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Trim();
                sb.AppendLine($"**About this Product:** {description}");
                sb.AppendLine();
            }

            sb.AppendLine();

            // Search SharePoint for brochure, IFU and manual in parallel
            try
            {
                var brochureTask = _sharePointService.ProductDocumentExistsAsync(product.ProductCustomID, "Product Brochures");
                var ifuTask      = _sharePointService.ProductDocumentExistsAsync(product.ProductCustomID, "Product IFUs");
                var manualTask   = _sharePointService.ProductDocumentExistsAsync(product.ProductCustomID, "Product Manuals");
                await Task.WhenAll(brochureTask, ifuTask, manualTask);

                if (!string.IsNullOrWhiteSpace(_baseUrl))
                {
                    if (brochureTask.Result)
                        sb.AppendLine($"[Download Brochure]({_baseUrl}/api/chatbot/brochure/{Uri.EscapeDataString(product.ProductCustomID)})");
                    if (ifuTask.Result && request.CustomerId.HasValue)
                        sb.AppendLine($"[Download IFU]({_baseUrl}/api/chatbot/ifu/{Uri.EscapeDataString(product.ProductCustomID)})");
                    if (manualTask.Result)
                        sb.AppendLine($"[Download Manual]({_baseUrl}/api/chatbot/manual/{Uri.EscapeDataString(product.ProductCustomID)})");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"**Document retrieval error:** {ex.Message}");
                sb.AppendLine("To access product brochures and IFUs you can:");
                sb.AppendLine("- [Speak to Sales Rep](chataction://GetRepersentativeByCountryOrState)");
                sb.AppendLine("- [Visit MedGyn Academy](https://netorgft3403149.sharepoint.com/sites/MedGynAcademy)");
            }

            sb.AppendLine();
            bool isUS;
            if (request.CustomerId.HasValue)
            {
                var customer = _customerService.GetCustomer(request.CustomerId.Value);
                var countryCode = _codeService.GetCodeLookupByType(CodeTypeEnum.Countries)
                    .TryGetValue(customer?.CountryCodeID ?? 0, out var code) ? code.CodeName : null;
                isUS = string.Equals(countryCode, "USA", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                isUS = string.Equals(request.Country, "US", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(request.Country, "USA", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(request.Country, "United States", StringComparison.OrdinalIgnoreCase);
            }
            if (isUS)
                sb.AppendLine("[Order Online](https://www.medgyn.com/sign-in/)");
            sb.AppendLine("[Request a Quote](https://www.medgyn.com/request-a-quote/)");
            sb.AppendLine("[Speak to Sales Rep](chataction://GetRepersentativeByCountryOrState)");

            return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = sb.ToString(), Data = product };
        }
    }
}
