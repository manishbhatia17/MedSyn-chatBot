using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Data.Repositories;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Facades
{
    public class RepersentativeTerritoryFacade : IRepersentativeTerritoryFacade
    {
        private readonly IRepersentativeTerritoryService _service;
        public RepersentativeTerritoryFacade(IRepersentativeTerritoryService service)
        {
            _service = service;
        }

        public async Task<RepresentativeDTO> GetRepresentativeByCustomerChatLogId(int id)
        {
            RepresentativeTerritory representiveTerritoryData =  await _service.GetRepresentativeByCustomerChatLogId(id);
            RepresentativeDTO representativeData = new RepresentativeDTO()
            {
                Email = representiveTerritoryData.Representative.Email,
                Id = representiveTerritoryData.Representative.Id,
                Name = representiveTerritoryData.Representative.Name,
                Phone = representiveTerritoryData.Representative.Phone
            };
            return representativeData;
        }
    }
}
