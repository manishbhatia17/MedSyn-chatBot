using System.Threading.Tasks;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
    public interface IChatBotService
    {
        Task LogCustomerChatAsync(CustomerChatLogContract model);
    }
}
