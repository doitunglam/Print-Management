using PM.Application.Handle.HandleEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.InterfaceService
{
    public interface IEmailService
    {
         string SendEmail(EmailMessage emailMessage);
    }
}
