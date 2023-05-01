using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Repositories.EmailSender;

namespace Main.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailSenderRepository _emailSender;
        public EmailController(IEmailSenderRepository emailSender)
        {
            _emailSender = emailSender;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult SendEmail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendEmail(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                var receiver = model.ToEmail;
                var subject = model.Subject;
                var message = model.Message;
                if (receiver != null)
                {
                     _emailSender.SendEmail(receiver, subject, message);
                    return RedirectToAction("EmailSendedSuccessfully");
                }
            }
            return View(model);
        }
        public IActionResult EmailSendedSuccessfully()
        {
            return View();
        }

        //public IActionResult SendEmail()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SendEmail(EmailModel model)
        //{

        //    var smtpClient = new SmtpClient("smtp.gmail.com", 587)
        //    {
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential("akbarbabayev0@gmail.com", "akbarbabayev010101"),
        //        EnableSsl = true
        //    };

        //    try
        //    {
        //        var mailMessage = new MailMessage
        //        {
        //            From = new MailAddress("akbarbabayev0@gmail.com"),
        //            To = { model.ToEmail },
        //            Subject = model.Subject,
        //            Body = model.Message
        //        };

        //        await smtpClient.SendMailAsync(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrorMessage = $"An error occurred while sending the email: {ex.Message}";
        //        return View(model);
        //    }
        //    return RedirectToAction("EmailSendedSuccessfully");
        //}

    }
}
