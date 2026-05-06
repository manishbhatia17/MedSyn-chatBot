using System.Threading.Tasks;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Repositories
{
    public class ChatBotRepository : IChatBotRepository
    {
        private readonly IDbContext _dbContext;
        public ChatBotRepository(IDbContext context)
        {
            _dbContext = context;
        }

        public async Task<int> AddCustomerChatLogAsync(CustomerChatLog log)
        {
            _dbContext.BeginTransaction();
            _dbContext.Save(log);
            _dbContext.Commit();

            return log.Id;
        }
    }
}
