using KYC_WebPlatform.Models;
using KYC_WebPlatform.Services.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace KYC_WebPlatform.Services.Business
{
    public class AuthenticationService
    {
        SecurityDAO securityDAO = new SecurityDAO();

        public bool Authenticate(LoginDto logindto)
        {
            Debug.WriteLine("From Authenticate: " + logindto.Email);
            return securityDAO.FindByUser(logindto.Email, logindto.Password);
        }

        public bool SignUpUser(SignupDto signupDto)
        {
            Debug.WriteLine("From SignUpUser: " + signupDto.Email);
            return securityDAO.CreateUser(signupDto);
        }
    }
}