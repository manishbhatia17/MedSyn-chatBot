using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Data.Repositories;
using MedGyn.MedForce.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Service.Services
{
    public class RepresentativeTerritoryService : IRepersentativeTerritoryService
    {
        private readonly IRepresentativeTerritoryRepository _repository;
        private readonly IChatBotService _chatBotService;
        public RepresentativeTerritoryService(IRepresentativeTerritoryRepository repository, IChatBotService chatBotService)
        {
            _repository = repository;
            _chatBotService = chatBotService;
        }

        public async Task<RepresentativeTerritory> GetRepresentativeByCustomerChatLogId(int id)
        {
            var customerchatlog = await _chatBotService.GetCustomerChatLogAsync(id);

            //No region so we can send null
            return await _repository.GetRepresentativeByLocationAsync(null,customerchatlog.State,customerchatlog.Country);
        }

        public async Task<RepresentativeTerritory> GetRepresentativeByLocationAsync(string state,string country)
        {
            return await _repository.GetRepresentativeByLocationAsync(null,state,country);
        }
    }
}
