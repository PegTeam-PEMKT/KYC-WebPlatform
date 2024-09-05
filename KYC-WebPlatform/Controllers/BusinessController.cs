using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services;
using KYC_WebPlatform.Services.Business;
using KYC_WebPlatform.Services.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class BusinessController : Controller
    {
        private ClientService _storage = new ClientService();
        private readonly ApiService _apiService;
        public BusinessController()
        {
            _apiService = new ApiService(); // For Open Sanctions validation

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Email"] == null) // or any session variable that confirms login
            {
                filterContext.Result = RedirectToAction("Index", "Login");
            }
            base.OnActionExecuting(filterContext);
        }

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
    
        */



        public ActionResult ViewClientDetails(int BusinessId)
        {
            try
            {
                Debug.WriteLine("POPOPOPOP:  " + BusinessId);
                // Fetch records from the database and map to ViewModel
                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@BusinessId",BusinessId)

                };
                Dictionary<string, List<object>> cards = _storage.ExecuteSelectQuery("GetBusinessAndDirectorsInfo", parameters);
                Debug.WriteLine("SOOO FAAAAARRRR WE HAVE GOT " + cards.Count + " RECORDS");
                // Fetch records from the database and map to ViewModel

                Debug.WriteLine("\n\n\n*******" + cards.Count.ToString() + "n\n\n\n*****");
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

        public ActionResult ApproveOrReject(ApprovalViewModel model, string action)
        {
            ApproveService approveService = new ApproveService();
            EmailService emailService = new EmailService("jemimahsoulsister@outlook.com", "jemimah@soulsister", "smtp.office365.com", 587, true);
            string name = "";
            int id = 0;

            string businessUserEmail = HttpContext.Session["Email"].ToString();

            Debug.WriteLine("From ApproveOrReject: " + businessUserEmail);

            foreach (var detail in model.BusinessDetails)
            {
                // Process each business detail here
                var businessId = detail.BusinessID;
                var businessName = detail.BusinessName;
                var directorName = detail.DirectorName;
                var directorNIN = detail.DirectorNIN;
                var isNINValid = detail.IsNINValid;
                var sanctionScore = detail.SanctionScore;
                var isSanctionValid = detail.IsSanctionValid;

                name = businessName;
                id = businessId;
            }

            if (action == "approve")
            {
                string email = approveService.GetBusinessEmailByIdToApprove(id);
                if (email != null)
                {
                    if (approveService.ApproveClientByEmail(email))
                    {
                        if (approveService.ApproveBusinessByEmail(email, businessUserEmail))
                        {

                            string toEmail = email;
                            string subject = "Pegasus Client Review";
                            string body = "We've reviewed your business details. You can now upload your KYC documents. The following are the requirements required to be onboarded onto the platform.\r\n\r\n- Contract with Pegasus Technologies\r\n- Valid Trading License.\r\n- Tax registration Certificate\r\n- Memorandum & Articles of Association\r\n- Company form 20 (certified)\r\n- Certificate of incorporation(certified)\r\n- Directors IDs\r\n- Company profile\r\n- Board resolution to apply for and use Pegasus services.\r\n- Proof of Address\r\n- Letter from official banker.\r\n- Bank statement for 6 months\r\n- 6 months' revenue projections\r\n- Beneficiaries form (form 1)(certified)\r\nThank you . Regards Please contact " + businessUserEmail;


                            emailService.SendEmail(toEmail, subject, body);

                            TempData["SuccessMessage"] = "Client has been approved";
                            return RedirectToAction("ViewClients", "Business");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Client has not been approved";
                            return RedirectToAction("ViewClients", "Business");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("ApproveClientByEmail did not execute");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Client email not found";
                    return RedirectToAction("ViewClients", "Business");
                }
            }
            else if (action == "reject")
            {
                string email = approveService.GetBusinessEmailByIdToApprove(id);
                if (email != null)
                {
                    if (approveService.RejectClientByEmail(email))
                    {
                        if (approveService.RejectBusinessByEmail(email, businessUserEmail))
                        {
                            string toEmail = email;
                            string subject = "Pegasus Client Review";
                            string body = "We've reviewed your business info and it's not within our requirements. Please contact " + businessUserEmail +" for more details";

                            emailService.SendEmail(toEmail, subject, body);

                            TempData["SuccessMessage"] = "Client has been rejected";
                            return RedirectToAction("ViewClients", "Business");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Client has not been rejected";
                            return RedirectToAction("ViewClients", "Business");
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Client email not found";
                    return RedirectToAction("ViewClients", "Business");
                }
            }

            TempData["ErrorMessage"] = "Action not recognized";
            return RedirectToAction("ViewClients", "Business");
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
                ViewBag.SuccessMessage = "Client credentials added successfully";
                return View("CreateView"); // Redirect to login page
            }
            else
            {
                ViewBag.ErrorMessage = "Client already exists";
                return View("CreateView");
            }
        }

        /*private List<AddBusiness_MODEL> ConvertDataTableToYourModel(DataTable dataTable)
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
        }*/

        private List<AddBusiness_MODEL> ConvertDataToBusinessModel(Dictionary<string, List<object>> data)
        {
            List<AddBusiness_MODEL> result = new List<AddBusiness_MODEL>();

            foreach (var businessEntry in data)
            {
                AddBusiness_MODEL model = new AddBusiness_MODEL
                {
                    BusinessName = businessEntry.Key,
                    Directors = new List<Director_MODEL>()
                };

                foreach (var directorObj in businessEntry.Value)
                {
                    // Assuming directorObj is of type DataRow or a similar structure
                    var directorData = (DataRow)directorObj;

                    Director_MODEL director = new Director_MODEL
                    {
                        DirectorSurname = directorData["DirectorSurname"].ToString(),
                        DirectorGivenName = directorData["DirectorGivenName"].ToString(),
                        DirectorDOB = directorData["DirectorDOB"].ToString(),
                        NIN = directorData["NIN"].ToString(),
                        DirectorPhoneNumber = directorData["DirectorPhoneNumber"].ToString(),
                        NiraValidation = directorData["NiraValidation"].ToString(),
                        SancationsValidation = directorData["SancationsValidation"].ToString(),
                        Sanctioned = Convert.ToBoolean(directorData["Sanctioned"]),
                        SanctionScore = directorData["SanctionScore"].ToString(),
                        SanctionDescription = directorData["SanctionDescription"].ToString(),
                        DirectorEmail = directorData["DirectorEmail"].ToString(),
                        DirectorUtility = directorData["DirectorUtility"].ToString(),
                        DirectorVendorCode = directorData["DirectorVendorCode"].ToString(),
                        DirectorDocumentID = directorData["DirectorDocumentID"].ToString()
                    };

                    model.Directors.Add(director);
                }

                result.Add(model);
            }

            return result;
        }

        [HttpPost]
        public ActionResult NINValidation(string dateOfBirth, string givenName, string nationalId, string surname, string cardnumber)
        {
            var response = _apiService.NiraValidation(dateOfBirth, cardnumber , givenName, "NIRA", "NIRA-TEST_BILLPAYMENTS", "10F57BQ754", nationalId, surname);
            ViewBag.NINResult = response;
            Debug.WriteLine(response.ToString());
            return View("Validations");
        }

        [HttpPost]
        public ActionResult SanctionsCheck(string PassedName, string PassedSchema)
        {
            var response = _apiService.SendRequestAsync(PassedName, PassedSchema);
            ViewBag.SanctionResult = response;
            return View("Validations");
        }

        public ActionResult Validations()
        {
            return View("Validations");
        }


        public ActionResult Help()
        {
            return View("BusinessHelp");
        }






    }

}