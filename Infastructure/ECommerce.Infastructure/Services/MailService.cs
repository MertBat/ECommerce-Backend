using Amazon.Runtime.Internal.Util;
using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.DTOs.Mail;
using ECommerce.Domain.Entities;
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

        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendCompletedOrderMailAsync(string to, string orderCode, DateTime orderDate, bool orderStatus, List<MailListProductDTO> orderList, long totalPrice)
        {
            string table = $@"
<table style=""border-collapse: collapse; width: 100%;"">
  <thead>
    <tr>
      <th style=""border: 1px solid #ddd; padding: 8px; background-color: #b4cafa; text-align: left;"">Product</th>
      <th style=""border: 1px solid #ddd; padding: 8px; background-color: #b4cafa; text-align: left;"">Quantity</th>
      <th style=""border: 1px solid #ddd; padding: 8px; background-color: #b4cafa; text-align: left;"">Price</th>
    </tr>
  </thead>
  <tbody>";

            int index = 0;
            foreach (var item in orderList)
            {
                table += $@"
<tr>
  <td style=""border: 1px solid #ddd; padding: 8px; {(index % 2 == 1 ? "background-color: #f2f2f2;" : "")}"">{item.ProductName}</td>
  <td style=""border: 1px solid #ddd; padding: 8px; {(index % 2 == 1 ? "background-color: #f2f2f2;" : "")}"">{item.Quantity}</td>
  <td style=""border: 1px solid #ddd; padding: 8px; {(index % 2 == 1 ? "background-color: #f2f2f2;" : "")}"">{item.Price}₺</td>
</tr>";
                index++;
            }


            table += $@"
    <tr>
      <td style=""border: 1px solid #ddd; padding: 8px; text-align: right;"" colspan=""2"">Total price:</td>
      <td style=""border: 1px solid #ddd; padding: 8px;"">{totalPrice}₺</td>
    </tr>
  </tbody>
</table>";

            string content = (orderStatus ? "Order Accepted" : "Order Denied") + $" ({orderDate})";
            string header = "Your products are listed below;";
            string footer = orderStatus ? "We are pleased to inform you that your products have been processed successfully. Thank you for your business." : "We regret to inform you that your products have been denied. Please contact us for further information.";
            string mailBody = MessageFormat(content, header, table, footer);

            await SendMessageAsync(to, "Order Information", mailBody);

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

        public async Task SendPasswordResetMessageAsync(string to, string userId, string resetToken)
        {
            string messageLink = $"<a href='{_configuration["ClientUrl"]}update-password/{userId}/{resetToken}' style='background-color: #007bff; color: #fff; text-decoration: none; padding: 10px 20px; border-radius: 5px; display: inline-block;'>Reset Password</a>";

            string mailBody = MessageFormat("Password Reset", "We received a request to reset your password. To create a new password, click the button below:", messageLink, "If you didn't request this, you can ignore this email. Your password will not be changed.");

            await SendMessageAsync(to, "Reset Password", mailBody);
        }

        private string MessageFormat(string messageContent, string messageHeader, string messageBody, string messageFooter)
        {
            return $@"<!DOCTYPE html>
    <html lang=""en"">
    <head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset</title>
    </head>
    <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
    
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 10px; box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1);"">
    <tr>
    <td align=""center"">
    <img src=""https://i.pinimg.com/originals/75/fa/9b/75fa9b17f632646e5ae7fae3cf837761.jpg"" width=""150"" style=""display: block; margin-bottom: 20px;"">
    </td>
    </tr>
    <tr>
    <td>
    <h2 style=""margin-top: 0;"">{messageContent}</h2>
    <p>Hello,</p>
    <p>{messageHeader}</p>
    </td>
    </tr>
    <tr>
    <td align=""center"">
    {messageBody}
    </td>
    </tr>
    <tr>
    <td>
    <p>{messageFooter}</p>
    </td>
    </tr>
    <tr>
    <td>
    <p>Regards,<br> The ECommerce Team</p>
    </td>
    </tr>
    </table> 
    </body>
    </html>";
        }
    }
}
