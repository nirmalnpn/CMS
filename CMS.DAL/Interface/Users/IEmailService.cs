using CMS.Modules.Modules;
using CMS.Modules.Modules.RequestModel.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Interface.Users
{
    public interface IEmailService
    {
        Task<EmailResult> SendEmailAsync(string to, string subject, string body);     
    }
}
