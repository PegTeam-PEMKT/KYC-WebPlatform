using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Models
{
    public class Director_MODEL
    {
        // Director's phone number
        public string DirectorPhoneNumber { get; set; }

        // Director's National Identification Number (NIN)
        public string NIN { get; set; }

        // Director's email address
        public string NiraValidation { get; set; }

        // Director's email address
        public string SanctionsValidation { get; set; }

        // Director's email address
        public string DirectorEmail { get; set; }

        public string DirectorDOB { get; set; }

        public string DirectorSurnameName { get; set; }

        public string DirectorGivenName { get; set; }

        public string DirectorUtility { get; set; }

        public string DirectorVendorCode { get; set; }

        public string DirectorDocumentID { get; set; }
    }



}