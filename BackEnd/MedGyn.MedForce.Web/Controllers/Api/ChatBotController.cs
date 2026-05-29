using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.DTOs;

namespace MedGyn.MedForce.Web.Controllers.Api
{
    [AllowAnonymous]
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

            // Existing customer is determined by whether they provided a Customer ID
            model.IsExistingCustomer = model.CustomerId.HasValue;

            if (model.CustomerId.HasValue && !_customerFacade.CustomerExists(model.CustomerId.Value))
                return BadRequest("Customer ID not found. Please check your ID and try again.");

            // Log the chat entry
            int id = await _chatBotFacade.LogCustomerChatAsync(model);

            return Ok(new {
                ChatLogId = id,
                model.IsExistingCustomer });
        }

        /// <summary>
        /// Chats with customer providing the appropriate result
        /// </summary>
        /// <param name="request">Chat Details(Message)</param>
        /// <returns>Returns text with information</returns>
        [HttpPost("chat")]
        public async Task<IActionResult>Chat([FromBody]CustomerChatRequestDTO request)
        {
            var response = await _chatBotFacade.ProcessMessage(request);
            return Ok(response);
        }

    }
}
