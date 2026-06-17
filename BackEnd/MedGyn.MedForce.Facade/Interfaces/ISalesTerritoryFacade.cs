using MedGyn.MedForce.Facade.DTOs;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Interfaces
{
    public interface ISalesTerritoryFacade
    {
        Task<SalesTerritoryRepresentativeDTO> GetRepresentativeByLocationAsync(string state, string country);
    }
}
