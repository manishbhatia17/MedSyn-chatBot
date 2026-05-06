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
	public class GetCustomerPOCommandHandler : IEmailBotCommandHandler
	{
		public EmailBotCommandType CommandType => EmailBotCommandType.GetCustomerPO;

		private readonly ILLMService _lLMAgent;
		private readonly ICustomerOrderService _customerOrderService;
		private readonly Medforce.Graph.Services.Interfaces.IEmailService _emailService;

		public GetCustomerPOCommandHandler(ILLMService agent, ICustomerOrderService customerOrderService, Medforce.Graph.Services.Interfaces.IEmailService emailService)
		{
			_lLMAgent = agent;
			_customerOrderService = customerOrderService;
			_emailService = emailService;
		}

		public async Task HandleCommandAsync(string[] parameters, string fromEmail, string inboxId, string emailId)
		{
			string emailBody = "";
			var poId = parameters[0];
			Dictionary<string, string> poDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(poId);

			var customerOrder = _customerOrderService.GetCustomerOrderByPO(poDict["po_number"]);

			if (customerOrder == null || customerOrder.PONumber != poDict["po_number"])
			{
				emailBody = await _lLMAgent.SummarizeContent("Purchase Order not found.", "You are a medical supplies sales associateand the product that was requested by email was not found, please create response to the requestor. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"Purchase Order found: {customerOrder.PONumber} with ID {customerOrder.CustomerOrderID}");
				var products = _customerOrderService.GetCustomerOrderProducts(customerOrder.CustomerOrderID);
				if (products != null)
				{
					foreach (var productItem in products)
					{
						stringBuilder.AppendLine($"Product: {productItem.ProductName}, Quantity: {productItem.Quantity}");
					}
				}
				else
				{
					stringBuilder.AppendLine("No products found for this Purchase Order.");
				}
				emailBody = await _lLMAgent.SummarizeContent(stringBuilder.ToString(), "You are a medical supplies sales associate and the product that was requested by email was found, please create response to the requestor. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");
			}

			EmailModel emailModel = new EmaiModelFactory().BuildReplyEmail(emailBody, emailId);

			await _emailService.CreateDraftReply(inboxId, emailModel);
		}
	}
}
