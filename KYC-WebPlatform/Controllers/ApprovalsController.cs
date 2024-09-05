using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Business;
using KYC_WebPlatform.Services.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers


{
    public class ApprovalsController : Controller
    {
        private ClientService _storage = new ClientService();
        public SecurityDAO security = new SecurityDAO();
        // GET: Approvals
        /*public ActionResult Index()
        {
            return View("PendingBusinesses");
        }*/

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Email"] == null) // or any session variable that confirms login
            {
                filterContext.Result = RedirectToAction("Index", "Login");
            }
            base.OnActionExecuting(filterContext);
        }

        //for the businesses

        public ActionResult GetBusinesses()
        {
            string Email = HttpContext.Session["Email"] as string;
            Debug.WriteLine("From GetFiles: " + Email);
            try
            {
                // Fetch records from the database and map to ViewModel
                Dictionary<string, List<object>> pendingBusinesses = _storage.ExecuteSelectQuery("sp_GetBusinessPendingBusinesses");
                Debug.WriteLine("AAAAAAA********" + pendingBusinesses.Values.Count);
                return View("PendingBusinesses", pendingBusinesses);
            }
            catch (SqlException sq)
            {
                Debug.WriteLine(sq.LineNumber + "`````00000```````" + sq.ToString());
                return View("Error");
            }
        }

        /*public ActionResult ApproveOrReject(ApprovalViewModel model, string action)
        {
            ApproveService approveService = new ApproveService();
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
                        if(approveService.ApproveBusinessByEmail(email, businessUserEmail))
                        {
                            ViewBag.SuccessMessage = "Client has been approved";
                            return View("ViewClientDetails");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Client has not been approved";
                            return View("ViewClientDetails");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("ApproveClientByEmail did not execute");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Client email not found";
                    return View("ViewClientDetails");
                }
            }
            else if (action == "reject")
            {
                string email = approveService.GetBusinessEmailByIdToApprove(id);
                if (email != null)
                {
                    if (approveService.RejectClientByEmail(email))
                    {
                        if(approveService.RejectBusinessByEmail(email, businessUserEmail))
                        {
                            ViewBag.SuccessMessage = "Client has been rejected";
                            return View("ViewClientDetails");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Client has not been rejected";
                            return View("ViewClientDetails");
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Client email not found";
                    return View("ViewClientDetails");
                }
            }

            ViewBag.ErrorMessage = "Action not recognized";
            return View("ViewClientDetails"); ;
        }*/

        //for the files
        public ActionResult GetFiles(int BusinessId)
        {
            try
            {
                HttpContext.Session["BusinessId"] = BusinessId;
                string Email = HttpContext.Session["Email"] as string;
                Debug.WriteLine("From GetFiles: " + Email);
                Debug.WriteLine("BBBBBBBB" + BusinessId);
                Debug.WriteLine("EEEEEEEE" + Email);
                List<object> l = new List<object>();
                SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@BusinessId", BusinessId),
                        new SqlParameter("@UserEmail", Email)
                    };

                Dictionary<string, List<object>> pendingBusinessesFiles = _storage.ExecuteSelectQuery("sp_NewGetPendingBusinessFiles", parameters);
                Debug.WriteLine("BBBBBBBB" + pendingBusinessesFiles.Count);
                return View("PendingBusinessFiles", pendingBusinessesFiles);


            }
            catch (SqlException s)
            {
                Debug.WriteLine(s.Message);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult DisplayApproval()
        {
            try
            {
                string BusinessId;
                string fileName = Request.Form["FileName"];
                string businessName = Request.Form["BusinessName"];
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@BusinessName", businessName) };
                Dictionary<string, List<object>> result = _storage.ExecuteSelectQuery("sp_GetBusinessId", parameters);

                List<object> firstList = result.Values.First();

                BusinessId = firstList.First().ToString();
                Debug.WriteLine("BUSINESS ID HERE HERE HERE: " + BusinessId);


                /*
                Debug.WriteLine(Request.Form["UploadedDate"]);
                DateTime uploadedDate = DateTime.Parse(Request.Form["UploadedDate"]);*/
                string filePath = Request.Form["FilePath"];
                string currentApprovalCode = Request.Form["ApprovalCode"];
                /* if (System.IO.File.Exists(filePath))
                 {
                     return File(filePath, "application/pdf");
                 }*/
                // Use these values to display the view with the corresponding data
                return View("FileViewer", new List<Object> { filePath, currentApprovalCode, BusinessId, fileName, BusinessId });

            }
            catch (Exception ee)
            {
                Debug.WriteLine("From DisplayApproval: " + ee.Message);
                return View("Error");
            }

        }

        public ActionResult UpdateApprovalCode(int status, string approvalCode, int businessId, string fileName)
        {
            Debug.WriteLine("received status...." + status + "  approvalCode....." + approvalCode + " businessId...." + businessId + "  fileName....." + fileName);

            SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@status", status),
                        new SqlParameter("@approvalCode", approvalCode),
                        new SqlParameter("@businessId", businessId),
                        new SqlParameter("@fileName", fileName)
                    };
            Debug.WriteLine("received code...." + approvalCode);

            Dictionary<string, List<object>> results = _storage.ExecuteSelectQuery("UpdateApprovalCode", parameters);
            // Assuming you know the key (e.g., "ApprovalCode")
            // Get the first key-value pair in the dictionary
            var firstKeyValuePair = results.FirstOrDefault();
            object firstElement = new object();
            string updatedApprovalCode = firstElement.ToString();
            // Check if the dictionary is not empty
            if (firstKeyValuePair.Key != null)
            {
                // Print the first key
                Console.WriteLine($"First Key: {firstKeyValuePair.Key}");

                // Retrieve the list associated with the first key
                List<object> values = firstKeyValuePair.Value;

                // Check if the list is not empty and print the first element
                if (values.Count > 0)
                {
                    firstElement = values[0];
                    Console.WriteLine($"First Value in the List: {firstElement}");
                }
                else
                {
                    Console.WriteLine("The list is empty.");
                }
            }
            else
            {
                Console.WriteLine("The dictionary is empty.");
            }


            Debug.WriteLine("UPDATED code...." + firstElement.ToString());
            return RedirectToAction("NotifyNextApprover", new { approvalCode, updatedApprovalCode });
        }

        public ActionResult UpdateApprovalCode2(int status, string approvalCode, int businessId, string fileName, string nextApprover = null, string rejectionReason = null)
        {
            Debug.WriteLine("received status...." + status + "  approvalCode....." + approvalCode + " businessId...." + businessId + "  fileName....." + fileName);

            SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@status", status),
                        new SqlParameter("@approvalCode", approvalCode),
                        new SqlParameter("@businessId", businessId),
                        new SqlParameter("@fileName", fileName),
                        new SqlParameter("@NextApprover", (object)nextApprover ?? DBNull.Value),
                        new SqlParameter("@RejectionReason", (object)rejectionReason ?? DBNull.Value)
                    };
            Debug.WriteLine("received code...." + approvalCode);

            Dictionary<string, List<object>> results = _storage.ExecuteSelectQuery("UpdateApprovalCode2", parameters);
            // Assuming you know the key (e.g., "ApprovalCode")
            // Get the first key-value pair in the dictionary
            var firstKeyValuePair = results.FirstOrDefault();
            object firstElement = new object();
            string updatedApprovalCode = firstElement.ToString();
            // Check if the dictionary is not empty
            if (firstKeyValuePair.Key != null)
            {
                // Print the first key
                Console.WriteLine($"First Key: {firstKeyValuePair.Key}");

                // Retrieve the list associated with the first key
                List<object> values = firstKeyValuePair.Value;

                // Check if the list is not empty and print the first element
                if (values.Count > 0)
                {
                    firstElement = values[0];
                    Console.WriteLine($"First Value in the List: {firstElement}");
                }
                else
                {
                    Console.WriteLine("The list is empty.");
                }
            }
            else
            {
                Console.WriteLine("The dictionary is empty.");
            }


            Debug.WriteLine("UPDATED code...." + firstElement.ToString());
            return RedirectToAction("NotifyNextApprover", new { approvalCode, updatedApprovalCode, nextApprover });
        }

        public ActionResult NotifyNextApprover(string approvalCode, string updatedApprovalCode, string nextApprover)
        {
            try
            {
                string SourceEmailAddress = HttpContext.Session["Email"] as string;
                Debug.WriteLine("Inside the notifier a.k.a the rumourMonger: " + SourceEmailAddress);


                Dictionary<string, string> approvalSequence = new Dictionary<string, string>
                {
                    { "BUSINESS#001", "HRLEGAL#001" },
                    { "HRLEGAL#001", "FINANCE#001" },
                    { "FINANCE#001", "MDAPPROVE#001" }
                };


                if (SendNotification(updatedApprovalCode, nextApprover))
                {
                    return RedirectToAction("GetFiles", "Approvals");
                }
                else
                {
                    return View("Error");
                }

            }
            catch (Exception e)
            {

                return View("Error", e.Message);

            }


        }

        public bool SendNotification(string approvalCode)
        {
            EmailService notifyEmail = new EmailService("jemimahsoulsister@outlook.com", "jemimah@soulsister", "smtp.office365.com", 587, true);
            bool SentOk = false;
            // Use a parameterized query to prevent SQL injection

            //string query = "SELECT Email FROM Departments WHERE DeptCode = @ApprovalCode";

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@ApprovalCode", approvalCode)
            };
            Dictionary<string, List<object>> receiver = _storage.ExecuteSelectQuery("GetToEmail", parameters);
            string toEmail = receiver.Values.ToString();
            string subject = "Pending KYC Approval";
            string body = "You have a pending File approval from the KYC platform";
            string altHost = "smtp-mail.outlook.com";
            bool SendOk = notifyEmail.SendEmail(toEmail, subject, body);

            if (SentOk)
            {
                return true;
            }

            else
            {
                Debug.WriteLine("Hmmm the email dint go");
                return false;
            }

        }

        public bool SendNotification(string approvalCode, string toEmail)
        {
            EmailService notifyEmail = new EmailService("jemimahsoulsister@outlook.com", "jemimah@soulsister", "smtp.office365.com", 587, true);
            bool SendOk = false;
            string subject = "Pending KYC Approval";
            string body = "You have a pending File approval from the KYC platform";
            string altHost = "smtp-mail.outlook.com";
            SendOk = notifyEmail.SendEmail(toEmail, subject, body);

            if (SendOk)
            {
                return true;
            }

            else
            {
                Debug.WriteLine("Hmmm the email dint go");
                return false;
            }
        }
    }
}