using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    // controller determines which view is displayed to the user
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
