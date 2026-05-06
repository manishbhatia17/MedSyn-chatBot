using Medforce.Graph.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers.Interfaces
{
	public enum EmailBotCommandType
	{
		GetProductByName = 0,
		GetProductById = 1,
		GetInvoice = 2,
		GetCustomerPO = 3,
		GetCustomerOrder = 4,
		GetCustomerOrderByEmail = 5
	}
	public interface IEmailBotCommandHandler
	{
		EmailBotCommandType CommandType { get; }

		Task HandleCommandAsync(string[] parameters, string fromEmail, string inboxId, string emailId);

	}
}
