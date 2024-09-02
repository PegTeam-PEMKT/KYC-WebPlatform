using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Models
{
    public class BusinessViewModel
    {
        public int BusinessID { get; set; }
        public string BusinessName { get; set; }
        public string DirectorName { get; set; }
        public string DirectorNIN { get; set; }
        public bool IsNINValid { get; set; }
        public int SanctionScore { get; set; }
        public bool IsSanctionValid { get; set; }
    }
}