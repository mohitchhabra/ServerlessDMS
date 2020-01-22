using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessDMS.DTO
{
    public class Attachment
    {
        public string Name { get; set; }
        public string ContentBytes { get; set; }
        public string ContentType { get; set; }
    }
}
