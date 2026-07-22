using Medforce.Graph.Services.Interfaces;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Facades
{
    public class SalesTerritoryFacade : ISalesTerritoryFacade
    {
        private const string TerritoriesFilePath = "Contact Sales Team/Territories.xlsx";
        private const string CacheKey = "sales_territories";

        private readonly ISharePointListSearchService _sharePointService;
        private readonly IMemoryCache _memoryCache;

        public SalesTerritoryFacade(ISharePointListSearchService sharePointService, IMemoryCache memoryCache)
        {
            _sharePointService = sharePointService;
            _memoryCache = memoryCache;
        }

        public async Task<SalesTerritoryRepresentativeDTO> GetRepresentativeByLocationAsync(string state, string country)
        {
            var territories = await GetTerritoriesAsync();
            if (territories == null) return null;

            state = state?.Trim().ToLower();
            country = country?.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(state))
            {
                var exactStateMatch = territories.FirstOrDefault(t =>
                    t.States.Any(s => s.ToLower() == state));
                if (exactStateMatch != null) return ToDto(exactStateMatch);

                var partialStateMatch = territories.FirstOrDefault(t =>
                    t.States.Any(s => s.ToLower().Contains(state) || state.Contains(s.ToLower())));
                if (partialStateMatch != null) return ToDto(partialStateMatch);
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                var exactCountryMatch = territories.FirstOrDefault(t =>
                    t.Countries.Any(c => c.ToLower() == country));
                if (exactCountryMatch != null) return ToDto(exactCountryMatch);
            }

            return null;
        }

        private async Task<List<SalesTerritoryEntry>> GetTerritoriesAsync()
        {
            if (_memoryCache.TryGetValue(CacheKey, out List<SalesTerritoryEntry> cached))
                return cached;

            var fileBytes = await _sharePointService.GetFileContentAsync(TerritoriesFilePath);
            if (fileBytes == null) return null;

            var territories = ParseTerritories(fileBytes);
            _memoryCache.Set(CacheKey, territories, TimeSpan.FromHours(1));
            return territories;
        }

        private static List<SalesTerritoryEntry> ParseTerritories(byte[] fileBytes)
        {
            var entries = new List<SalesTerritoryEntry>();

            using (var package = new ExcelPackage(new MemoryStream(fileBytes)))
            {
                var sheet = package.Workbook.Worksheets[0];

                // Domestic territories occupy columns B-O: row 3 = territory name, row 4 = states,
                // row 5 = rep name, row 6 = phone, row 7 = email.
                for (var col = 2; col <= 15; col++)
                {
                    var territoryName = sheet.Cells[3, col].Text?.Trim();
                    if (string.IsNullOrWhiteSpace(territoryName)) continue;

                    entries.Add(new SalesTerritoryEntry
                    {
                        TerritoryName = territoryName,
                        States = SplitLines(sheet.Cells[4, col].Text),
                        Countries = new List<string>(),
                        Name = sheet.Cells[5, col].Text?.Trim(),
                        Phone = sheet.Cells[6, col].Text?.Trim(),
                        Email = sheet.Cells[7, col].Text?.Trim()
                    });
                }

                // International territories occupy columns B-I: row 15 = territory name, row 17 = countries,
                // row 18 = rep name, row 19 = phone, row 20 = email.
                for (var col = 2; col <= 9; col++)
                {
                    var territoryName = sheet.Cells[15, col].Text?.Trim();
                    if (string.IsNullOrWhiteSpace(territoryName)) continue;

                    entries.Add(new SalesTerritoryEntry
                    {
                        TerritoryName = territoryName,
                        States = new List<string>(),
                        Countries = SplitLines(sheet.Cells[17, col].Text),
                        Name = sheet.Cells[18, col].Text?.Trim(),
                        Phone = sheet.Cells[19, col].Text?.Trim(),
                        Email = sheet.Cells[20, col].Text?.Trim()
                    });
                }
            }

            return entries;
        }

        private static List<string> SplitLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();

            return text.Split('\n')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        private static SalesTerritoryRepresentativeDTO ToDto(SalesTerritoryEntry entry) => new SalesTerritoryRepresentativeDTO
        {
            TerritoryName = entry.TerritoryName,
            Name = entry.Name,
            Phone = entry.Phone,
            Email = entry.Email
        };

        private class SalesTerritoryEntry
        {
            public string TerritoryName { get; set; }
            public List<string> States { get; set; }
            public List<string> Countries { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
        }
    }
}
