using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Models
{
    public class DocumentViewModel
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
    }
    public class NewCompanyDocument
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int? SubmissionId { get; set; }
        public DateTime UploadedOn { get; set; }
        public bool IsVerified { get; set; }
        public string ApprovalCode { get; set; } 
        public bool IsBusinessApproved { get; set; }
        public bool IsLegalApproved { get; set; }
        public bool IsFinanceApproved { get; set; }
        public bool IsMDApproved { get; set; }
    }
}