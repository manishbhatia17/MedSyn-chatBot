using System.Threading.Tasks;
using MedGyn.MedForce.Facade.DTOs;

namespace MedGyn.MedForce.Facade.Interfaces
{
    public interface IChatBotFacade
    {
        Task LogCustomerChatAsync(CustomerChatLogModel model);
    }
}
