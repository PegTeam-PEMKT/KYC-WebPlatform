using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform
{
    public class SanctionResponse
    {
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string EntityId { get; set; }
        public bool Sanctioned { get; set; }
        public double Score { get; set; }
        public List<string> Name { get; set; }
        public List<string> ActualName { get; set; }
        public List<string> Sources { get; set; }
        public List<string> Nationality { get; set; }
        public List<string> BirthDate { get; set; }
        public List<string> Address { get; set; }
        public List<string> BirthPlace { get; set; }
        public List<string> Country { get; set; }
        public List<string> Position { get; set; }
        public List<string> Gender { get; set; }
        public List<string> Topics { get; set; }
    }

}