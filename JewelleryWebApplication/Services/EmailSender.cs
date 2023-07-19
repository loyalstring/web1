
using JewelleryWebApplication.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace JewelleryWebApplication.Services
{
    public class EmailSender :IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly IEmailSender _emailSender;
        private IConfigurationRoot _configRoot;
        public EmailSender(IOptions<EmailSettings> emailSettings, IConfiguration configRoot)
        {
            _emailSettings = emailSettings.Value;
            _configRoot = (IConfigurationRoot)configRoot;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_emailSettings.MailServer)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password),
                Port = _emailSettings.MailPort,
                EnableSsl = _emailSettings.SSL
            };

            //var client = new SmtpClient(_configRoot["EmailSettings.MailServer"])
            //{
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(_configRoot["EmailSettings.Sender"], _configRoot["EmailSettings.Password"]),
            //    Port = Convert.ToInt32(_configRoot["EmailSettings.MailPort"]),
            //    EnableSsl = Convert.ToBoolean(_configRoot["EmailSettings.SSL"])
            //};
            var sender = new MailAddress("info@mkgharejewellers.com", "Ghare Jewellers");
            var mailMessage = new MailMessage
            {
               From = sender,
                
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = htmlMessage;
            mailMessage.IsBodyHtml = true;

            return client.SendMailAsync(mailMessage);
            //  return Task.CompletedTask;
        }

     

        public Task SendEmailAsync(string email, string subject, string htmlMessage, Attachment file)
        {
            var client = new SmtpClient(_emailSettings.MailServer)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password),
                Port = _emailSettings.MailPort,
                EnableSsl = _emailSettings.SSL
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("info@mkgharejewellers.com")
            };
            mailMessage.Attachments.Add(file);
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = htmlMessage;
            mailMessage.IsBodyHtml = true;

            return client.SendMailAsync(mailMessage);
        }
    }
}

