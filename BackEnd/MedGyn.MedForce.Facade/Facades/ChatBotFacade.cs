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
                State = model.State,
                Country = model.Country,
                IsExistingCustomer = model.IsExistingCustomer,
                CustomerId = model.CustomerId
            };
           return await _chatBotService.LogCustomerChatAsync(contract);
        }

        public async Task<CustomerChatResponseDTO> ProcessMessage(CustomerChatRequestDTO request)
        {
            string functionJson = File.ReadAllText(@".\wwwroot\js\ChatGPTMCPServerJson.json");

            var llmResponse = await _llmService.AgentFunction(request.Message,functionJson,"You are a MedGyn customer chatbot assistant.");

            if (llmResponse == null)
            {
                return new CustomerChatResponseDTO
                {
                    Message =
                        "Unable to process request."
                };
            }

            var handler = _handlerFactory.GetCommandHandler(llmResponse.FunctionName);

            if (handler == null)
            {
                return new CustomerChatResponseDTO
                {
                    Message =
                        "No handler found."
                };
            }

            return await handler.HandleAsync(llmResponse.Parameters.ToArray(),request);
        }
          
    }
}
