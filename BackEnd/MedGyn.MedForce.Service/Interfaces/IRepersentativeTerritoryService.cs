using MedGyn.MedForce.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Service.Interfaces
{
    public interface IRepersentativeTerritoryService
    {
        Task<RepresentativeTerritory> GetRepresentativeByCustomerChatLogId(int id);
        Task<RepresentativeTerritory> GetRepresentativeByLocationAsync(string state, string country);
    }
}
