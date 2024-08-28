using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KYC_WebPlatform
{
    // capture the URL and route it to the appropriate page
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Route for UploadKYC
            routes.MapRoute(
                name: "UploadKYC",
                url: "Client/UploadKYC",
                defaults: new { controller = "Client", action = "UploadKYC" }
            );

            routes.MapRoute(
                name: "Upload",
                url: "Client/Upload",
                defaults: new { controller = "Client", action = "Upload" }
            );

            // Custom Route for Client Notifications
            routes.MapRoute(
                name: "ClientNotifications",
                url: "Client/Notifications",
                defaults: new { controller = "Client", action = "ClientNotifications" }
            );

            // Custom Route for Client Status View
            routes.MapRoute(
                name: "ViewStatus",
                url: "Client/Status",
                defaults: new { controller = "Client", action = "ViewStatus" }
            );

            // Custom Route for Adding a Business
            routes.MapRoute(
                name: "AddBusiness",
                url: "Client/AddBusiness",
                defaults: new { controller = "Client", action = "AddBusiness" }
            );

            // Default route for Client Index
            routes.MapRoute(
                name: "Client",
                url: "Client",
                defaults: new { controller = "Client", action = "ClientIndex" }
            );

            // Default route for Client Help
            routes.MapRoute(
                name: "ClientHelp",
                url: "Client/Help",
                defaults: new { controller = "Client", action = "Help" }
            );

            routes.MapRoute(
                name: "Login",
                url: "Login", //Login/Index
                defaults: new { controller = "Login", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}", //home/Index or About
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
