using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessDMS.DTO
{
    public class Subscription
    {
        public string ChangeType { get; set; }
        public string NotificationUrl { get; set; }
        public string Resource { get; set; }
        public string ApplicationId { get; set; }
        public string ExpirationDateTime { get; set; }
        public string Id { get; set; }
        public string ClientState { get; set; }
        public string CreatorId { get; set; }
    }
}
