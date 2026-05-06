using System.Threading.Tasks;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Interfaces
{
    public interface IChatBotRepository
    {
        Task<int> AddCustomerChatLogAsync(CustomerChatLog log);
    }
}
