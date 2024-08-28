using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Data;
using NiraApiIntegrationService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        public ActionResult ClientIndex()
        {
            return View("ClientIndex");
        }

        public ActionResult AddBusiness(AddBusiness_MODEL model)
        {
            if (/*ModelState.IsValid*/true)
            {
                try {
                    DBContext dbContext = DBContext.GetInstance();
                    using (SqlConnection connection = dbContext.GetConnection())
                    {
                        // Open the connection
                        connection.Open();
                        Debug.WriteLine("NIN: " + model.NIN + " BusinessName: " + model.BusinessName);
                        string sqlCommand = "INSERT INTO Directors (DirectorId, DirectorNIN, BusinessId) VALUES (@DirectorId, @NIN, @BusinessName)";
                        using (SqlCommand command = new SqlCommand(sqlCommand, connection))
                        {
                            command.Parameters.AddWithValue("@DirectorId", (string)model.DirectorPhoneNumber);
                            command.Parameters.AddWithValue("@NIN", model.NIN);
                            command.Parameters.AddWithValue("@BusinessName", model.BusinessName);
                            command.ExecuteNonQuery();
                        }
                        //Close the connection
                        connection.Close();
                    }

                } catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                return View("AddBusiness");
            }
               
            return View("AddBusiness", model);
        }

       /* public ActionResult SubmitBusiness(AddBusiness_MODEL model)
        {
            if (ModelState.IsValid)
            {*/
                // Save the model to the database
                //var business = new Business
                //{
                //    BusinessName = model.BusinessName,
                //    ContactName = model.ContactName,
                //    BusinessPhoneNumber = model.BusinessPhoneNumber,
                //    BusinessEmail = model.BusinessEmail,
                //    BusinessWebsite = model.BusinessWebsite,
                //    DirectorName = model.DirectorName,
                //    NIN = model.NIN,
                //    DirectorPhoneNumber = model.DirectorPhoneNumber,
                //    DirectorEmail = model.DirectorEmail,
                //    SchoolFeesPayment = model.SchoolFeesPayment,
                //    USSDAggregation = model.USSDAggregation,
                //    MobileMoneyAggregation = model.MobileMoneyAggregation,
                //    CustomSoftware = model.CustomSoftware,
                //    OnlinePaymentGateway = model.OnlinePaymentGateway,
                //    BulkPayments = model.BulkPayments,
                //    SMSServices = model.SMSServices,
                //    NumberOfTransactions = model.NumberOfTransactions,
                //    YearsOfOperation = model.YearsOfOperation,
                //    AmountEarnedPerMonth = model.AmountEarnedPerMonth,
                //    ReceiveEmailNotifications = model.ReceiveEmailNotifications
                //};

                //

                // Save business to database (assuming you have a DbContext set up)
                /*DBContext dbContext = DBContext.GetInstance();
                using (SqlConnection connection = dbContext.GetConnection())
                {
                    // Open the connection
                    connection.Open();

                    // Define the SQL command to insert data
                    string sqlCommand = $"INSERT INTO Directors (DirectorId, DirectorNIN, BusinessId) VALUES (@Column1, @Column2, @Column3)";

                    // Create a SqlCommand object
                    using (SqlCommand command = new SqlCommand(sqlCommand, connection))
                    {
                        // Define the parameters and their values
                        command.Parameters.AddWithValue("@Column1", model.DirectorPhoneNumber);
                        command.Parameters.AddWithValue("@Column2", model.NIN);
                        command.Parameters.AddWithValue("@Column3", model.BusinessName);

                        // Execute the command
                        command.ExecuteNonQuery();
                    }
                }

                // Redirect to a success page or another view
                return RedirectToAction("Index", "Home");
            }

            // If model state is not valid, return the view with the current model to display validation errors
            return View("AddBusiness", model);
        }*/


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
                //throw e;
                Console.WriteLine(e.Message);
                return View("ErrorView", e);
            }
        }

        private readonly PegPayService _pegPayService;

        public ClientController()
        {
            _pegPayService = new PegPayService(); // Ideally use dependency injection
        }

        [HttpGet]
        public ActionResult QueryCustomer()
        {
            return View();
        }

        [HttpPost]
        public String QueryCustomer(string dateOfBirth, string documentId, string givenName,
                                          string utility, string vendorCode, string password,
                                          string nationalId, string surname)
        {
            var result = _pegPayService.QueryCustomerDetails(dateOfBirth, documentId, givenName,
                                                             utility, vendorCode, password,
                                                             nationalId, surname);

            /* ViewBag.Result = result;
             return View();*/

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

    }
}