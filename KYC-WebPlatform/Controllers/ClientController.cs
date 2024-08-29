using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KYC_WebPlatform.Services;
using NiraApiIntegrationService;
using KYC_WebPlatform.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Data.SqlClient;
using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Data;

namespace KYC_WebPlatform.Controllers
{
    public class ClientController : Controller
    {
        private readonly PegPayService _pegPayService;
        private readonly ApiService _apiService;

        public ClientController()
        {
            _pegPayService = new PegPayService(); // For the NIRA validation
            _apiService = new ApiService(); // For Open Sanctions validation
        }

        // GET: Client
        public ActionResult ClientIndex()
        {
            return View("ClientIndex");
        }

        public ActionResult AddBusiness(AddBusiness_MODEL model)
        {
            BusinessService businessService = new BusinessService();
            if (businessService.SaveBusinessInfo(model))
            {

                return View("AddBusiness");
            }

            return View("AddBusiness", model);
        }

        public ActionResult ViewStatus()
        {
            return View("ViewStatus");
        }

        public ActionResult ClientNotifications()
        {
            return View("ClientNotifications");
        }

        public ActionResult Help()
        {
            return View("Help");
        }

        public ActionResult UploadKYC()
        {
            return View("UploadKYC");
        }

        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            try
            {
                var fileDic = "Files";
                string filePath = Server.MapPath("~/") + fileDic;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var fileName = file.FileName;
                filePath = Path.Combine(filePath, fileName);
                file.SaveAs(filePath);
                Debug.WriteLine(filePath + "***success!");
                return RedirectToAction("ViewStatus");
            }
            catch (System.NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                return View("ErrorView", e);
            }
        }

        //NIRA Validation method
        public String QueryCustomer(string dateOfBirth, string documentId, string givenName,
                                    string utility, string vendorCode, string password,
                                    string nationalId, string surname)
        {
            var result = _pegPayService.QueryCustomerDetails(dateOfBirth, documentId, givenName,
                                                             utility, vendorCode, password,
                                                             nationalId, surname);

            Console.WriteLine(result);
            if (result != null)
            {
                if (result.Contains("True"))
                {
                    return "Validated";
                }
                else
                {
                    return "Not Valid";
                }
            }

            return "Nira Returned Null";
        }

        // Submit Director Info
        [HttpPost]
        public async Task<ActionResult> SubmitDirectorInfo(AddBusiness_MODEL model)
        {
            if (ModelState.IsValid)
            {
                // Validate the NIN using the QueryCustomer method
                /*model.NiraValidation = QueryCustomer(model.DirectorDOB, "000092564", model.DirectorGivenName, "NIRA", "NIRA-TEST_BILLPAYMENTS", "10F57BQ754", model.NIN, model.DirectorSurnameName);*/
                model.NiraValidation = QueryCustomer("01/01/1993", "000092564", "Johnson", "NIRA", "NIRA-TEST_BILLPAYMENTS", "10F57BQ754", "CM930121003EGE", "Tipiyai");


                var jsonResponse = await _apiService.SendRequestAsync("Vladimir Putin");

                if (jsonResponse == null)
                {
                    return Content("Error occurred while processing the request.");
                }

                string jsonresp = jsonResponse.ToString();
                // Assuming jsonResponse is a string containing the JSON array
                /*var sanctionResponses = JsonConvert.DeserializeObject<List<SanctionResponse>>(jsonresp);*/


                var sanctionResponses =  jsonresp;

              

                if (sanctionResponses != null && jsonresp.Contains("EXISTS"))
                {
                    model.SancationsValidation = "NotValidated";

                }
                else
                {
                    model.SancationsValidation = "Validated";
                }

                // Add to the database

                // Redirect to a confirmation page
                return RedirectToAction("Confirmation", model);


            }

            // If the model state is not valid, return the form view with validation messages
            return View(model);
        }

        // Confirmation page after successful submission
        public ActionResult Confirmation(AddBusiness_MODEL model)
        {
            return View(model);
        }

        // SANCTIONS: Method to handle sanctions checking (optional)
        public async Task<ActionResult> Sanctions()
        {
            var jsonResponse = await _apiService.SendRequestAsync("Putin");

            if (jsonResponse == null)
            {
                return Content("Error occurred while processing the request.");
            }

            string jsonresp = jsonResponse.ToString();
            // Assuming jsonResponse is a string containing the JSON array
            var sanctionResponses = JsonConvert.DeserializeObject<List<SanctionResponse>>(jsonresp);

            if (sanctionResponses != null && jsonresp.Contains("EXISTS"))
            {
                return View(sanctionResponses);
            }

            return Content("No matching sanctions found.");
        }
    }
}
