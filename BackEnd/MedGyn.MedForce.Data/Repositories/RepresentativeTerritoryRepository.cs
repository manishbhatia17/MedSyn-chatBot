using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Data.Repositories
{
    public class RepresentativeTerritoryRepository : IRepresentativeTerritoryRepository
    {
        private readonly IDbContext _dbContext;
        public RepresentativeTerritoryRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RepresentativeTerritory>
    GetRepresentativeByLocationAsync(
        string? region,
        string? state,
        string? country)
        {
            state =
                state?.Trim().ToLower();

            country =
                country?.Trim().ToLower();

            // -----------------------------------
            // 1. EXACT STATE MATCH
            // -----------------------------------

            if (!string.IsNullOrWhiteSpace(state))
            {
                var exactStateMatch =
                    await _dbContext
                        .RepersentativeTerritories
                        .Where(rt =>
                            rt.Territory.States.Any(s =>
                                s.Name.ToLower() == state))
                        .FirstOrDefaultAsync();

                if (exactStateMatch != null)
                {
                    return exactStateMatch;
                }

                // -----------------------------------
                // 2. PARTIAL STATE MATCH
                // -----------------------------------

                var partialStateMatch =
                    await _dbContext
                        .RepersentativeTerritories
                        .Where(rt =>
                            rt.Territory.States.Any(s =>
                                s.Name.ToLower()
                                    .Contains(state)
                                || state.Contains(
                                    s.Name.ToLower())))
                        .FirstOrDefaultAsync();

                if (partialStateMatch != null)
                {
                    return partialStateMatch;
                }
            }

            // -----------------------------------
            // 3. EXACT COUNTRY MATCH
            // -----------------------------------

            if (!string.IsNullOrWhiteSpace(country))
            {
                var exactCountryMatch =
                    await _dbContext
                        .RepersentativeTerritories
                        .Where(rt =>
                            rt.Territory.Countries.Any(c =>
                                c.Name.ToLower() == country))
                        .FirstOrDefaultAsync();

                if (exactCountryMatch != null)
                {
                    return exactCountryMatch;
                }
            }

            return null;
        }
    }
}
