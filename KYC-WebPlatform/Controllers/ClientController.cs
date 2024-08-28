using NiraApiIntegrationService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public ActionResult AddBusiness()
        {
            return View("AddBusiness");
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