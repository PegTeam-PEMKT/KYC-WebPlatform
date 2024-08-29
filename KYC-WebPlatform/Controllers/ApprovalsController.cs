using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KYC_WebPlatform.Services.Data;
using KYC_WebPlatform.Services.Data.Interfaces;

namespace KYC_WebPlatform.Controllers
{
    public class ApprovalsController : Controller
    {
        private ClientService _storage = new ClientService();
        // GET: Approvals
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetBusinesses()
        {
            // Fetch records from the database and map to ViewModel
            Dictionary<string, List<object>> pendingBusinesses = _storage.ExecuteSelectQuery("SELECT B.BusinessName, B.Location ,D.FileID,D.FileName,D.BusinessId,D.UploadedOn,D.IsVerified , D.StatusCode, D.FilePath FROM ClientBusiness as B INNER JOIN CompanyDocument as D ON B.BusinessId = D.BusinessId WHERE B.StatusCode = ");
            return View("PendingBusinesses", pendingBusinesses);
        }

    }
}