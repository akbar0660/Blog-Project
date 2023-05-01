using Application.Repositories.EmailSender;

using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories.EmailSender
{
    public class EmailSenderRepository : IEmailSenderRepository
    {
        //public Task SendEmailAsync(string email, string message, string subject)
        //{
        //    var mail = "akbarbabayev0@gmail.com";
        //    var password = "akbarbabayev010101";
        //    var client = new SmtpClient("smtp-mail.outlook.com", 587)
        //    {
        //        EnableSsl = true,
        //        Credentials = new NetworkCredential(mail, password)
        //    };
        //    return client.SendMailAsync(
        //        new MailMessage(
        //            from: mail,
        //            to: email,
        //            subject,
        //            message
        //            ) );
        //}
        public void SendEmail(string email, string message, string subject)
        {
            var fromAddress = new MailAddress("akbarbabayev2004@outlook.com");
            var fromPassword = "akbarbabayev010101";
            var toAddress = new MailAddress(email);

            string subjectt = subject;
            string body = message;

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp-mail.outlook.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var messagee = new MailMessage(fromAddress, toAddress)
            {
                Subject = subjectt,
                Body = body
            })

                smtp.Send(messagee);
        }
    }
}
