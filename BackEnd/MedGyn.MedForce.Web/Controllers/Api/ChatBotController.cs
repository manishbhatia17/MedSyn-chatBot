using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.DTOs;
using Medforce.Graph.Services.Interfaces;
using MedGyn.MedForce.Web.Middleware;

namespace MedGyn.MedForce.Web.Controllers.Api
{
    [AllowAnonymous]
    [Route("api/chatbot")]
    public class ChatBotController : BaseApiController
    {
        private readonly ICustomerFacade _customerFacade;
        private readonly IChatBotFacade _chatBotFacade;
        private readonly ISharePointListSearchService _sharePointService;

        public ChatBotController(ICustomerFacade customerFacade, IChatBotFacade chatBotFacade, ISharePointListSearchService sharePointService)
        {
            _customerFacade = customerFacade;
            _chatBotFacade = chatBotFacade;
            _sharePointService = sharePointService;
        }

        /// <summary>
        /// Verifies if the customer exists by email and logs the chat entry.
        /// </summary>
        /// <param name="model">Customer chat details (name, email, state, country)</param>
        /// <returns>Verification result and log entry status</returns>
        [HttpPost, Route("logchatcustomer")]
        [TypeFilter(typeof(ChatWidgetAccessFilter))]
        public async Task<IActionResult> LogChatCustomer([FromBody] CustomerChatLogModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Invalid customer details.");

            // Existing customer is determined by whether they provided a Customer ID
            model.IsExistingCustomer = model.CustomerId.HasValue;

            if (model.CustomerId.HasValue && !_customerFacade.CustomerExistsWithEmail(model.CustomerId.Value, model.Email))
                return BadRequest("Customer ID and email address do not match. Please check your details and try again.");

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
        [TypeFilter(typeof(ChatWidgetAccessFilter))]
        public async Task<IActionResult>Chat([FromBody]CustomerChatRequestDTO request)
        {
            var response = await _chatBotFacade.ProcessMessage(request);
            return Ok(response);
        }

        /// <summary>
        /// Returns the invoice PDF for a shipment, validated against the customer's chat session.
        /// </summary>
        [HttpGet("invoice/{shipmentId}/{chatLogId}")]
        public async Task<IActionResult> GetInvoice(int shipmentId, int chatLogId)
        {
            var pdf = await _chatBotFacade.GetChatbotInvoicePdfAsync(shipmentId, chatLogId);
            if (pdf == null)
                return Unauthorized();

            Response.Headers.Add("Content-Disposition", new ContentDisposition
            {
                FileName = "Invoice.pdf",
                Inline = true
            }.ToString());

            return File(pdf, "application/pdf");
        }

        /// <summary>
        /// Returns the product brochure PDF, fetched from SharePoint via the backend's app credentials.
        /// </summary>
        [HttpGet("brochure/{productId}")]
        public async Task<IActionResult> GetBrochure(string productId)
        {
            var doc = await _sharePointService.GetProductDocumentContentAsync(productId, "Product Brochures");
            if (doc == null)
                return NotFound();

            Response.Headers.Add("Content-Disposition", new ContentDisposition
            {
                FileName = doc.Value.FileName,
                Inline = false
            }.ToString());

            return File(doc.Value.Content, "application/pdf");
        }

        /// <summary>
        /// Returns the product IFU PDF, fetched from SharePoint via the backend's app credentials.
        /// </summary>
        [HttpGet("ifu/{productId}")]
        public async Task<IActionResult> GetIfu(string productId)
        {
            var doc = await _sharePointService.GetProductDocumentContentAsync(productId, "Product IFUs");
            if (doc == null)
                return NotFound();

            Response.Headers.Add("Content-Disposition", new ContentDisposition
            {
                FileName = doc.Value.FileName,
                Inline = false
            }.ToString());

            return File(doc.Value.Content, "application/pdf");
        }

        /// <summary>
        /// Returns the product manual PDF, fetched from SharePoint via the backend's app credentials.
        /// </summary>
        [HttpGet("manual/{productId}")]
        public async Task<IActionResult> GetManual(string productId)
        {
            var doc = await _sharePointService.GetProductDocumentContentAsync(productId, "Product Manuals");
            if (doc == null)
                return NotFound();

            Response.Headers.Add("Content-Disposition", new ContentDisposition
            {
                FileName = doc.Value.FileName,
                Inline = false
            }.ToString());

            return File(doc.Value.Content, "application/pdf");
        }

    }
}
