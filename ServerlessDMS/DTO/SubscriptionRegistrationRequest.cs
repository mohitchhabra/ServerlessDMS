using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessDMS.DTO
{
    public class SubscriptionRegistrationRequest
    {
        public string Type { get; set; }
        public string Resource { get; set; }
        public DateTime Expiration { get; set; }
    }
}
