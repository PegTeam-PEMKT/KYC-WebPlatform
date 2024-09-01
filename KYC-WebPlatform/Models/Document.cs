using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Models
{
    public class Document
    {
        public string FileId { get; set; }       // Corresponds to the "FileId" column
        public string FileName { get; set; }     // Corresponds to the "FileName" column
        public int? BusinessId { get; set; }     // Corresponds to the "BusinessId" column
        public DateTime? UploadedOn { get; set; } // Corresponds to the "UploadedOn" column
        public bool? IsVerified { get; set; }    // Corresponds to the "IsVerified" column
        public string ApprovalCode { get; set; } // Corresponds to the "Approval_Code" column
        public string FilePath { get; set; }     // Corresponds to the "FilePath" column
    }

}