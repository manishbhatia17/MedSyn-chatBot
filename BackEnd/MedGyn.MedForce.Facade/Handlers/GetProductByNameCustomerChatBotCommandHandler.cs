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
        private readonly IVendorService _vendorService;
        private readonly ICodeService _codeService;
        private readonly ISharePointListSearchService _sharePointService;
        private readonly string _baseUrl;

        public GetProductByNameCustomerChatBotCommandHandler(
            IProductService productService,
            IVendorService vendorService,
            ICodeService codeService,
            ISharePointListSearchService sharePointService,
            IOptions<AppSettings> appSettings)
        {
            _productService = productService;
            _vendorService = vendorService;
            _codeService = codeService;
            _sharePointService = sharePointService;
            _baseUrl = appSettings.Value.Url?.TrimEnd('/');
        }

        public async Task<CustomerChatResponseDTO> HandleAsync(string[] parameters, CustomerChatRequestDTO request)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters[0]);

            if (!dict.TryGetValue("product_name", out string productName) || string.IsNullOrWhiteSpace(productName))
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Please provide a product name to search for." };

            var product = _productService.SearchProductByName(productName);

            if (product == null || product.ProductID == 0)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = $"No product found matching \"{productName}\"." };

            var stock = _productService.GetProductStockInfo(product.ProductID);
            var uomLookup       = _codeService.GetCodeLookupByType(CodeTypeEnum.UnitOfMeasure);
            var weightUomLookup = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipWeightUnit);
            var dimUomLookup    = _codeService.GetCodeLookupByType(CodeTypeEnum.ShipDimensionsUnit);

            var sb = new StringBuilder();
            sb.AppendLine($"Here is the information for **{product.ProductName}** (Product ID: {product.ProductCustomID}):");
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(product.Description))
            {
                var description = product.Description.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Trim();
                sb.AppendLine($"**About this Product:** {description}");
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(product.Manufacturer))
                sb.AppendLine($"**Manufacturer:** {product.Manufacturer}");

            if (request.CustomerId.HasValue)
            {
                if (product.PrimaryVendorID.HasValue)
                {
                    var vendor = _vendorService.GetVendor(product.PrimaryVendorID.Value);
                    if (vendor != null && !string.IsNullOrWhiteSpace(vendor.VendorName))
                        sb.AppendLine($"**Primary Vendor:** {vendor.VendorName}");
                }

                var additionalVendorIds = new[]
                {
                    product.AdditionalVendor1ID, product.AdditionalVendor2ID, product.AdditionalVendor3ID,
                    product.AdditionalVendor4ID, product.AdditionalVendor5ID, product.AdditionalVendor6ID
                };
                var additionalVendorNames = new List<string>();
                foreach (var vid in additionalVendorIds)
                {
                    if (!vid.HasValue) continue;
                    var v = _vendorService.GetVendor(vid.Value);
                    if (v != null && !string.IsNullOrWhiteSpace(v.VendorName))
                        additionalVendorNames.Add(v.VendorName);
                }
                if (additionalVendorNames.Count > 0)
                    sb.AppendLine($"**Additional Vendors:** {string.Join(", ", additionalVendorNames)}");
            }

            if (!string.IsNullOrWhiteSpace(product.Color))
                sb.AppendLine($"**Color:** {product.Color}");

            if (product.UnitOfMeasureCodeID.HasValue &&
                uomLookup.TryGetValue(product.UnitOfMeasureCodeID.Value, out var uomCode))
                sb.AppendLine($"**Unit of Measure:** {uomCode.CodeName}");

            if (request.CustomerId.HasValue)
            {
                if (product.PriceDomesticList.HasValue)
                    sb.AppendLine($"**List Price:** ${product.PriceDomesticList:F2}");

                if (product.PriceInternationalDistribution.HasValue)
                    sb.AppendLine($"**International Price:** ${product.PriceInternationalDistribution:F2}");
            }

            sb.AppendLine($"**Status:** {(product.IsDiscontinued ? "Discontinued" : "Available")}");

            if (product.SpecialOrderOnly == true)
                sb.AppendLine("**Special Order:** This product is available by special order only.");

            if (product.InternationalOnly == true)
                sb.AppendLine("**Availability:** International markets only.");

            if (product.ShipWeight.HasValue)
            {
                string weightUnit = product.ShipWeightUnitsCodeID.HasValue &&
                                    weightUomLookup.TryGetValue(product.ShipWeightUnitsCodeID.Value, out var wu)
                                    ? wu.CodeName : string.Empty;
                sb.AppendLine($"**Ship Weight:** {product.ShipWeight:F2}{(string.IsNullOrEmpty(weightUnit) ? "" : " " + weightUnit)}");
            }

            if (product.Length.HasValue || product.Width.HasValue || product.Depth.HasValue)
            {
                string dimUnit = product.ShipDimensionUnitsCodeID.HasValue &&
                                 dimUomLookup.TryGetValue(product.ShipDimensionUnitsCodeID.Value, out var du)
                                 ? du.CodeName : string.Empty;
                var dims = $"{product.Length ?? 0:F2} x {product.Width ?? 0:F2} x {product.Depth ?? 0:F2}";
                sb.AppendLine($"**Dimensions (L x W x D):** {dims}{(string.IsNullOrEmpty(dimUnit) ? "" : " " + dimUnit)}");
            }

            if (stock != null && request.CustomerId.HasValue)
            {
                sb.AppendLine($"**On Hand:** {stock.OnHand}");
                sb.AppendLine($"**Committed:** {stock.UnfilledCOs}");
                sb.AppendLine($"**Net Quantity:** {stock.NetQuantity}");
            }

            if (!string.IsNullOrWhiteSpace(product.Notes))
                sb.AppendLine($"**Notes:** {product.Notes}");

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
            sb.AppendLine("[Order Online](https://www.medgyn.com/sign-in/)");
            sb.AppendLine("[Speak to Sales Rep](chataction://GetRepersentativeByCountryOrState)");

            return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = sb.ToString(), Data = product };
        }
    }
}
