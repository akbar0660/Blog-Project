using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories.EmailSender
{
    public interface IEmailSenderRepository
    {
        void SendEmail(string email , string message , string subject);
    }
}
