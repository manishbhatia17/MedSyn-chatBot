using MedGyn.MedForce.Facade.Factories.Interfaces;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedGyn.MedForce.Facade.Factories
{
    public class CustomerChatBotCommandHandlerFactory : ICustomerChatBotCommandHandlerFactory
    {
        IEnumerable<ICustomerChatBotCommandHandler>
            _commandHandlers;

        public CustomerChatBotCommandHandlerFactory(
            IEnumerable<ICustomerChatBotCommandHandler>
                commandHandlers)
        {
            _commandHandlers = commandHandlers;
        }

        public ICustomerChatBotCommandHandler
           GetCommandHandler(
               string functionName)
        {
            CustomerChatBotCommandType commandType =
                GetCommandTypeFromFunctionName(
                    functionName);

            return _commandHandlers.FirstOrDefault(
                handlers =>
                    handlers.CommandType ==
                    commandType);
        }

        private CustomerChatBotCommandType
            GetCommandTypeFromFunctionName(
                string functionName)
        {
            return functionName switch
            {
                "GetProductByName" =>
                    CustomerChatBotCommandType
                        .GetProductByName,

                "GetProductById" =>
                    CustomerChatBotCommandType
                        .GetProductById,

                "GetInvoice" =>
                    CustomerChatBotCommandType
                        .GetInvoice,

                "GetCustomerPO" =>
                    CustomerChatBotCommandType
                        .GetCustomerPO,

                "GetCustomerOrder" =>
                    CustomerChatBotCommandType
                        .GetCustomerOrder,

                "GetCustomerOrderByEmail" =>
                    CustomerChatBotCommandType
                        .GetCustomerOrderByEmail,

                "GetRepersentativeByCountryOrState" =>
                    CustomerChatBotCommandType
                         .GetRepersentativeByCountryOrState,

                "LeaveMessageForMedGyn" =>
                    CustomerChatBotCommandType
                         .LeaveMessageForMedGyn,

                _ => (CustomerChatBotCommandType)(-1)
            };
        }
    }
}
