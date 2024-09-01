using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services;
using KYC_WebPlatform.Services.Data;
using NiraApiIntegrationService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace KYC_WebPlatform.Controllers
{
    public class ClientController : Controller
    {
        private readonly PegPayService _pegPayService;
        private readonly ApiService _apiService;
        private readonly ClientService _storage = new ClientService();

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

        public  ActionResult AddBusiness(AddBusiness_MODEL model)
        {
            try
            {
                HttpContext.Session["TIN"] = model.BusinessTIN;
                string clientEmail = HttpContext.Session["Email"] as string;
                // Performing NIRA Validation (assuming it's a synchronous call)
                model.NiraValidation = QueryCustomer(model.DirectorDOB, "000092564", model.DirectorGivenName, "NIRA", "NIRA-TEST_BILLPAYMENTS", "10F57BQ754", model.NIN, model.DirectorSurname);

                // Performing Sanctions Validation
                SanctionResponse sanctionresponse = CheckSanctions(model.DirectorSurname + " " + model.DirectorGivenName);
                var name = model.DirectorSurname + " " + model.DirectorGivenName;
                Debug.WriteLine("\n\n\n******\n\n");
                Debug.WriteLine("SancCode: " + sanctionresponse.StatusCode + " SancDesc: " + sanctionresponse.StatusDescription + " IsSanc: " + sanctionresponse.Sanctioned + " SancScore: " + sanctionresponse.Score);
                Debug.WriteLine("\n******\n\n");
                Debug.WriteLine("Nira: " + model.NiraValidation + " Sanctions: " + model.SancationsValidation);

                // Doing database operations
                DBContext dbContext = DBContext.GetInstance();
                using (SqlConnection connection = dbContext.GetConnection())
                {
                    // Open the connection
                    connection.Open();
                    Debug.WriteLine("NIN: " + model.NIN + " BusinessName: " + model.BusinessName);
                    Debug.WriteLine("Nira: " + model.NiraValidation + " Sanctions: " + model.SancationsValidation);

                    using (SqlCommand command = new SqlCommand("AddBusinessAndDirector", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters for Business
                        command.Parameters.AddWithValue("@BusinessName", model.BusinessName);
                        command.Parameters.AddWithValue("@ContactPerson", model.ContactName);
                        command.Parameters.AddWithValue("@IsActive", false);
                        command.Parameters.AddWithValue("@TransactionVolume", model.NumberOfTransactions);
                        command.Parameters.AddWithValue("@TransactionTraffic", model.AmountEarnedPerMonth);
                        command.Parameters.AddWithValue("@Email", clientEmail);
                        command.Parameters.AddWithValue("@PhoneNumber", model.BusinessPhoneNumber);
                        command.Parameters.AddWithValue("@TIN", model.BusinessTIN);

                        // Input parameters for Director
                        command.Parameters.AddWithValue("@DirectorName", name);
                        command.Parameters.AddWithValue("@DirectorNIN", model.NIN);
                        command.Parameters.AddWithValue("@NiraValidation", model.NiraValidation);
                        /*command.Parameters.AddWithValue("@SanctionsValidation", model.SancationsValidation);*/
                        command.Parameters.AddWithValue("@SanctionScore", sanctionresponse.Score);
                        command.Parameters.AddWithValue("@SanctionStatusCode", sanctionresponse.StatusCode);
                        command.Parameters.AddWithValue("@SanctionStatusDescription", sanctionresponse.StatusDescription);


                        // Execute the stored procedure
                        command.ExecuteNonQuery();
                    }
                    //Close the connection
                    connection.Close();
                }

                return View("ClientIndex");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                ModelState.AddModelError("", "An error occurred while processing your request.");
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
                var fileDic = "Content/Files";
                string filePath = Server.MapPath("~/") + fileDic;

                string TIN = HttpContext.Session["TIN"] as string;
                string Email = HttpContext.Session["Email"] as string;
                Debug.WriteLine($"=============TIN: {Email}=============");

                string query = "Select BusinessId from ClientBusiness where Email = @BusinessTIN";
                // Retrieve the BusinessId from the ClientBusiness table
                SqlParameter[] param = new SqlParameter[]
                    {
                        new SqlParameter("@Email", Email)
                    };
                Dictionary<string, List<object>> results = _storage.ExecuteSelectQuery("sp_GetBusinessIdByEmail", param);
                int businessId = (int)(results.Values.First().FirstOrDefault() ?? default(int));


                Debug.WriteLine($"++++++++FROM Upload: {businessId}++++++++++++");

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var fileName = file.FileName;
                filePath = Path.Combine(filePath, fileName);
                file.SaveAs(filePath);
                Debug.WriteLine(filePath + "***success!");
                var random = new Random();
                var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZab#c@defghijklmnopqrstuvwxyz0123456789";
                var randomText = new string(Enumerable.Range(0, 15)
                    .Select(_ => characters[random.Next(characters.Length)])
                    .ToArray());
                Debug.WriteLine("o0o0o0o0o0o0" + randomText);
                SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FileId", randomText),
                        new SqlParameter("@FileName", fileName),
                        new SqlParameter("@BusinessId", businessId),
                        new SqlParameter("@UploadedOn", DateTime.Now),
                        new SqlParameter("@IsVerified", false),
                        new SqlParameter("@Approval_Code","BUSINESS#001"),
                        new SqlParameter("@FilePath",filePath)
                    };
                Debug.WriteLine(parameters.Length);
                string InsertQuery = "INSERT INTO CompanyDocument (FileId, FileName, BusinessId, UploadedOn, IsVerified, Approval_Code, FilePath)  VALUES (@FileId, @FileName, @BusinessId, @UploadedOn, @IsVerified, @Approval_Code, @FilePath)";
                int rowsAffected = _storage.ExecuteInsertQuery(InsertQuery, parameters);
                Debug.WriteLine("INSERTEEEED!!! " + rowsAffected);
                return RedirectToAction("ViewStatus");
            }
            catch (System.NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                return View("ErrorView", e);
            }
        }

        //NIRA Validation method
        public string QueryCustomer(string dateOfBirth, string documentId, string givenName,
                                    string utility, string vendorCode, string password,
                                    string nationalId, string surname)
        {
            var result = _pegPayService.QueryCustomerDetails(dateOfBirth, documentId, givenName,
                                                             utility, vendorCode, password,
                                                             nationalId, surname);
            Debug.WriteLine(result);
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


        [HttpPost]
        public SanctionResponse CheckSanctions(string name)
        {
            try
            {
                var jsonResponse = _apiService.SendRequestAsync(name);
                Debug.WriteLine(jsonResponse);

                if (jsonResponse == null)
                {
                    return new SanctionResponse { StatusCode = "Error", StatusDescription = "Error occurred while processing the request." };
                }

                Debug.WriteLine($"StatusCode: {jsonResponse.StatusCode}");
                Debug.WriteLine($"StatusDescription: {jsonResponse.StatusDescription}");
                Debug.WriteLine($"Sanctioned: {jsonResponse.Sanctioned}");
                Debug.WriteLine($"Score: {jsonResponse.Score}");
                Debug.WriteLine($"Score: {jsonResponse.Name}");
                Debug.WriteLine($"Score: {jsonResponse.ActualName}");
                // Add more as needed

                return jsonResponse;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new SanctionResponse { StatusCode = "Error", StatusDescription = "Error occurred while processing the request." };
            }
        }
    }
}

