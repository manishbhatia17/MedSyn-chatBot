using Azure;
using Medforce.Graph.Services.Models;
using Medgyn.Meforce.LLMAgent.LLMAgents;
using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.Factories;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
	public class GetCustomerOrderCommandHandler : IEmailBotCommandHandler
	{
		public EmailBotCommandType CommandType => EmailBotCommandType.GetCustomerOrder;

		private readonly ILLMService _lLMAgent;
		private readonly ICustomerOrderService _customerOrderService;
		private readonly Medforce.Graph.Services.Interfaces.IEmailService _emailService;

		public GetCustomerOrderCommandHandler(ILLMService agent, ICustomerOrderService customerOrderService, Medforce.Graph.Services.Interfaces.IEmailService emailService)
		{
			_lLMAgent = agent;
			_customerOrderService = customerOrderService;
			_emailService = emailService;
		}

		public async Task HandleCommandAsync(string[] parameters, string fromEmail, string inboxId, string emailId)
		{
			string emailBody = "";
			var orderId = parameters[0];
			Dictionary<string, string> orderDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(orderId);
			var customerOrderById = _customerOrderService.GetCustomerOrderByCONumber(orderDict["order_number"]);
			if (customerOrderById == null || customerOrderById.CustomerOrderCustomID != orderDict["order_number"])
			{
				emailBody = await _lLMAgent.SummarizeContent("Customer Order not found.", "You are a medical supplies sales associate and the customer order that was requested by email was not found, please create response to the requestor. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");
			}
			else
			{
				string orderDetails = $"Customer Order found: {customerOrderById.CustomerOrderCustomID} with ID {customerOrderById.CustomerOrderID}. ";
				orderDetails += "Products in the order: ";
				var orderProducts = _customerOrderService.GetCustomerOrderProducts(customerOrderById.CustomerOrderID);
				if (orderProducts != null)
				{
					foreach (var productItem in orderProducts)
					{
						orderDetails += $"Product: {productItem.ProductName}, Quantity: {productItem.Quantity}. ";
					}
				}
				else
				{
					orderDetails += "No products found for this Customer Order.";
				}

				emailBody = await _lLMAgent.SummarizeContent(orderDetails, "You are a medical supplies sales associate and the customer order that was requested by email was found, please create response to the requestor. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");
			}

			EmailModel emailModel = new EmaiModelFactory().BuildReplyEmail(emailBody, emailId);

			await _emailService.CreateDraftReply(inboxId, emailModel);
		}
	}
}
