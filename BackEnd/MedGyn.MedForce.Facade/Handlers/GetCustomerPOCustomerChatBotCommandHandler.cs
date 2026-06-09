using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetCustomerPOCustomerChatBotCommandHandler : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType => CustomerChatBotCommandType.GetCustomerPO;

        private readonly ICustomerOrderService _customerOrderService;

        public GetCustomerPOCustomerChatBotCommandHandler(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        public async Task<CustomerChatResponseDTO> HandleAsync(string[] parameters, CustomerChatRequestDTO request)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters[0]);

            if (!request.CustomerId.HasValue)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "To view order information, please verify your account first by providing your customer ID." };

            if (!dict.TryGetValue("po_number", out string poNumber) || string.IsNullOrWhiteSpace(poNumber))
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Please provide your PO number to look up the order." };

            var order = await _customerOrderService.GetCustomerOrderChatStatus(poNumber, request.CustomerId);

            if (order == null)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Purchase Order not found." };

            var sb = new StringBuilder();
            sb.AppendLine($"Here are the details for PO {order.PONumber}:");
            sb.AppendLine();
            sb.AppendLine($"**Order:** {order.CustomerOrderCustomID}");
            sb.AppendLine($"**Status:** {order.Status}");

            if (!string.IsNullOrEmpty(order.InvoiceNumber))
                sb.AppendLine($"**Invoice:** {order.InvoiceNumber}");

            sb.AppendLine();

            if (!string.IsNullOrEmpty(order.AttachmentURI))
                sb.AppendLine($"[View Purchase Order]({order.AttachmentURI})");

            return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = sb.ToString(), Data = order };
        }
    }
}
