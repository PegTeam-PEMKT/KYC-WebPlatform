using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Services.Business
{
    public class AuthenticationService
    {
        SecurityDAO securityDAO = new SecurityDAO();

        public bool Authenticate(LoginDto logindto)
        {
            return securityDAO.FindByUser(logindto);
        }
    }
}