using GraphRepository;
using Medforce.Graph.Services.Interfaces;
using MedGyn.MedForce.Common.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Medforce.Graph.Services
{
	public class GraphWebhookSubscriptions: BaseGraphClass, IGraphWebhookSubscriptions
	{
		private const int SubscriptionDurationMinutes = 1440; // Max duration for Graph subscriptions
		private const string SubscriptionCacheKeyPrefix = "GraphEmailSubscription_";
		private readonly IMemoryCache _cache;


		public GraphWebhookSubscriptions(IMemoryCache cache, IOptions<AppSettings> appSettings)
			: base(cache, appSettings)
		{
			_cache = cache;
		}
		/// <summary>
		/// Subscribes to a user's inbox for new mail notifications.
		/// </summary>
		public async Task<Subscription> SubscribeToInboxWebhook(string inboxId, string notificationUrl, string lifecyleUrl)
		{
			var subscription = new Subscription
			{
				ChangeType = "created,updated",
				NotificationUrl = notificationUrl,
				LifecycleNotificationUrl = lifecyleUrl,
				Resource = $"users/{inboxId}/mailFolders('inbox')/messages?$select=id,subject,receivedDateTime",
				ExpirationDateTime = DateTime.UtcNow.AddMinutes(SubscriptionDurationMinutes), // Max 4230 mins, but keep short for demo
				ClientState = Guid.NewGuid().ToString(),
				IncludeResourceData = true,
				EncryptionCertificate = Convert.ToBase64String(CreateCertificate().Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert)),
				EncryptionCertificateId = "GraphEmailCert"
			};
			var created = await _graphClient.Subscriptions.PostAsync(subscription);
			// Cache subscription info for renewal
			_cache.Set(SubscriptionCacheKeyPrefix + inboxId, created, created.ExpirationDateTime.Value);
			return created;
		}

		/// <summary>
		/// Ensures the subscription is active, renews if expiring soon.
		/// </summary>
		public async Task<Subscription> EnsureActiveSubscription(string userId, string notificationUrl, string lifecycleUrl)
		{
			if (_cache.TryGetValue<Subscription>(SubscriptionCacheKeyPrefix + userId, out var sub))
			{
				// Renew if less than 10 minutes left
				if (sub.ExpirationDateTime.HasValue && sub.ExpirationDateTime.Value <= DateTime.UtcNow.AddMinutes(10))
				{
					// Renew subscription
					sub.ExpirationDateTime = DateTime.UtcNow.AddMinutes(SubscriptionDurationMinutes);
					var updated = await _graphClient.Subscriptions[sub.Id].PatchAsync(sub);
					_cache.Set(SubscriptionCacheKeyPrefix + userId, updated, updated.ExpirationDateTime.Value);
					return updated;
				}
				return sub;
			}
			// No subscription, create new
			return await SubscribeToInboxWebhook(userId, notificationUrl, lifecycleUrl);
		}

		public async Task DeleteSubscription(string subscriptionId)
		{
			await _graphClient.Subscriptions[subscriptionId].DeleteAsync();
		}

		public string ExtractUserId(string resource)
		{
			var match = Regex.Match(resource, @"Users\('([^']+)'\)");
			return match.Success ? match.Groups[1].Value : null;
		}
	}
}
