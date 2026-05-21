using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetProductByNameCustomerChatBotCommandHandler : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType =>
            CustomerChatBotCommandType.GetProductByName;

        private readonly IProductService
            _productService;

        private readonly ILLMService
            _llmService;

        public GetProductByNameCustomerChatBotCommandHandler(
            IProductService productService,
            ILLMService llmService)
        {
            _productService = productService;
            _llmService = llmService;
        }

        public async Task<CustomerChatResponseDTO>HandleAsync(string[] parameters,CustomerChatRequestDTO request)
        {
            var dict =
                JsonConvert.DeserializeObject<
                    Dictionary<string, string>>(
                        parameters[0]);

            string productName =
                dict["product_name"];

            var product =
                _productService
                    .SearchProductByName(productName);

            if (product == null)
            {
                return new CustomerChatResponseDTO
                {
                    FunctionName = CommandType.ToString(),
                    Message = "Product not found."
                };
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Product Name: {product.ProductName}");
            sb.AppendLine($"Product ID: {product.ProductCustomID}");

            if (!string.IsNullOrWhiteSpace(product.Description))
                sb.AppendLine($"Description: {product.Description}");

            if (!string.IsNullOrWhiteSpace(product.Manufacturer))
                sb.AppendLine($"Manufacturer: {product.Manufacturer}");

            if (!string.IsNullOrWhiteSpace(product.Notes))
                sb.AppendLine($"Notes: {product.Notes}");

            if (product.PriceDomesticList.HasValue)
                sb.AppendLine($"List Price: ${product.PriceDomesticList:F2}");

            sb.AppendLine($"Status: {(product.IsDiscontinued ? "Discontinued" : "Available")}");

            if (!string.IsNullOrWhiteSpace(product.PrimaryImageURI))
                sb.AppendLine($"Product Image: {product.PrimaryImageURI}");

            string systemPrompt =
                "You are a MedGyn medical supplies sales assistant. " +
                "Provide a clear and helpful summary of this product using the information provided. " +
                "At the end of your response, ask the customer if they would like to place an order " +
                "online at www.medgyn.com or if they would like to speak with their sales representative.";

            string response =
                await _llmService.SummarizeContent(
                    sb.ToString(),
                    systemPrompt);

            return new CustomerChatResponseDTO
            {
                FunctionName = CommandType.ToString(),
                Message = response,
                Data = product
            };
        }
    }
}
