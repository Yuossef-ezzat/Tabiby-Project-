using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace PL.Utilites
{
    public static class EmailSettings
    {
        private static string _senderEmail = string.Empty;
        private static string _appPassword = string.Empty;

        public static void Initialize(IConfiguration configuration)
        {
            _senderEmail = configuration["EmailSettings:SenderEmail"]!;
            _appPassword = configuration["EmailSettings:AppPassword"]!;
        }

        public async static Task<bool> SendEmail(Email email)
        {
            try
            {

                var client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_senderEmail, _appPassword);
                await client.SendMailAsync(_senderEmail, email.To, email.Subject, email.Body);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

