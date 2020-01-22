using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessDMS.DTO
{
    public class SubscriptionRequest
    {
        [JsonProperty("changeType")]
        public string ChangeType { get; set; }
        [JsonProperty("notificationUrl")]
        public string NotificationUrl { get; set; }
        [JsonProperty("resource")]
        public string Resource { get; set; }
        [JsonProperty("expirationDateTime")]
        public string ExpirationDateTime { get; set; }
        [JsonProperty("clientState")]
        public string ClientState { get; set; }
    }
}
