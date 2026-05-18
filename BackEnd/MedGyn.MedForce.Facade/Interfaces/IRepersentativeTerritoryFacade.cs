using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Data.Repositories;
using MedGyn.MedForce.Facade.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Interfaces
{
    public interface IRepersentativeTerritoryFacade
    {
        Task<RepresentativeDTO> GetRepresentativeByCustomerChatLogId(int id);
    }
}
