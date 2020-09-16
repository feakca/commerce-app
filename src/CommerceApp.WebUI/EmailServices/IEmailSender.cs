using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.EmailServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string mail, string subject, string htmlBody);
    }
}
