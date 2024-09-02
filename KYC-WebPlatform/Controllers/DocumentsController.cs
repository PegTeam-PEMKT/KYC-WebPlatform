using KYC_WebPlatform.Services.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KYC_WebPlatform.Controllers
{
    public class DocumentsController : Controller
    {
        private ClientService _storage = new ClientService();

        // GET: Documents
        public ActionResult Index()
        {
            return View();
        }






       /* [HttpPost]
        public ActionResult Resend(int documentId)
        {
            using (var dbContext = new MyDbContext())
            {
                var document = dbContext.Documents.Find(documentId);
                if (document != null)
                {
                    document.IsSubmitted = false;
                    dbContext.SaveChanges();

                    // Delete the previous file
                    var uploadedFile = dbContext.UploadedFiles.FirstOrDefault(uf => uf.DocumentNumber == document.DocumentNumber);
                    if (uploadedFile != null)
                    {
                        dbContext.UploadedFiles.Remove(uploadedFile);
                        dbContext.SaveChanges();
                    }
                }
            }

            return Json(new { success = true });
        }*/
    } 
}