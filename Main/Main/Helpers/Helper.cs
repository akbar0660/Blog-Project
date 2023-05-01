using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Main.Helpers
{
    public static class Helper
    {
        public enum Roles
        {
            Admin,
            Writer,
            Customer,
        }
        public static async Task SendMailAsync(string messageSubject, string messageBody, string mailTo)
        {

            SmtpClient client = new SmtpClient("smtp.yandex.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("akbarbabayev0@gmail.com", "akbarbabayev010101");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage message = new MailMessage("akbarbabayev0@gmail.com", mailTo);
            message.Subject = messageSubject;
            message.Body = messageBody;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;


            await client.SendMailAsync(message);


        }
    }
}
