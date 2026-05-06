using System;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
    public class ChatBotService : IChatBotService
    {
        private readonly IChatBotRepository _chatBotRepository;
        public ChatBotService(IChatBotRepository chatBotRepository)
        {
            _chatBotRepository = chatBotRepository;
        }
        public async Task LogCustomerChatAsync(CustomerChatLogContract model)
        {
            var entity = new CustomerChatLog
            {
                Name = model.Name,
                Email = model.Email,
                State = model.State,
                Country = model.Country,
                IsExistingCustomer = model.IsExistingCustomer,
                CustomerId = model.CustomerId,
                CreatedAt = DateTime.UtcNow
            };
            await _chatBotRepository.AddCustomerChatLogAsync(entity);
        }
    }
}
