using System.Net;
using System.Net.Mail;

namespace API.InternalClasses
{
    internal static class SendEmail
    {
        public static void SendLoginInformation(string email, string password)
        {
            MailAddress from = new(_email, "SkyWhySales");
            MailAddress to = new(email);

            MailMessage mail_message = new(from, to)
            {
                Subject = "Информация для входа в приложение",
                Body = $"<h2>Логин: {email}</h2><br/><h2>Пароль: {password}</h2>",
                IsBodyHtml = true
            };

            SmtpClient smtp = new("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(_email, _appPassword),
                UseDefaultCredentials = false,
                EnableSsl = true
            };

            smtp.Send(mail_message);
        }

        private const string _email = "ksebija@gmail.com";
        private const string _appPassword = "brsq xdqx jzce jsqm";

    }
}
