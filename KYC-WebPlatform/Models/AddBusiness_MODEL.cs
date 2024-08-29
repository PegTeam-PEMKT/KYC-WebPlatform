using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Models
{
    public class AddBusiness_MODEL
    {
        // General Information
        public string BusinessName { get; set; }
        public string ContactName { get; set; }
        public string BusinessPhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string BusinessWebsite { get; set; }

        // Director Information
        public string DirectorName { get; set; }
        public string NIN { get; set; }
        public string DirectorPhoneNumber { get; set; }
        public string DirectorEmail { get; set; }

        // Interested Pegasus Services
        public bool SchoolFeesPayment { get; set; }
        public bool USSDAggregation { get; set; }
        public bool MobileMoneyAggregation { get; set; }
        public bool CustomSoftware { get; set; }
        public bool OnlinePaymentGateway { get; set; }
        public bool BulkPayments { get; set; }
        public bool SMSServices { get; set; }

        // Financial Information
        public int NumberOfTransactions { get; set; }
        public int YearsOfOperation { get; set; }
        public decimal AmountEarnedPerMonth { get; set; }

        // Notifications
        public bool ReceiveEmailNotifications { get; set; }
    }

}