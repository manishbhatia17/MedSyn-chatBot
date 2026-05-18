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

            string prompt = $@"
Product Name: {product.ProductName}
Product ID: {product.ProductCustomID}
";

            string response =
                await _llmService.SummarizeContent(
                    prompt,
                    "You are a medical supplies assistant.");

            return new CustomerChatResponseDTO
            {
                FunctionName = CommandType.ToString(),
                Message = response,
                Data = product
            };
        }
    }
}
