using MedGyn.MedForce.Facade.Handlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.Factories.Interfaces
{
    public interface ICustomerChatBotCommandHandlerFactory
    {
        ICustomerChatBotCommandHandler GetCommandHandler(string functionName);
    }
}
