using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Facades;
using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Web.Controllers.Api
{
    [ApiController]
    [Route("api/repersentativeterritory")]
    public class RepersentativeTerritoryController : BaseApiController
    {
        private readonly IRepersentativeTerritoryFacade _repersentativeTerritoryFacade;
        public RepersentativeTerritoryController(IRepersentativeTerritoryFacade repersentativeTerritoryFacade)
        {
            _repersentativeTerritoryFacade = repersentativeTerritoryFacade;
        }


        /// <summary>
        /// Gets the repersentative details from representative id.
        /// </summary>
        /// <param name="model">Representative details (name, email, phone)</param>
        /// <returns>Representative details</returns>
        [HttpGet, Route("GetRepresentativeByCustomerChatLogId")]
        public async Task<IActionResult> GetRepresentativeByCustomerChatLogId(int id)
        {
            var rep =  await _repersentativeTerritoryFacade.GetRepresentativeByCustomerChatLogId(id);

            return Ok(rep);
        }

    }
}
