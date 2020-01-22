using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessDMS.DTO
{
    public class DriveItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
