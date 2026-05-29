using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetOrderInvoiceChatBotCommandHandler : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType => CustomerChatBotCommandType.GetOrderInvoice;

        private readonly ICustomerOrderService _customerOrderService;

        public GetOrderInvoiceChatBotCommandHandler(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        public async Task<CustomerChatResponseDTO> HandleAsync(string[] parameters, CustomerChatRequestDTO request)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters[0]);

            if (!dict.TryGetValue("po_number", out string poNumber) || string.IsNullOrWhiteSpace(poNumber))
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Please provide your PO number to retrieve the invoice." };

            var order = await _customerOrderService.GetCustomerOrderChatStatus(poNumber, request.CustomerId);

            if (order == null)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Order not found." };

            var sb = new StringBuilder();
            sb.AppendLine($"Here are the invoice details for PO **{order.PONumber}**:");
            sb.AppendLine();
            sb.AppendLine(!string.IsNullOrEmpty(order.InvoiceNumber)
                ? $"**Invoice Number:** {order.InvoiceNumber}"
                : "**Invoice Number:** Not yet generated.");

            if (order.InvoiceDate.HasValue)
                sb.AppendLine($"**Invoice Date:** {order.InvoiceDate:MMMM dd, yyyy}");

            sb.AppendLine(!string.IsNullOrEmpty(order.AttachmentURI)
                ? $"[Download Invoice]({order.AttachmentURI})"
                : "**Invoice Download:** Not yet available.");

            return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = sb.ToString(), Data = order };
        }
    }
}
