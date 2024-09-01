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

        //for the businesses
        public ActionResult GetBusinesses()
        {
            string Email = HttpContext.Session["Email"] as string;
            Debug.WriteLine("From GetFiles: " + Email);
            try
            {
                // Fetch records from the database and map to ViewModel
                Dictionary<string, List<object>> pendingBusinesses = _storage.ExecuteSelectQuery("sp_GetInactiveBusinesses");
                Debug.WriteLine("AAAAAAA********" + pendingBusinesses.Values.Count);
                return View("PendingBusinesses", pendingBusinesses);
            }
            catch (SqlException sq)
            {
                Debug.WriteLine(sq.LineNumber + "`````00000```````" + sq.ToString());
                return View("Error");
            }
        }

        //for the files
        public ActionResult GetFiles(int BusinessId, string BusinessName, int fileCount)
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

                Dictionary<string, List<object>> pendingBusinessesFiles = _storage.ExecuteSelectQuery("sp_GetPendingBusinessFiles", parameters);
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
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter ("@BusinessName", businessName) };
                Dictionary<string, List<object>> result = _storage.ExecuteSelectQuery("sp_GetBusinessId", parameters);
                
                    List<object> firstList = result.Values.First();
                    
                        BusinessId = firstList.First().ToString();
                        Debug.WriteLine("BUSINESS ID HERE HERE HERE: " + BusinessId);
                
                
                /*
                Debug.WriteLine(Request.Form["UploadedDate"]);
                DateTime uploadedDate = DateTime.Parse(Request.Form["UploadedDate"]);*/
                string filePath = Request.Form["FilePath"];
                string toBeApprovedBy = Request.Form["ToBeApprovedBy"];
                /* if (System.IO.File.Exists(filePath))
                 {
                     return File(filePath, "application/pdf");
                 }*/
                // Use these values to display the view with the corresponding data
                return View("FileViewer", new List<Object> { filePath, toBeApprovedBy, BusinessId, fileName, BusinessId });

            }
            catch (Exception ee)
            {
                return View("Error");
            }

        }

        public ActionResult UpdateApprovalCode(int status, string approvalCode,int businessId, string fileName)
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
            object firstElement= new object();
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

        public ActionResult NotifyNextApprover(string approvalCode, string updatedApprovalCode)
        {
            try {
                string SourceEmailAddress = HttpContext.Session["Email"] as string;
                Debug.WriteLine("Inside the notifier a.k.a the rumourMonger: " + SourceEmailAddress);
                Dictionary<string, string> approvalSequence = new Dictionary<string, string>
                {
                    { "BUSINESS#001", "HRLEGAL#001" },
                    { "HRLEGAL#001", "FINANCE#001" },
                    { "FINANCE#001", "MDAPPROVE#001" }
                };

                if (approvalSequence.TryGetValue(approvalCode, out var currentApprovalCode) && currentApprovalCode == updatedApprovalCode)
                {
                }

                    /*//below we are going to get the next expected approval code
                    if (approvalSequence.TryGetValue(approvalCode, out var currentApprovalCode) && currentApprovalCode == updatedApprovalCode)
                    {
                        // Prepare parameters for the stored procedure
                        var parameters = new SqlParameter[]
                        {
                        new SqlParameter("@DepartmentHeadId", updatedApprovalCode)
                        };

                        // Execute the stored procedure and retrieve results
                        var results = _storage.ExecuteSelectQuery("GetDeptHeadEmail", parameters);

                        // Retrieve the first value from the results if available
                        if (results.Count > 0)
                        {
                            var firstKey = results.Keys.First();
                            var values = results[firstKey];

                            if (values.Count > 0 && values[0] is string DestinationEmailAddress)
                            {
                                Debug.WriteLine("Email Address: " + DestinationEmailAddress);
                                return View("NotifyView", DestinationEmailAddress);
                            }
                            else
                            {
                                return View("Error", "No valid email address found.");
                            }
                        }
                        else
                        {
                            return View("Error", "The dictionary is empty.");
                        }
                    }
                    else
                    {
                        // Handle cases where the approval code is invalid or doesn't match
                        return View("Error", "The approval code is not valid or does not match.");
                    }*/


                    return View("NotifyView", SourceEmailAddress);
            } catch (Exception e) {

                return View("Error", e.Message);

            }
            

            

        }
    }
}