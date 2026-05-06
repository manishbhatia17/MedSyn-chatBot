using Medforce.Graph.Services.Interfaces;
using Medforce.Graph.Services.Models;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Web.Controllers
{
	public class EmailAutomationController : Controller
	{
		private readonly IEmailResponseFacade _emailResponseFacade;
		private readonly IGraphWebhookSubscriptions _graphWebhookSubscriptions;


		public EmailAutomationController(IEmailResponseFacade emailResponseFacade, IGraphWebhookSubscriptions webhookSubscriptions	)
		{
			_graphWebhookSubscriptions = webhookSubscriptions;
			_emailResponseFacade = emailResponseFacade;
		}

		[HttpPost, AllowAnonymous]
		[Route("/api/microsoftgraphwebhooks/notification")]
		public async Task<IActionResult> Notification(string validationToken = null, GraphWebhookChangeNotification payload = null)
		{
			// Check for validationToken in query string
			if (!string.IsNullOrEmpty(validationToken))
			{
				// Respond with 200 OK and echo the token as plain text
				return Content(validationToken, "text/plain");
			}
			// 1. Process the payload and handle new emails
			if (payload.Value == null)
			{
				using (var reader = new StreamReader(Request.Body))
				{
					var content = await reader.ReadToEndAsync();
					payload = JsonConvert.DeserializeObject<GraphWebhookChangeNotification>(content);
				}
			}

			if (payload.Value != null)
			{

				await _emailResponseFacade.ProcessNotificationAsync(payload);

				// 2. Unpack userId from payload for subscription renewal

				string userId = _graphWebhookSubscriptions.ExtractUserId(payload?.Value?[0]?.Resource);
				string notificationUrl = Request.Scheme + "://" + Request.Host + "/api/microsoftgraphwebhooks/notification";
				string lifecycleUrl = Request.Scheme + "://" + Request.Host + "/api/microsoftgraphwebhooks/renew-subscription";

				// 3. Check and refresh subscription if almost expired
				await _graphWebhookSubscriptions.EnsureActiveSubscription(userId, notificationUrl, lifecycleUrl);
			}
			return Ok();
		}
	}
}
