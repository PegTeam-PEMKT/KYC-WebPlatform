using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Data;
using KYC_WebPlatform.Services.Data.Interfaces;

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
            try {
                // Fetch records from the database and map to ViewModel
                Dictionary<string, List<object>> pendingBusinesses = _storage.ExecuteSelectQuery("sp_GetInactiveBusinesses");
                Debug.WriteLine("AAAAAAA********" + pendingBusinesses.Values.Count);
                return View("PendingBusinesses", pendingBusinesses);
            }
            catch (SqlException sq)
            {
                Debug.WriteLine(sq.LineNumber +"`````00000```````" + sq.ToString());
                return View("Error");
            }
        }

        //for the files
        public ActionResult GetFiles(int BusinessId, string BusinessName, int fileCount)
        {
            try {
                HttpContext.Session["BusinessId"] = BusinessId;
                string Email = HttpContext.Session["Email"] as string;
                Debug.WriteLine("From GetFiles: " +Email);
                string UserEmail = "akulluedith2022@gmail.com";
                Debug.WriteLine("BBBBBBBB" + BusinessId);
                Debug.WriteLine("EEEEEEEE" + UserEmail);
                SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@BusinessId", BusinessId),
                        new SqlParameter("@UserEmail", Email)
                    };

                Dictionary<string, List<object>> pendingBusinessesFiles = _storage.ExecuteSelectQuery("sp_GetPendingBusinessFiles", parameters);
                Debug.WriteLine("BBBBBBBB" + pendingBusinessesFiles.Count);
                return View("PendingBusinessFiles", pendingBusinessesFiles);


            } catch (SqlException s) {
                Debug.WriteLine(s.Message);
                return View("Error");
            }  
        }

        public ActionResult DisplayApprovalllllll() {

            string filepath= "C:\\Users\\bugsbunny\\Downloads\\water.jpg";

            if (filepath != null)
            {
                //return File(filepath, "application/octet-stream", Path.GetFileName(filepath));
                return View("FileViewer",filepath);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult DisplayApproval()
        {
            try {
                string BusinessId = HttpContext.Session["BusinessId"] as string;
                string fileName = Request.Form["FileName"];
                string businessName = Request.Form["BusinessName"];/*
                Debug.WriteLine(Request.Form["UploadedDate"]);
                DateTime uploadedDate = DateTime.Parse(Request.Form["UploadedDate"]);*/
                string filePath = Request.Form["FilePath"];
                string toBeApprovedBy = Request.Form["ToBeApprovedBy"];
                /* if (System.IO.File.Exists(filePath))
                 {
                     return File(filePath, "application/pdf");
                 }*/
                // Use these values to display the view with the corresponding data
                return View("FileViewer", new List<Object> { filePath, toBeApprovedBy, businessName, fileName, BusinessId });

            } catch (Exception ee) { 
            return View("Error");
            }
           
        }

        public ActionResult UpdateApprovalCode(int status,string approvalCode)
        {
            Debug.WriteLine("received status...." + status + "  approvalCode....." + approvalCode);

            SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@status", status),
                        new SqlParameter("@approvalCode", approvalCode)
                    };
            Debug.WriteLine("received code...."+approvalCode);
            Dictionary<string, List<object>> currentApprovalCode = _storage.ExecuteSelectQuery("UpdateApprovalCode", parameters);
            Debug.WriteLine("UPDATED code...." + currentApprovalCode.Values.ToString());
            return View("PendingBusinessFiles", approvalCode);
        }
    }
}