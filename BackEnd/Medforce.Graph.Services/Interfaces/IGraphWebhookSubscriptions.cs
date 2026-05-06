using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medforce.Graph.Services.Interfaces
{
	public interface IGraphWebhookSubscriptions
	{
		Task<Subscription> SubscribeToInboxWebhook(string inboxId, string notificationUrl, string lifecylceUrl);
		Task<Subscription> EnsureActiveSubscription(string userId, string notificationUrl, string lifecycleUrl);
		Task DeleteSubscription(string subscriptionId);
		string ExtractUserId(string resource);
	}
}
