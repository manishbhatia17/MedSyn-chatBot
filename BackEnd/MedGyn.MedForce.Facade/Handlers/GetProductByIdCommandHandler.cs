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
	public class GetProductByIdCommandHandler: IEmailBotCommandHandler
	{
		public EmailBotCommandType CommandType => EmailBotCommandType.GetProductById;

		private readonly ILLMService _lLMAgent;
		private readonly Medforce.Graph.Services.Interfaces.IEmailService _emailService;
		private readonly IProductService _productService;
		private readonly ISharePointListSearchService _sharePointListSearchService;


		public GetProductByIdCommandHandler(IProductService productService,
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
			var productCustomId = parameters[0];
			Dictionary<string, string> idDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(productCustomId);

			// Example usage:
			string productIdSearch = idDict["product_id"];
			ProductContract product = _productService.GetProductByCustomId(productIdSearch);

			if (product == null)
			{
				emailBody = await _lLMAgent.SummarizeContent("Product not found. Please check the product name and try again.", "You are a medical supplies sales associateand the product that was requested by email was not found, please create response to the requestor. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");
			}
			else
			{
				emailBody =  await _lLMAgent.SummarizeContent($"Product found: {product.ProductName} with ID {product.ProductCustomID}", "You are a medical supplies sales associate and the product that was requested by email was found, please create response to the requestor. Return your response as JSON formatted as {'Subject': '<subject here>', 'Body': '<body here>'} and Body should be in HTML format.");
			}

			EmailModel emailModel = new EmaiModelFactory().BuildReplyEmail(emailBody, emailId);

			await _emailService.CreateDraftReply(inboxId, emailModel);
		}
	}
}
