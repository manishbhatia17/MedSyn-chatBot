using Azure;
using Medforce.Graph.Services.Interfaces;
using Medforce.Graph.Services.Models;
using Medgyn.Meforce.LLMAgent.LLMAgents;
using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.Factories;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
	public class GetProductByNameCommandHandler : IEmailBotCommandHandler
	{
		public EmailBotCommandType CommandType => EmailBotCommandType.GetProductByName;

		private readonly ILLMService _lLMAgent;
		private readonly Medforce.Graph.Services.Interfaces.IEmailService _emailService;
		private readonly IProductService _productService;
		private readonly ISharePointListSearchService _sharePointListSearchService;

		public GetProductByNameCommandHandler(IProductService productService, 
			ILLMService lLMAgent,
			Medforce.Graph.Services.Interfaces.IEmailService emailService,
			 ISharePointListSearchService sharePointListSearchService)
		{
			_productService = productService;
			_lLMAgent = lLMAgent;
			_emailService = emailService;
			_sharePointListSearchService = sharePointListSearchService;
		}


		public async Task HandleCommandAsync(string[] parameters, string fromEmail, string inboxId, string emailId)
		{
			string emailBody = "";
			var productName = parameters[0];
			Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(productName);

			// Example usage:
			string productSearchName = dict["product_name"];
			ProductContract product = _productService.SearchProductByName(productSearchName);
			
			if (product == null)
			{
				emailBody =  await _lLMAgent.SummarizeContent("Product not found. Please check the product name and try again.", "You are a medical supplies sales associateand the product that was requested by email was not found, please create response to the requestor. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");
			}
			else
			{
				// 4. Optionally call SharePoint search if agent requests documents (pseudo logic)
				// You may need to parse agentResponse for document search intent
				// Example: if (agentResponse.Contains("search_sharepoint")) { ... }
				// var docs = await _sharePointListSearchService.SearchSharePointList(siteId, listId, query);
				emailBody = await _lLMAgent.SummarizeContent($"Product found: {product.ProductName} with ID {product.ProductCustomID}", "You are a medical supplies sales associate and the product that was requested by email was found, please create response to the requestor. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");
			}

			EmailModel emailModel = new EmaiModelFactory().BuildReplyEmail(emailBody, emailId);

			await _emailService.CreateDraftReply(inboxId, emailModel);
		}
	}
}
