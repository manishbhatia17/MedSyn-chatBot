using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Data.Interfaces
{
    public interface IRepresentativeTerritoryRepository
    {
        Task<RepresentativeTerritory> GetRepresentativeByLocationAsync(string? regionId,string? countryId,string? stateId);
    }
}
