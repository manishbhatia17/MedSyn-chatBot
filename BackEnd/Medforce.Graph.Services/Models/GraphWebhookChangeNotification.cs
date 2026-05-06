using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Medforce.Graph.Services.Models
{
    public class GraphWebhookChangeNotification
    {
        [JsonPropertyName("value")]
        public List<GraphWebhookChangeNotificationItem> Value { get; set; }
    }

    public class GraphWebhookChangeNotificationItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonPropertyName("subscriptionExpirationDateTime")]
        public DateTime SubscriptionExpirationDateTime { get; set; }

        [JsonPropertyName("clientState")]
        public string ClientState { get; set; }

        [JsonPropertyName("changeType")]
        public string ChangeType { get; set; }

        [JsonPropertyName("resource")]
        public string Resource { get; set; }

        [JsonPropertyName("tenantId")]
        public string TenantId { get; set; }

        [JsonPropertyName("resourceData")]
        public ResourceData ResourceData { get; set; }
    }

    public class ResourceData
    {
        [JsonPropertyName("@odata.type")]
        public string ODataType { get; set; }

        [JsonPropertyName("@odata.id")]
        public string ODataId { get; set; }

        [JsonPropertyName("@odata.etag")]
        public string ODataEtag { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
