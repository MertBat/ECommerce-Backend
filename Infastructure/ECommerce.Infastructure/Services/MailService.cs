using Amazon.Runtime.Internal.Util;
using ECommerce.Application.Abstraction.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infastructure.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailService> _logger;

        public MailService(IConfiguration configuration , ILogger<MailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task SendMessageAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
           await SendMessageAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendMessageAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                MailMessage mail = new();
                mail.IsBodyHtml = isBodyHtml;
                foreach (string s in tos)
                {
                    mail.To.Add(s);
                }

                mail.Subject = subject;
                mail.Body = body;
                mail.From = new(_configuration["MailService:EMail"], _configuration["MailService:Name"], System.Text.Encoding.UTF8);

                SmtpClient smtp = new();
                smtp.Credentials = new NetworkCredential(_configuration["MailService:EMail"], _configuration["MailService:Password"]);
                smtp.Port = Convert.ToInt32(_configuration["MailService:Port"]);
                smtp.EnableSsl = true;
                smtp.Host = _configuration["MailService:Host"];
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Mail servcice: {ex.Message}");
            }
           
        }
    }
}
