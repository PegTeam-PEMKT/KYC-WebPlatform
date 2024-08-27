using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Business;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View("SignIn");
        }

        public string Login(LoginDto loginDto)
        {
            AuthenticationService authenticationService = new AuthenticationService();
            if (authenticationService.Authenticate(loginDto))
            {
                return "Successfully logged in";
            }
            else
            {
                return "Log in failed";
            }
        }
    }
}