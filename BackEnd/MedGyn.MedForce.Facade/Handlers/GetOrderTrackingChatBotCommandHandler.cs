using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetOrderTrackingChatBotCommandHandler : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType => CustomerChatBotCommandType.GetOrderTracking;

        private readonly ICustomerOrderService _customerOrderService;

        public GetOrderTrackingChatBotCommandHandler(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        public async Task<CustomerChatResponseDTO> HandleAsync(string[] parameters, CustomerChatRequestDTO request)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters[0]);

            if (!request.CustomerId.HasValue)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "To view order information, please verify your account first by providing your customer ID." };

            if (!dict.TryGetValue("po_number", out string poNumber) || string.IsNullOrWhiteSpace(poNumber))
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Please provide your PO number to check the tracking information." };

            var order = await _customerOrderService.GetCustomerOrderChatStatus(poNumber, request.CustomerId);

            if (order == null)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Order not found." };

            var sb = new StringBuilder();
            sb.AppendLine($"Here is the shipping information for PO **{order.PONumber}**:");
            sb.AppendLine();
            sb.AppendLine($"**Shipped:** {(order.IsShipped ? "Yes" : "No")}");
            sb.AppendLine(!string.IsNullOrEmpty(order.Carrier)
                ? $"**Carrier:** {order.Carrier}"
                : "**Carrier:** Not yet assigned.");

            if (!string.IsNullOrEmpty(order.TrackingNumber))
            {
                sb.AppendLine($"**Tracking Number:** {order.TrackingNumber}");
                string carrier = order.Carrier?.ToLower() ?? string.Empty;
                if (carrier.Contains("ups"))
                    sb.AppendLine($"[Track UPS Shipment](https://www.ups.com/track?tracknum={order.TrackingNumber})");
                else if (carrier.Contains("fedex"))
                    sb.AppendLine($"[Track FedEx Shipment](https://www.fedex.com/fedextrack/?tracknumbers={order.TrackingNumber})");
            }
            else
            {
                sb.AppendLine("**Tracking Number:** Not yet available.");
            }

            return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = sb.ToString(), Data = order };
        }
    }
}
