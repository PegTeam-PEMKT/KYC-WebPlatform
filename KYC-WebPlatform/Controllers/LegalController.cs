using KYC_WebPlatform.Services.Business;
using KYC_WebPlatform.Services.Data;
using KYC_WebPlatform.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class LegalController : Controller
    {
        private ClientService _storage = new ClientService();
        public SecurityDAO security = new SecurityDAO();

        // GET: Legal
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LegalValidations()
        {
            return View("LegalValidations");
        }

        public ActionResult Legal()
        {
            string Email = HttpContext.Session["Email"] as string;
            Debug.WriteLine("From Legal: " + Email);
            try
            {
                // Fetch records from the database and map to ViewModel
                Dictionary<string, List<object>> pendingBusinesses = _storage.ExecuteSelectQuery("sp_GetLegalPendingBusinesses");
                Debug.WriteLine("AAAAAAA********" + pendingBusinesses.Values.Count);
                return View("Legal", pendingBusinesses);
            }
            catch (SqlException sq)
            {
                Debug.WriteLine(sq.LineNumber + "`````00000```````" + sq.ToString());
                return View("Error");
            }
        }

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

                Dictionary<string, List<object>> pendingBusinessesFiles = _storage.ExecuteSelectQuery("sp_GetPendingLegalFiles", parameters);
                Debug.WriteLine("BBBBBBBB" + pendingBusinessesFiles.Count);
                return View("PendingLegalFiles", pendingBusinessesFiles);


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

                //getting all finance people
                string query = "SELECT Username, Email FROM Departments WHERE DeptCode = 'FINANCE#001'";
                Dictionary<string, List<object>> receivers = _storage.ExecuteSelectQuery(query);
                Debug.WriteLine("Emails here counts " + receivers.Count);
                var receiverList = new List<Tuple<string, string>>(); // Tuple to hold Username and Email
                foreach (var receiver in receivers.Values)
                {
                    receiverList.Add(new Tuple<string, string>(receiver[0].ToString(), receiver[1].ToString()));
                    Debug.WriteLine("*****\n\n\n****" + receiver[0].ToString() + "\n" + receiver[1].ToString() + "\n\n*******");
                }

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
                return View("LegalFileViewer", new List<Object> { filePath, currentApprovalCode, BusinessId, fileName, BusinessId, receiverList });

            }
            catch (Exception ee)
            {
                Debug.WriteLine("From DisplayApproval: " + ee.Message);
                return View("Error");
            }

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

        public string NotifyNextApprover(string approvalCode, string updatedApprovalCode, string nextApprover)
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
                    return "Notification sent to "+nextApprover;
                }
                else
                {
                    return "Notification not sent";
                }

            }
            catch (Exception e)
            {

                return "From NotifyNextApprover: "+e.Message;

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