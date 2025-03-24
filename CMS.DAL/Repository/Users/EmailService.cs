using CMS.DAL.Interface;
using CMS.DAL.Interface.Users;
using CMS.Modules.Modules;
using CMS.Modules.Modules.RequestModel.User;
using CMS.Modules.Modules.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Repository.Users
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(SmtpClient smtpClient, IOptions<SmtpSettings> smtpSettings)
        {
            _smtpClient = smtpClient;
            _smtpSettings = smtpSettings.Value;
        }
        public async Task<EmailResult> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Connect to SMTP server only when sending the email
                _smtpClient.Connect(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                _smtpClient.Authenticate(_smtpSettings.UserName, _smtpSettings.Password);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.UserName));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                await _smtpClient.SendAsync(message);
                
                return new EmailResult { Success = true };
            }
            catch (Exception ex)
            {
                return new EmailResult { Success = false, Errors = new[] { ex.Message } };
            }
            finally
            {
                await _smtpClient.DisconnectAsync(true);
            }
        }
    }
    
}
    

