using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Medforce.Graph.Services;
using System.Threading.Tasks;
using System.IO;
using Medgyn.Meforce.LLMAgent.Models;
using System.Collections.Generic;
using Medgyn.Meforce.LLMAgent.LLMAgents;
using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.Interfaces;
using Medforce.Graph.Services.Models;
using Newtonsoft.Json;
using Medforce.Graph.Services.Interfaces;

namespace MedGyn.MedForce.Web.Controllers
{
	[AllowAnonymous]
	public class GraphTestController : Controller
	{
		private readonly Medforce.Graph.Services.Interfaces.IEmailService _emailService;
		private readonly IEmailResponseFacade _emailResponseFacade;
		private readonly ISharePointListSearchService _sharePointListSearchService;

		public GraphTestController(Medforce.Graph.Services.Interfaces.IEmailService emailService, IEmailResponseFacade emailResponseFacade, ISharePointListSearchService sharePointListSearchService)
		{
			_emailService = emailService;
			_emailResponseFacade = emailResponseFacade;
			_sharePointListSearchService = sharePointListSearchService;
		}
		public async Task<IActionResult> Index()
		{
			try
			{
				var products = await _sharePointListSearchService.SearchSharePointList("5de904fe-55ca-461d-ba58-ca9959b39832", "8b6e7b91-0b69-4288-9f9a-7dedda1243f0", "050029");
				var emails = await _emailService.GetEmails(MedGyn.MedForce.Common.Constants.MailboxConstants.T1);

				foreach (var email in emails)
				{
					string body = System.IO.File.ReadAllText("c:\\temp\\(EXTERNAL) PO#LI001-0007972637 CONFIRMATION NEEDED PLEASE639014769713643298.txt");
					string subject = "PO#LI001-0007972637 CONFIRMATION NEEDED PLEASE639014769713643298";
					string functionJson = System.IO.File.ReadAllText(@".\wwwroot\js\ChatGPTMCPServerJson.json");

					var response = await _emailResponseFacade.GetEmailResponseAsync($"email subject {subject}. end subject. email body: {body}", functionJson, "You are acting as a medical supplies sales rep. It is your job to read through emails and find data that they are requesting. This data can be about product names, finding products by our custom id, looking up PO information, Invoice information and other presales tasks");

					if (!string.IsNullOrEmpty(response))
					{
						string cleanedJSON = response.Trim('`', '\n').Replace("json\n", "");
						EmailModel replyEmail = JsonConvert.DeserializeObject<EmailModel>(cleanedJSON);
						replyEmail.Id = emails[3].Id;
						await _emailService.CreateDraftReply(MedGyn.MedForce.Common.Constants.MailboxConstants.T1, replyEmail);
					}
				}

				return Ok();
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}	
		}
	}
}
