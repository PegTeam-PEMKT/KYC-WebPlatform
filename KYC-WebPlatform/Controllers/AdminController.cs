using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Business;
using KYC_WebPlatform.Services.Data;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View("AdminSignIn");
        }

        public ActionResult CreateUser(SignupDto signupDto)
        {
            if (signupDto.PhoneNumber.Length < 10 || signupDto.PhoneNumber.Length > 10)
            {
                ViewBag.ErrorMessage = "Invalid phone number length";
                return View("AddUser");
            }
            if (!signupDto.PhoneNumber.StartsWith("074") && !signupDto.PhoneNumber.StartsWith("075") && !signupDto.PhoneNumber.StartsWith("070") && !signupDto.PhoneNumber.StartsWith("078") && !signupDto.PhoneNumber.StartsWith("077"))
            {
                ViewBag.ErrorMessage = "Invalid phone number";
                return View("AddUser");
            }
            if (!(int.TryParse(signupDto.PhoneNumber, out _)))
            {
                ViewBag.ErrorMessage = "Invalid phone number digits";
                return View("AddUser");
            }
            AuthenticationService authenticationService = new AuthenticationService();

            if (authenticationService.SignUpUser(signupDto))
            {
                Debug.WriteLine("From Authenticate: " + signupDto.Email);
                ViewBag.SuccessMessage = "Account Created";
                return View("AddUser"); // Redirect to login page
            }
            else
            {
                ViewBag.ErrorMessage = "User already exists";
                return View("AddUser");
            }
        }

        public ActionResult SignInAdmin (LoginDto loginDto)
        {
            AuthenticationService authenticationService = new AuthenticationService();

            if (authenticationService.AuthenticateAdmin(loginDto))
            {
                HttpContext.Session["Email"] = loginDto.Email;
                string name = authenticationService.GetNameByEmail(loginDto.Email);

                HttpContext.Session["Username"] = name;
                TempData["Username"] = name;

                Debug.WriteLine("From Authenticate: " + loginDto.Email);
                if (SendOTP(loginDto.Email))
                {
                    ViewBag.Email = loginDto.Email;
                    return View("AdminOtpView");
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to send OTP. (Email not found)";
                    return View("AdminSignIn");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to log in (Invalid User Credentials)";
                return View("AdminSignIn");
            }
        }

        public bool SendOTP(string email)
        {
            Random rand = new Random();
            string randomCode = (rand.Next(999999)).ToString();
            DateTime otpTimeStamp = DateTime.Now;

            HttpContext.Session["OTP"] = randomCode; // Store OTP in session
            HttpContext.Session["OTPTime"] = otpTimeStamp;

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
                string storedOtp = HttpContext.Session["OTP"] as string;
                DateTime? storedOtpTimeStamp = HttpContext.Session["OTPTime"] as DateTime?;
                string userEmail = HttpContext.Session["Email"] as string;

                Debug.WriteLine(userEmail);
                Debug.WriteLine("OTP: " + model.Otp);
                Debug.WriteLine("Stored OTP: " + storedOtp);

                DateTime otpTimeStamp = storedOtpTimeStamp ?? DateTime.MinValue;

                // Check if OTP has expired
                if (DateTime.Now > otpTimeStamp.AddMinutes(5))
                {
                    ViewBag.ErrorMessage = "OTP has expired. Would you like to resend it?";
                    ViewBag.ExpiredOtp = true;
                    Session["OTPEmail"] = userEmail;
                    //ViewBag.Email = userEmail;
                    return View("AdminOtpView", model); // Re-render the view with an option to resend the OTP
                }

                if (!string.IsNullOrEmpty(storedOtp) && storedOtp == model.Otp)
                {
                    Session.Remove("OTP"); // Remove OTP from session after successful verification
                    Session.Remove("OTPTime");

                    return View("AddUser"); // Redirect to admin dashboard
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid OTP, please try again.";
                    Debug.WriteLine("Invalid OTP");
                }
            }

            return View("AdminOtpView", model);
        }


        /// <summary>
        /// This method has problem!!!
        /// </summary>
        /// <returns></returns>
        public ActionResult ResendOtp()
        {
            string userEmail = Session["OTPEmail"] as string;

            Debug.WriteLine("From ResendOtp: " + userEmail);

            if (string.IsNullOrEmpty(userEmail))
            {
                ViewBag.ErrorMessage = "Email not found. Please request a new OTP(By relogging in).";
                return View("AdminSignIn"); // Redirect to login or another appropriate action
            }
            else
            {
                // Resend the OTP
                if (SendOTP(userEmail))
                {
                    ViewBag.ErrorMessage = null;
                    ViewBag.ExpiredOtp = false;
                    return View("AdminOtpView");
                } // Ensure SendOtp is correctly implemented to handle resending
                else
                {
                    ViewBag.ErrorMessage = "Failed to send OTP.";
                    return View("AdminSignIn");
                }
            }

        }

        public ActionResult ForgotView()
        {
            return View("ForgotPassword");
        }


        /*public ActionResult SendForgotPasswordOTP(string email)
        {

            Session["ForgotEmail"] = email;

            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Email not found. Try again";
                return View("OtpView"); // Redirect to login or another appropriate action
            }

            Random rand = new Random();
            string randomCode = (rand.Next(999999)).ToString();
            DateTime otpTimeStamp = DateTime.Now;

            HttpContext.Session["OTP"] = randomCode; // Store OTP in session
            HttpContext.Session["OTPTime"] = otpTimeStamp;

            string toEmail = email;
            string subject = "Your OTP Code";
            string body = "Your OTP code is " + randomCode + " it will expire in 5 minutes";
            string altHost = "smtp-mail.outlook.com";

            EmailService emailService = new EmailService("jemimahsoulsister@outlook.com", "jemimah@soulsister", "smtp.office365.com", 587, true);
            bool emailSent = emailService.SendEmail(toEmail, subject, body);

            if (emailSent)
            {
                ViewBag.Email = email;
                return View("ForgotOtpView");
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to send OTP. Contact Support";
                return View("SignIn");
            }
        }*/

        /*public ActionResult VerifyForgotOtp(OtpViewModel model)
        {
            SecurityDAO dAO = new SecurityDAO();

            if (ModelState.IsValid)
            {
                string storedOtp = HttpContext.Session["OTP"] as string;
                DateTime? storedOtpTimeStamp = HttpContext.Session["OTPTime"] as DateTime?;
                string userEmail = HttpContext.Session["ForgotEmail"] as string;

                Debug.WriteLine(userEmail);
                Debug.WriteLine("OTP: " + model.Otp);
                Debug.WriteLine("Stored OTP: " + storedOtp);

                DateTime otpTimeStamp = storedOtpTimeStamp ?? DateTime.MinValue;

                // Check if OTP has expired
                if (DateTime.Now > otpTimeStamp.AddMinutes(5))
                {
                    ViewBag.ErrorMessage = "OTP has expired. Would you like to resend it?";
                    ViewBag.ExpiredOtp = true;
                    TempData["ForgotOTPEmail"] = userEmail;
                    //ViewBag.Email = userEmail;
                    return View("ForgotOtpView", model); // Re-render the view with an option to resend the OTP
                }

                if (!string.IsNullOrEmpty(storedOtp) && storedOtp == model.Otp)
                {
                    Session.Remove("OTP"); // Remove OTP from session after successful verification
                    Session.Remove("OTPTime");

                    return View("UpdatePassword");

                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid OTP, please try again.";
                    Debug.WriteLine("Invalid OTP");
                }
            }

            return View("ForgotOtpView", model);
        }*/

        /*public ActionResult UpdatePassword(LoginDto loginDto)
        {
            AuthenticationService authenticationService = new AuthenticationService();

            string forgotEmail = HttpContext.Session["ForgotEmail"] as string;
            loginDto.Email = forgotEmail;

            if (authenticationService.ChangePassword(loginDto))
            {
                ViewBag.SuccessMessage = "Password changed successfully.";
                return View("SignIn");
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to change password";
                return View("SignIn");
            }
        }*/
    }
}