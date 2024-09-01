using System.ComponentModel.DataAnnotations;

namespace KYC_WebPlatform.Models
{
    public class OtpViewModel
    {
        [Required]
        public string Otp { get; set; }
        public string Email { get; set; }
    }
}