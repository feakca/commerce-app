using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.EmailServices
{
    public class SmtpEmailSender : IEmailSender
    {
        private string _host;
        private int _port;
        private bool _enableSsl;
        private string _userName;
        private string _password;

        public SmtpEmailSender(string host, int port, bool enableSsl, string userName, string password)
        {
            _host = host;
            _port = port;
            _enableSsl = enableSsl;
            _userName = userName;
            _password = password;
        }

        public Task SendEmailAsync(string mail, string subject, string htmlBody)
        {
            var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = _enableSsl
            };
            return client.SendMailAsync(
                new MailMessage(_userName, mail, subject, htmlBody)
                {
                    IsBodyHtml = true
                });
        }
    }
}
