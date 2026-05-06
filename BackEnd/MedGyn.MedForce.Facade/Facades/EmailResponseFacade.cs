using Azure;
using Medforce.Graph.Services.Interfaces;
using Medforce.Graph.Services.Models;
using Medgyn.Meforce.LLMAgent.LLMAgents;
using Medgyn.Meforce.LLMAgent.Models;
using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.Factories.Interfaces;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.DeviceManagement.DeviceConfigurations.Item.GetOmaSettingPlainTextValueWithSecretReferenceValueId;
using Newtonsoft.Json;
using NHibernate.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace MedGyn.MedForce.Facade.Facades
{
	public class EmailResponseFacade : IEmailResponseFacade
	{
		private readonly ILLMService _lLMAgent;
		private readonly Medforce.Graph.Services.Interfaces.IEmailService _emailService;
		private readonly IEmailBotCommandHandlerFactory _emailBotCommandHandlerFactory;

		private readonly IMemoryCache _cache;

		public EmailResponseFacade(ILLMService agent, Medforce.Graph.Services.Interfaces.IEmailService emailService,  IMemoryCache cache, IEmailBotCommandHandlerFactory emailBotCommandHandlerFactory)
		{
			_lLMAgent = agent;
			_emailBotCommandHandlerFactory = emailBotCommandHandlerFactory;
			_emailService = emailService;
			_cache = cache;
		}

		public async Task<string> GetEmailResponseAsync(string content, string agentFunctions, string agentDirections = "")
		{
			string emailAddress = "";
			string functionJson = System.IO.File.ReadAllText(@".\wwwroot\js\ChatGPTMCPServerJson.json");
			//var complettionTest = await _lLMAgent.SummarizeContent(emails[2].Body, "You are a medical supplies sales associate assistant. Summarize the content of the email in a concise manner. With some recommendations");
			var response = await _lLMAgent.AgentFunction(content, functionJson);
			
			string body = await _lLMAgent.SummarizeContent($"The email is asking about {response.FunctionName} with the following details: {string.Join(", ", response.Parameters)}", "You are acting as a medical supplies sales rep assistant. It is your job to read through the email and determine what the sender is asking for and provide a concise summary of the request along with any recommendations you have based on the content of the email. Return only the summary and recommendations in your response.");

			return body;
		}

		public async Task ProcessNotificationAsync(GraphWebhookChangeNotification payload)
		{
			// 1. Unpack the payload (assume resourceData contains user/inbox info)
			string inboxId = payload?.Value?[0]?.Resource != null ? payload?.Value?[0]?.Resource?.Split('\'')[1] : string.Empty;
			if (string.IsNullOrEmpty(inboxId)) return;

			// 2. Get new emails
			var emails = await _emailService.GetNewEmails(inboxId, DateTime.Now.AddMinutes(-5)); // Adjust lastChecked as needed

			foreach (var email in emails)
			{
				if(_cache.TryGetValue(email.Id, out bool processed) && processed)
				{
					continue; // Skip already processed emails
				}


				_cache.Set(email.Id, true, TimeSpan.FromHours(1)); // Mark email as processed

																	// 3. Call LLM agent to determine response
				string content = $"email subject {email.Subject}. end subject. email body: {email.Body}";
				string functionJson = System.IO.File.ReadAllText(@".\wwwroot\js\ChatGPTMCPServerJson.json");

				var agentResponse = await _lLMAgent.AgentFunction(content, functionJson, "You are acting as a medical supplies sales rep. It is your job to read through emails and find data that they are requesting. This data can be about product names, finding products by our custom id, looking up PO information, Invoice information and other presales tasks");
				await ProcessAgentFunction(agentResponse, inboxId, email.Id, email.From);

			}
		}

		private async Task ProcessAgentFunction(LLMFunctionServiceContract response, string inboxId, string emailId, string fromEmail)
		{
			if (response != null)
			{

				IEmailBotCommandHandler emailBotCommandHandler = _emailBotCommandHandlerFactory.GetCommandHandler(response.FunctionName);

				if (emailBotCommandHandler != null)
				{
					await emailBotCommandHandler.HandleCommandAsync(response.Parameters.ToArray(), fromEmail, inboxId, emailId);
				}
			

				//switch (response.FunctionName)
				//{
				//	case "GetProductByName":
						
				//	case "GetProductById":
						
				//		break;
				//	//case "GetInvoice":
				//	//	var invoice_data = response.Parameters[0];
				//	//	Dictionary<string, string> invoiceDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(invoice_data);

				//	//	var invoiceId = invoiceDict["invoice_number"];

				//	//	_customerOrderService.
				//	//	break;
				//	//case "GetInvoicesByCustomer":
				//	//	var customerId = response.Candidates[0].Content.Parts[0].FunctionCall.Args["customer_id"];

				//	//	break;
				//	case "GetCustomerPO":
						
						
				//	case "GetCustomerOrder":
						
				//	case "GetCustomerOrderByEmail":
					
				//	default:
				//		//do nothing, return at the end will handle it
				//		break;
				//}

			}
		}
	}
}
