using MedGyn.MedForce.Facade.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers.Interfaces
{
    public enum CustomerChatBotCommandType
    {
        GetProductByName,
        GetProductById,
        GetInvoice,
        GetCustomerPO,
        GetCustomerOrder,
        GetCustomerOrderByEmail,
        GetRepersentativeByCountryOrState,
        LeaveMessageForMedGyn,
        GetOrderStatus,
        GetOrderInvoice,
        GetOrderTracking
    }
    public interface ICustomerChatBotCommandHandler
    {
        CustomerChatBotCommandType CommandType { get; }

        Task<CustomerChatResponseDTO> HandleAsync(
            string[] parameters,
            CustomerChatRequestDTO request);

    }
}
