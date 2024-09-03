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
    }
}