using System.Threading.Tasks;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
    public interface IChatBotService
    {
        Task<int> LogCustomerChatAsync(CustomerChatLogContract model);

        Task<CustomerChatLog> GetCustomerChatLogAsync(int id);
    }
}
