using System;
using System.IO;
using System.Threading.Tasks;
using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Factories.Interfaces;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Facade.Facades
{
    public class ChatBotFacade : IChatBotFacade
    {
        private readonly IChatBotService _chatBotService;
        private readonly ILLMService _llmService;
        private readonly ICustomerChatBotCommandHandlerFactory _handlerFactory;

        public ChatBotFacade(IChatBotService chatBotService,ILLMService llmService, ICustomerChatBotCommandHandlerFactory handlerFactory)
        {
            _chatBotService = chatBotService;
            _llmService = llmService;
            _handlerFactory = handlerFactory;
        }
        public async Task<int> LogCustomerChatAsync(CustomerChatLogModel model)
        {
            var contract = new MedGyn.MedForce.Service.Contracts.CustomerChatLogContract
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                State = model.State,
                Country = model.Country,
                IsExistingCustomer = model.IsExistingCustomer,
                CustomerId = model.CustomerId
            };
           return await _chatBotService.LogCustomerChatAsync(contract);
        }

        public async Task<CustomerChatResponseDTO> ProcessMessage(CustomerChatRequestDTO request)
        {
            try
            {
                string functionJson = File.ReadAllText(@".\wwwroot\js\ChatGPTMCPServerJson.json");

                var llmResponse = await _llmService.AgentFunction(
                    request.Message,
                    functionJson,
                    "You are a helpful MedGyn customer support assistant. Use the available functions to answer the customer's question.");

                if (llmResponse == null)
                {
                    return new CustomerChatResponseDTO
                    {
                        Message = "I wasn't able to understand your request. Could you please rephrase your question?"
                    };
                }

                var handler = _handlerFactory.GetCommandHandler(llmResponse.FunctionName);

                if (handler == null)
                {
                    return new CustomerChatResponseDTO
                    {
                        Message = "I'm not sure how to help with that. Please select an option from the menu or ask about products, orders, or your sales representative."
                    };
                }

                return await handler.HandleAsync(llmResponse.Parameters.ToArray(), request);
            }
            catch (Exception ex)
            {
                return new CustomerChatResponseDTO
                {
                    Message = $"Error: {ex.Message}"
                };
            }
        }
          
    }
}
