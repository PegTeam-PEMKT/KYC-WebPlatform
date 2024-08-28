using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Models
{
    public class OtpViewModel
    {
        [Required]
        public string Otp { get; set; }
        public string Email { get; set; }
    }
}