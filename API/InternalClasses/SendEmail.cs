using System.Net;
using System.Net.Mail;

namespace API.InternalClasses
{
    public static class SendEmail
    {
        public static void SendLoginInformation(string email, string password)
        {
            MailAddress from = new MailAddress("ksebija@gmail.com", "SkyWhySales");
            MailAddress to = new MailAddress(email);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Информация для входа в приложение";
            m.Body = $"<h2>Логин: {email}</h2><br/><h2>Пароль: {password}</h2>";
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("ksebija@gmail.com", "brsq xdqx jzce jsqm");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
