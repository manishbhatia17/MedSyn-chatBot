using MedGyn.MedForce.Facade.Handlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MedGyn.MedForce.Facade.Factories.Interfaces;

namespace MedGyn.MedForce.Facade.Factories
{
	public class EmailbotCommandHandlerFactory : IEmailBotCommandHandlerFactory
	{
		IEnumerable<IEmailBotCommandHandler> _commandHandlers;
		public EmailbotCommandHandlerFactory(IEnumerable<IEmailBotCommandHandler> commandHandlers)
		{
			_commandHandlers = commandHandlers;
		}

		public IEmailBotCommandHandler GetCommandHandler(string functionName)
		{
			EmailBotCommandType commandType = GetCommandTypeFromFunctionName(functionName);

			return _commandHandlers.FirstOrDefault(Handlers => Handlers.CommandType == commandType);
		}

		private EmailBotCommandType GetCommandTypeFromFunctionName(string functionName)
		{
			// This is a simple mapping. You can enhance this logic based on your requirements.
			return functionName switch
			{
				"GetProductByName" => EmailBotCommandType.GetProductByName,
				"GetProductById" => EmailBotCommandType.GetProductById,
				"GetInvoice" => EmailBotCommandType.GetInvoice,
				"GetCustomerPO" => EmailBotCommandType.GetCustomerPO,
				"GetCustomerOrder" => EmailBotCommandType.GetCustomerOrder,
				"GetCustomerOrderByEmail" => EmailBotCommandType.GetCustomerOrderByEmail,
				_ => throw new ArgumentException($"Unknown function name: {functionName}")
			};
		}
	}
}
