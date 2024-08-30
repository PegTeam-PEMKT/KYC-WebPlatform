using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Business;
using KYC_WebPlatform.Services.Data;
using System;
using System.Collections.Generic;
using System.Data;
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
        /*public ActionResult ViewClientDetails(int id)
        {
            // Define the stored procedure name
            string storedProcedure = "GetBusinessAndDirectorsInfo";

            // Define the parameters for the stored procedure
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@BusinessId", id)
            };

            // Execute the stored procedure and get the result as a DataTable
            DataTable businessAndDirectorsInfo = _storage.ExecuteSelectQuery2(storedProcedure, parameters);

            // If you want to pass the DataTable directly to the view:
            return View("ViewClientDetails", businessAndDirectorsInfo);

          *//*  // Or, you can convert the DataTable to a list or other model as needed:
            List<AddBusiness_MODEL> model = ConvertDataTableToYourModel(businessAndDirectorsInfo);*/

            /*Debug.WriteLine(model);
            return View("ViewClientDetails", model);*//*
        }*/



        public ActionResult ViewClientDetails()
        {
            try
            {

                string storedProcedure = "SELECT cb.BusinessId, cb.BusinessName, cb.Location, d.DirectorId, d.DirectorNIN, d.NinValidated, d.SanctionScore, d.SanctionStatusDescription FROM ClientBusiness cb LEFT JOIN Directors d ON cb.BusinessId = d.BusinessId WHERE cb.BusinessId = @BusinessId;";
                // Fetch records from the database and map to ViewModel
                Dictionary<string, List<object>> cards = _storage.ExecuteSelectQuery(storedProcedure);
                // Fetch records from the database and map to ViewModel
                
                Debug.WriteLine("\n\n\n*******"+cards.Count.ToString()+"n\n\n\n*****");
                return View("ViewClientDetails", cards);
            }
            catch (SqlException e)
            {

                return View("Error", e.Message);
            }
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

        private List<AddBusiness_MODEL> ConvertDataTableToYourModel(DataTable dataTable)
        {
            List<AddBusiness_MODEL> result = new List<AddBusiness_MODEL>();
            foreach (DataRow row in dataTable.Rows)
            {

                AddBusiness_MODEL model = new AddBusiness_MODEL
                {
                    
                    BusinessName = row["BusinessName"].ToString(),
                    ContactName = row["ContactPerson"].ToString(),
                    BusinessPhoneNumber = row["PhoneNumber"].ToString(),
                    BusinessEmail = row["Email"].ToString(),
                    Businesslocation = row["Location"].ToString(),
                    BusinessId = row["BusinessId"].ToString(),

                    DirectorSurname = row["DirectorName"].ToString(),
                    DirectorGivenName = row["DirectorName"].ToString(),    
                    NIN = row["DirectorNIN"].ToString(),
                    NiraValidation = row["NinValidated"].ToString(),
                    SanctionScore = row["SanctionScore"].ToString(),
                    SanctionDescription = row["SanctionStatusDescription"].ToString()
                    

                };
                result.Add(model);
            }
            return result;
        }

    }
}