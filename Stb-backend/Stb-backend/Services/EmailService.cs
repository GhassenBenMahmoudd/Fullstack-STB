using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace stb_backend.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var message = new MailMessage();
            message.To.Add(to);
            message.Subject = subject;
            message.Body = htmlContent;
            message.IsBodyHtml = true;
            message.From = new MailAddress("ghassenbenmahmoud6@gmail.com");

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("ghassenbenmahmoud6@gmail.com", "horbzkpmjqsobvih")
            };

            await smtp.SendMailAsync(message);
        }
    }
}