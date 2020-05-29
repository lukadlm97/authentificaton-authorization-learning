using Auth.WebApi.EmailSender.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.WebApi.EmailSender
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
