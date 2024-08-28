using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Business;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View("SignIn");
        }

        public ActionResult Login(LoginDto loginDto)
        {
            AuthenticationService authenticationService = new AuthenticationService();

            if (authenticationService.Authenticate(loginDto))
            {
                Debug.WriteLine("From Authenticate: " + loginDto.Email);
                if (SendOTP(loginDto.Email))
                {
                    return View("OtpView");
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to send OTP.";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to log in";
                return RedirectToAction("Index", "Login");
            }
        }

        public bool SendOTP(string email)
        {
            Random rand = new Random();
            string randomCode = (rand.Next(999999)).ToString();
            DateTime otpTimeStamp = DateTime.Now;

            HttpContext.Session["OTP"] = randomCode; // Store OTP in session
            HttpContext.Session["OTPTime"] = otpTimeStamp;
            HttpContext.Session["Email"] = email;

            string toEmail = email;
            string subject = "Your OTP Code";
            string body = "Your OTP code is " + randomCode + " it will expire in 5 minutes";

            EmailService emailService = new EmailService("jemimahsoulsister@outlook.com", "jemimah@soulsister", "smtp.office365.com", 587, true);
            bool emailSent = emailService.SendEmail(toEmail, subject, body);

            if (emailSent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult VerifyOtp(OtpViewModel model)
        {
            if (ModelState.IsValid)
            {
                string storedOtp = Session["OTP"] as string;
                DateTime? storedOtpTimeStamp = Session["OTPTime"] as DateTime?;
                string userEmail = Session["Email"] as string;

                // Check if OTP has expired
                if (DateTime.Now > storedOtpTimeStamp.Value.AddSeconds(15))
                {
                    ViewBag.ErrorMessage = "OTP has expired. Would you like to resend it?";
                    ViewBag.ExpiredOtp = true;
                    ViewBag.UserEmail = userEmail;
                    return View("OtpView", model); // Re-render the view with an option to resend the OTP
                }

                if (!string.IsNullOrEmpty(storedOtp) && storedOtp == model.Otp)
                {
                    Session.Remove("OTP"); // Remove OTP from session after successful verification
                    Session.Remove("OTPTime");
                    Session.Remove("UserEmail");
                    return RedirectToAction("Index", "Home"); // Redirect to dashboard
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid OTP, please try again.";
                    Debug.WriteLine("Invalid OTP");
                }
            }

            return View("OtpView", model);
        }

        //public ActionResult ResendOtp()
        //{
        //    string userEmail = Session["UserEmail"] as string;

        //    if (string.IsNullOrEmpty(userEmail))
        //    {
        //        ViewBag.ErrorMessage = "Email not found. Please request a new OTP.";
        //        return RedirectToAction("Login"); // Redirect to login or another appropriate action
        //    }

        //    // Resend the OTP
        //    SendOTP(userEmail); // Ensure SendOtp is correctly implemented to handle resending
        //}
    }
}