using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Business;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class SignupController : Controller
    {
        // GET: Signup
        public ActionResult Index()
        {
            return View("ClientSignup");
        }

        public ActionResult Signup(SignupDto signupDto)
        {
         
            if (signupDto.PhoneNumber.Length < 10 || signupDto.PhoneNumber.Length > 10) 
            {
                ViewBag.ErrorMessage = "Invalid phone number length";
                return View("ClientSignup");
            }
            if (!signupDto.PhoneNumber.StartsWith("074") && !signupDto.PhoneNumber.StartsWith("075") && !signupDto.PhoneNumber.StartsWith("070") && !signupDto.PhoneNumber.StartsWith("078") && !signupDto.PhoneNumber.StartsWith("077"))
            {
                ViewBag.ErrorMessage = "Invalid phone number";
                return View("ClientSignup");
            }
            if (!(int.TryParse(signupDto.PhoneNumber, out _)))
            {
                ViewBag.ErrorMessage = "Invalid phone number digits";
                return View("ClientSignup");
            }

            AuthenticationService authenticationService = new AuthenticationService();

            if (authenticationService.SignUpUser(signupDto))
            {
                Debug.WriteLine("From Authenticate: " + signupDto.Email);
                ViewBag.SuccessMessage = "Account Created";
                return RedirectToAction("Index", "Login"); // Redirect to login page
            }
            else
            {
                ViewBag.ErrorMessage = "User already exists";
                return View("ClientSignup");
            }
        }
    }
}