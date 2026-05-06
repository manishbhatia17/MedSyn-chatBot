using MedGyn.MedForce.Facade.Handlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.Factories.Interfaces
{
	public interface IEmailBotCommandHandlerFactory
	{
		IEmailBotCommandHandler GetCommandHandler(string functionName);
	}
}
