using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Business;
using KYC_WebPlatform.Services.Data;
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
                    ViewBag.Email = loginDto.Email;
                    return View("OtpView");
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to send OTP.";
                    return View("SignIn");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to log in";
                return View("SignIn");
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
            string altHost = "smtp-mail.outlook.com";
            
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
            SecurityDAO dAO = new SecurityDAO();

            if (ModelState.IsValid)
            {
                string storedOtp = Session["OTP"] as string;
                DateTime? storedOtpTimeStamp = Session["OTPTime"] as DateTime?;
                string userEmail = Session["Email"] as string;

                Debug.WriteLine(userEmail);

                DateTime otpTimeStamp = storedOtpTimeStamp ?? DateTime.MinValue;

                // Check if OTP has expired
                if (DateTime.Now > otpTimeStamp.AddMinutes(5))
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
                    

                    if (dAO.RetrieveRole(model.Email) == 15)
                    {
                        ViewBag.SuccessMessage = "Logged In Successfully";
                        return RedirectToAction("ClientIndex", "Client"); // Redirect to client dashboard
                    }
                    if (dAO.RetrieveRole(model.Email) == 11)
                    {
                        ViewBag.SuccessMessage = "Logged In Successfully";
                        return RedirectToAction("ViewClients", "Business"); // Redirect to admin dashboard
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid OTP, please try again.";
                    Debug.WriteLine("Invalid OTP");
                }
            }

            return View("OtpView", model);
        }


        /// <summary>
        /// This method has problem!!!
        /// </summary>
        /// <returns></returns>
        public ActionResult ResendOtp()
        {
            string userEmail = Session["UserEmail"] as string;

            if (string.IsNullOrEmpty(userEmail))
            {
                ViewBag.ErrorMessage = "Email not found. Please request a new OTP.";
                return View("SignIn"); // Redirect to login or another appropriate action
            }
            else
            {
                // Resend the OTP
                if (SendOTP(userEmail))
                {
                    return View("OtpView");
                } // Ensure SendOtp is correctly implemented to handle resending
                else
                {
                    ViewBag.ErrorMessage = "Failed to send OTP.";
                    return View("SignIn");
                }
            }
         
        }
    }
}