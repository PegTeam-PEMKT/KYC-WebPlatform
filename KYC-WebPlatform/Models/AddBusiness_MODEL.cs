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
        public string Businesslocation { get; set; }
        public string BusinessId { get; set; }
        public string BusinessTIN { get; set; }



        // Director Information
        public string DirectorSurname { get; set; }
        public string DirectorGivenName { get; set; }
        public string DirectorDOB { get; set; }
        public string NIN { get; set; }
        public string DirectorPhoneNumber { get; set; }
        public string NiraValidation { get; set; }
        public string SancationsValidation { get; set; }
        public bool Sanctioned { get; set; }
        public string SanctionScore { get; set; }
        public string SanctionDescription { get; set; }
        public string DirectorEmail { get; set; }
        public string DirectorUtility { get; set; }
        public string DirectorVendorCode { get; set; }
        public string DirectorDocumentID { get; set; }




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