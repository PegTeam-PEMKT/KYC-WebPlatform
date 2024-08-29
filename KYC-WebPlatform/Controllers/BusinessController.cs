using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Business;
using KYC_WebPlatform.Services.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class BusinessController : Controller
    {
        private ClientService _storage = new ClientService();

        // GET: Business
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewClients()
        {
            try
            {
                // Fetch records from the database and map to ViewModel
                Dictionary<string, List<object>> cards = _storage.ExecuteSelectQuery("Select * from ClientBusiness");
                // Fetch records from the database and map to ViewModel
                
                return View("ViewClients", cards);
            }
            catch (SqlException e)
            {

                return View("Error", e.Message);
            }
        }

        public ActionResult ViewStatus()
        {
            return View("ViewStatus");
        }

        public ActionResult CreateView() 
        {
            return View("CreateView");
        }

        public ActionResult CreateUser(SignupDto signupDto)
        {
            AuthenticationService authenticationService = new AuthenticationService();

            if (authenticationService.SignUpUser(signupDto))
            {
                Debug.WriteLine("From Authenticate: " + signupDto.Email);
                ViewBag.SuccessMessage = "Account Created";
                return RedirectToAction("ViewClients"); // Redirect to login page
            }
            else
            {
                ViewBag.ErrorMessage = "User already exists";
                return View("ViewClients");
            }
        }
    }
}