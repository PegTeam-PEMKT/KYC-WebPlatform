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
using System.Xml.Linq;


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

        public ActionResult AddBusiness(AddBusiness_MODEL model)
        {
            if (model.Directors == null)
            {
                model.Directors = new List<Director_MODEL>();
            }
            try
            {
                HttpContext.Session["TIN"] = model.BusinessTIN;
                string clientEmail = HttpContext.Session["Email"] as string;

                // Create an XML element to store the list of directors
                XElement directorsXml = new XElement("Directors");

                if (model.Directors != null)
                {
                    foreach (var director in model.Directors)
                    /*foreach (var director in model.Director_MODEL)*/
                    {
                        // Performing NIRA Validation for each director
                        string niraValidation = QueryCustomer(director.DirectorDOB, "000092564", director.DirectorGivenName, "NIRA", "NIRA-TEST_BILLPAYMENTS", "10F57BQ754", director.NIN, director.DirectorSurname);

                        // Performing Sanctions Validation for each director
                        SanctionResponse sanctionResponse = CheckSanctions(director.DirectorSurname + " " + director.DirectorGivenName);
                        var name = director.DirectorSurname + " " + director.DirectorGivenName;

                        // Create an XML element for each director
                        XElement directorXml = new XElement("Director");
                        directorXml.Add(new XElement("DirectorName", name));
                        directorXml.Add(new XElement("DirectorNIN", director.NIN));
                        directorXml.Add(new XElement("NiraValidation", niraValidation));
                        directorXml.Add(new XElement("SanctionScore", sanctionResponse.Score));
                        directorXml.Add(new XElement("SanctionStatusCode", sanctionResponse.StatusCode));
                        directorXml.Add(new XElement("SanctionStatusDescription", sanctionResponse.StatusDescription));

                        // Add the director XML element to the list of directors
                        directorsXml.Add(directorXml);
                    }

                }
                else
                {
                    Debug.WriteLine("\n\n\n****Directors are Empty\n\n\n\n**");
                }


                // Doing database operations
                DBContext dbContext = DBContext.GetInstance();
                using (SqlConnection connection = dbContext.GetConnection())
                {
                    // Open the connection
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("AddBusinessAndDirector2", connection))
                    /*using (SqlCommand command = new SqlCommand("AddBusinessAndDirector", connection))*/
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

                        // Input parameter for Directors (XML)
                        command.Parameters.AddWithValue("@Directors", directorsXml.ToString());

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

        //ViewStatus with tab
        public ActionResult ViewStatus(string tab)
        {
            string clientEmail = HttpContext.Session["Email"] as string;

            List<Document> documents = new List<Document>();
            /*List<Document> documents = dbContext.GetClientDocuments(clientEmail);*/

            // Get the database context instance
            DBContext dbContext = DBContext.GetInstance();

            // Open the connection
            using (SqlConnection connection = dbContext.GetConnection())
            {
                // Open the connection
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetClientDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add input parameter for the stored procedure
                    command.Parameters.AddWithValue("@ClientEmail", clientEmail);

                    // Execute the command and read the data
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create a new Document object and populate it with the data
                            Document document = new Document
                            {
                                FileId = reader["FileId"] != DBNull.Value ? reader["FileId"].ToString() : null,
                                FileName = reader["FileName"] != DBNull.Value ? reader["FileName"].ToString() : null,
                                BusinessId = reader["BusinessId"] != DBNull.Value ? (int?)reader["BusinessId"] : null,
                                UploadedOn = reader["UploadedOn"] != DBNull.Value ? (DateTime?)reader["UploadedOn"] : null,
                                IsVerified = reader["IsVerified"] != DBNull.Value ? (bool?)reader["IsVerified"] : null,
                                ApprovalCode = reader["Approval_Code"] != DBNull.Value ? reader["Approval_Code"].ToString() : null,
                                FilePath = reader["FilePath"] != DBNull.Value ? reader["FilePath"].ToString() : null,
                            };

                            // Add the document to the list
                            documents.Add(document);
                        }
                    }
                }

                // Close the connection
                connection.Close();
            }

            switch (tab) //sorting the documents according to the tab selected (All, Pending, Approved, Rejected)
            {
                case "orders-paid":
                    documents = documents.Where(d => d.IsVerified.HasValue && d.IsVerified.Value).ToList();
                    break;
                case "orders-pending":
                    documents = documents.Where(d => !d.IsVerified.HasValue || !d.IsVerified.Value).ToList();
                    break;
                case "orders-cancelled":
                    documents = documents.Where(d => d.ApprovalCode == "Cancelled").ToList(); //To change to correct Approval Code
                    break;
                default:
                    // Show all documents
                    break;
            }

            return View("ViewStatus", documents);
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



