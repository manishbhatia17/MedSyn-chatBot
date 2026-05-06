using System.Threading.Tasks;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Facade.Facades
{
    public class ChatBotFacade : IChatBotFacade
    {
        private readonly IChatBotService _chatBotService;
        public ChatBotFacade(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }
        public async Task LogCustomerChatAsync(CustomerChatLogModel model)
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
            await _chatBotService.LogCustomerChatAsync(contract);
        }
    }
}
