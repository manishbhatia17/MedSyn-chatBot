using Medforce.Graph.Services.Models;
using Medgyn.Meforce.LLMAgent.LLMAgents;
using MedGyn.MedForce.Facade.Factories;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using MedGyn.MedForce.Service.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
	public class GetCustomerOrderByEmailCommandHandler : IEmailBotCommandHandler
	{

		private readonly ICustomerOrderService _customerOrderService;
		private readonly ICustomerService _customerService;
		private readonly Medforce.Graph.Services.Interfaces.IEmailService _emailService;
		private readonly ILLMAgent _lLMAgent;

		public EmailBotCommandType CommandType => EmailBotCommandType.GetCustomerOrderByEmail;
		public GetCustomerOrderByEmailCommandHandler(ICustomerOrderService customerOrderService, ICustomerService customerService, Medforce.Graph.Services.Interfaces.IEmailService emailService, ILLMAgent agent)
		{
			_lLMAgent = agent;
			_customerOrderService = customerOrderService;
			_emailService = emailService;
			_customerService = customerService;
		}
		public async Task HandleCommandAsync(string[] parameters, string fromEmail, string inboxId, string emailId)
		{
			var customer = _customerService.GetCustomerByEmail(fromEmail);
			var orders = _customerOrderService.GetCustomerOrdersByCustomerId(customer.CustomerID);

			string orderListDetails = $"Found {orders.Count} orders for customer associated with email {fromEmail}.";
			foreach (var order in orders)
			{
				orderListDetails += $"Order Number: {order.CustomerOrderCustomID},  OrderDate: {order.SubmitDate} ";
			}

			string emailBody = await _lLMAgent.SummarizeContent(orderListDetails, "You are a medical supplies sales associate and the customer orders that were requested by email were found, please create response to the requestor with a list of the orders supplied and ask the requester what order they are einquirieng out. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");

			EmailModel emailModel = new EmaiModelFactory().BuildReplyEmail(emailBody, emailId);

			await _emailService.CreateDraftReply(inboxId, emailModel);
		}
	}
}
