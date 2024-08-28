using KYC_WebPlatform.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class BusinessController : Controller
    {
        private ClientService _storage = new ClientService();

        // GET: Business
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewClients()
        {
            // Fetch records from the database and map to ViewModel
            List<object> cards = _storage.ExecuteSelectQuery("Select * from ClientBusiness");


            return View("ViewClients", cards);
        }

        public ActionResult ViewStatus()
        {
            return View("ViewStatus");
        }
    }
}