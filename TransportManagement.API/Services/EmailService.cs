using System.Net;
using System.Net.Mail;

namespace TransportManagement.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService ( IConfiguration config )
        {
            _config = config;
        }

        public void SendEmail ( string to, string subject, string body, bool isHtml = false )
        {
            // اقرأ الباسورد من appsettings.json
            var smtpPassword = _config["Smtp:AppPassword"];
            if (string.IsNullOrWhiteSpace(smtpPassword))
                throw new Exception("SMTP AppPassword is not set in appsettings.json!");

            using var smtpClient = new SmtpClient(_config["Smtp:Host"])
            {
                Port = int.Parse(_config["Smtp:Port"]),
                Credentials = new NetworkCredential(_config["Smtp:Username"], smtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage(_config["Smtp:From"], to, subject, body)
            {
                IsBodyHtml = isHtml
            };

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("فشل إرسال البريد الإلكتروني", ex);
            }
        }
    }
}
