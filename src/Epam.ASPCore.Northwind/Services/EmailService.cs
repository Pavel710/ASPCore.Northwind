using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Epam.ASPCore.Northwind.WebUI.Settings;
using Microsoft.Extensions.Options;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "Admin")
                };
                mail.To.Add(new MailAddress(email));

                mail.Subject = "Restore password - " + subject;
                mail.Body = htmlMessage;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(_emailSettings.Domain, _emailSettings.Port))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception e)
            {
                Log.Error("Email service error!" + Environment.NewLine + $"{e}");
            }
        }
    }
}
