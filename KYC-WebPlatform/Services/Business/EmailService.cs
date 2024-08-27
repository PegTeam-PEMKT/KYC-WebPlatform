using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace KYC_WebPlatform.Services.Business
{
    public class EmailService
    {
        private string _fromEmail;
        private string _password;
        private string _smtpHost;
        private int _smtpPort;
        private bool _enableSsl;

        public EmailService(string fromEmail, string password, string smtpHost, int smtpPort, bool enableSsl)
        {
            _fromEmail = fromEmail;
            _password = password;
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _enableSsl = enableSsl;
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(_fromEmail);
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;

                SmtpClient smtpClient = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_fromEmail, _password),
                    EnableSsl = _enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}