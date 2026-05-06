using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.DTOs;

namespace MedGyn.MedForce.Web.Controllers.Api
{
    [Authorize]
    [Route("api/chatbot")]
    public class ChatBotController : BaseApiController
    {
        private readonly ICustomerFacade _customerFacade;
        private readonly IChatBotFacade _chatBotFacade;

        public ChatBotController(ICustomerFacade customerFacade, IChatBotFacade chatBotFacade)
        {
            _customerFacade = customerFacade;
            _chatBotFacade = chatBotFacade;
        }

        /// <summary>
        /// Verifies if the customer exists by email and logs the chat entry.
        /// </summary>
        /// <param name="model">Customer chat details (name, email, state, country)</param>
        /// <returns>Verification result and log entry status</returns>
        [HttpPost, Route("logchatcustomer")]
        public async Task<IActionResult> LogChatCustomer([FromBody] CustomerChatLogModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Invalid customer details.");

            // Use facade to verify customer existence
            model.IsExistingCustomer = await _customerFacade.VerifyCustomerByEmailAsync(model.Email);

            // Log the chat entry
            await _chatBotFacade.LogCustomerChatAsync(model);

            return Ok(new { model.IsExistingCustomer });
        }
    }
}
