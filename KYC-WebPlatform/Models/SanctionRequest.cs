using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform
{
    public class SanctionRequest
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public string RequestType { get; set; }
        public string Threshold { get; set; }
    }
}