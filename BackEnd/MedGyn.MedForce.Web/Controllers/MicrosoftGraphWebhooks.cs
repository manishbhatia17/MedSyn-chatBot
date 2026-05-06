using FluentNHibernate.Conventions.Helpers;
using Medforce.Graph.Services.Interfaces;
using Medforce.Graph.Services.Models;
using MedGyn.MedForce.Facade.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Web.Controllers
{
	public class SubscriptionRequest
	{
		public string InboxId { get; set; }
		public string NotificationUrl { get; set; }
		public string LifecycleUrl { get; set; }
	}

	public class MicrosoftGraphWebhooks : Controller
	{
		private readonly IGraphWebhookSubscriptions _webhookSubscriptions;
		public MicrosoftGraphWebhooks(IGraphWebhookSubscriptions webhookSubscriptions)
		{
			_webhookSubscriptions = webhookSubscriptions;
		}

		public IActionResult Index()
		{
			return View();
		}

	

		[HttpPost, AllowAnonymous]
		[Route("/api/microsoftgraphwebhooks/create-subscription")]
		public async Task<IActionResult> CreateSubscription([FromBody] SubscriptionRequest request)
		{
			try
			{
				var subscription = await _webhookSubscriptions.SubscribeToInboxWebhook(request.InboxId, request.NotificationUrl, request.LifecycleUrl);
				return Json(new { success = true, subscriptionId = subscription.Id, expiration = subscription.ExpirationDateTime });
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		

		[HttpPost, AllowAnonymous]
		[Route("/api/microsoftgraphwebhooks/renew-subscription")]
		public async Task<IActionResult> RenewSubscription(string validationToken = null, ChangeNotificationCollection payload =null)
		{
			try
			{
				if (!string.IsNullOrEmpty(validationToken))
				{
					// Respond with 200 OK and echo the token as plain text
					return Content(validationToken, "text/plain");
				}

				List<ChangeNotification> changeLifeCycles = payload.Value.Where(n => n.LifecycleEvent != null).ToList();

				foreach (ChangeNotification changeNotification in changeLifeCycles)
				{

					string inboxId = changeNotification.Resource.Split('/')[1];
					string notificationUrl = Request.Scheme + "://" + Request.Host + "/api/microsoftgraphwebhooks/notification";
					string lifecycleUrl = Request.Scheme + "://" + Request.Host + "/api/microsoftgraphwebhooks/renew-subscription";

					var subscription = await	_webhookSubscriptions.EnsureActiveSubscription(inboxId, notificationUrl, lifecycleUrl);
				}

				return Accepted();
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete, AllowAnonymous]
		[Route("/api/microsoftgraphwebhooks/delete-subscription/{subscriptionId}")]
		public async Task<IActionResult> DeleteSubscription(string subscriptionId)
		{
			try
			{
				await _webhookSubscriptions.DeleteSubscription(subscriptionId);
				return Json(new { success = true });
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
