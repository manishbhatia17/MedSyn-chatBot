using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetOrderStatusChatBotCommandHandler : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType => CustomerChatBotCommandType.GetOrderStatus;

        private readonly ICustomerOrderService _customerOrderService;
        private readonly string _baseUrl;

        public GetOrderStatusChatBotCommandHandler(
            ICustomerOrderService customerOrderService,
            IOptions<AppSettings> appSettings)
        {
            _customerOrderService = customerOrderService;
            _baseUrl = appSettings.Value.Url?.TrimEnd('/');
        }

        public async Task<CustomerChatResponseDTO> HandleAsync(string[] parameters, CustomerChatRequestDTO request)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters[0]);

            if (!request.CustomerId.HasValue)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "To view order information, please verify your account first by providing your customer ID." };

            if (!dict.TryGetValue("po_number", out string poNumber) || string.IsNullOrWhiteSpace(poNumber))
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "Please provide your PO number to check the order status." };

            var order = await _customerOrderService.GetCustomerOrderChatStatus(poNumber, request.CustomerId);

            if (order == null)
                return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = "PO not found." };

            var shipments = await _customerOrderService.GetOrderShipmentsAsync(poNumber, request.CustomerId);
            var invoicedShipments = shipments.Where(s => s.IsInvoiced).ToList();
            var allInvoiced  = shipments.Count > 0 && invoicedShipments.Count == shipments.Count;
            var partlyFilled = shipments.Count > 0 && invoicedShipments.Count > 0 && invoicedShipments.Count < shipments.Count;

            var sb = new StringBuilder();

            if (allInvoiced)
                sb.AppendLine($"Your order for PO **{order.PONumber}** is invoiced.");
            else if (partlyFilled)
                sb.AppendLine($"Your order for PO **{order.PONumber}** is partly filled.");
            else
                sb.AppendLine($"Your order for PO **{order.PONumber}** is processing.");

            if (invoicedShipments.Count > 0 && !string.IsNullOrWhiteSpace(_baseUrl))
            {
                sb.AppendLine();
                foreach (var shipment in invoicedShipments)
                {
                    var label = !string.IsNullOrWhiteSpace(shipment.InvoiceNumber)
                        ? $"Invoice #{shipment.InvoiceNumber}"
                        : "View Invoice";
                    sb.AppendLine($"[{label}]({_baseUrl}/api/chatbot/invoice/{shipment.ShipmentId}/{request.ChatLogId})");
                }
            }

            return new CustomerChatResponseDTO { FunctionName = CommandType.ToString(), Message = sb.ToString(), Data = order };
        }
    }
}
