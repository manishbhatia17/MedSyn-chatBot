using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetCustomerPOCustomerChatBotCommandHandler
        : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType =>
            CustomerChatBotCommandType.GetCustomerPO;

        private readonly ICustomerOrderService
            _customerOrderService;

        private readonly ILLMService
            _llmService;

        public GetCustomerPOCustomerChatBotCommandHandler(
            ICustomerOrderService customerOrderService,
            ILLMService llmService)
        {
            _customerOrderService =
                customerOrderService;

            _llmService =
                llmService;
        }

        public async Task<CustomerChatResponseDTO>HandleAsync(string[] parameters,CustomerChatRequestDTO request)
        {
            var dict =
                JsonConvert.DeserializeObject
                    <Dictionary<string, string>>(
                        parameters[0]);

            string poNumber =
                dict["po_number"];

            var order =
                await _customerOrderService
                    .GetCustomerOrderChatStatus(
                        poNumber);

            if (order == null)
            {
                return new CustomerChatResponseDTO
                {
                    FunctionName =
                        CommandType.ToString(),

                    Message =
                        "Purchase Order not found."
                };
            }

            StringBuilder sb =
                new StringBuilder();

            sb.AppendLine(
                $"PO Number: {order.PONumber}");

            sb.AppendLine(
                $"Status: {order.Status}");

            sb.AppendLine(
                $"Approved: {order.IsApproved}");

            sb.AppendLine(
                $"Shipped: {order.IsShipped}");

            sb.AppendLine(
                $"Invoiced: {order.IsInvoiced}");

            if (!string.IsNullOrEmpty(order.TrackingNumber))
            {
                sb.AppendLine(
                    $"Tracking Number: {order.TrackingNumber}");
            }

            if (order.InvoiceNumber != null)
            {
                sb.AppendLine(
                    $"Invoice Number: {order.InvoiceNumber}");
            }

            if(!string.IsNullOrEmpty(order.Carrier))
            {
                sb.AppendLine(
                    $"Carrier Name: {order.Carrier}");
            }

            if (!string.IsNullOrEmpty(order.AttachmentURI))
            {
                sb.AppendLine(
                    $"Attachment: {order.AttachmentURI}");
            }

            string aiResponse =
                await _llmService
                    .SummarizeContent(
                        sb.ToString(),
                        "You are a medical supplies customer support assistant. Explain order status clearly and professionally.");

            return new CustomerChatResponseDTO
            {
                FunctionName =
                    CommandType.ToString(),

                Message =
                    aiResponse,

                Data = order
            };
        }
    }
}