using System.Collections.Generic;

namespace KYC_WebPlatform.Models
{
    public class BusinessValidation_MODEL
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public string RequestType { get; set; }
        public string Threshold { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string EntityId { get; set; }
        public bool Sanctioned { get; set; }
        public double Score { get; set; }
        public List<string> Names { get; set; }
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
        public string dateOfBirth { get; set; }
        public string documentId { get; set; }
        public string givenName { get; set; }
        public string utility { get; set; }
        public string vendorCode { get; set; }
        public string password { get; set; }
        public string nationalId { get; set; }
        public string surname { get; set; }
        //  statuscode
        public string ResponseField6 { get; set; }
        // Status Description
        public string ResponseField7 { get; set; }
        // Isvalid
        public bool ResponseField27 { get; set; }
        public string ErrorMessage { get; set; }



    }
}