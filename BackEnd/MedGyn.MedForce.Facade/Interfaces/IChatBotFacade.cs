using System.Threading.Tasks;
using MedGyn.MedForce.Facade.DTOs;

namespace MedGyn.MedForce.Facade.Interfaces
{
    public interface IChatBotFacade
    {
        Task<int> LogCustomerChatAsync(CustomerChatLogModel model);
        Task<CustomerChatResponseDTO> ProcessMessage(CustomerChatRequestDTO request);
    }
}
