using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetOrderStatusChatBotCommandHandler : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType => CustomerChatBotCommandType.GetOrderStatus;

        private readonly ICustomerOrderService _customerOrderService;

        public GetOrderStatusChatBotCommandHandler(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        public async Task<CustomerChatResponseDTO> HandleAsync(string[] parameters, CustomerChatRequestDTO request)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters[0]);

            if (!dict.TryGetValue("po_number", out string poNumber) || string.IsNullOrWhiteSpace(poNumber))
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Please provide your PO number to check the order status." };

            var order = await _customerOrderService.GetCustomerOrderChatStatus(poNumber, request.CustomerId);

            if (order == null)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Order not found." };

            var sb = new StringBuilder();
            sb.AppendLine($"Here is the current status for PO **{order.PONumber}**:");
            sb.AppendLine();
            sb.AppendLine($"**Status:** {order.Status}");
            sb.AppendLine($"**Approved:** {(order.IsApproved ? "Yes" : "No")}");
            sb.AppendLine($"**Shipped:** {(order.IsShipped ? "Yes" : "No")}");
            sb.AppendLine($"**Invoiced:** {(order.IsInvoiced ? "Yes" : "No")}");

            return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = sb.ToString(), Data = order };
        }
    }
}
