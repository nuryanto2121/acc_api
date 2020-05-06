using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Interface
{
    public interface IEmailService
    {
        Output SendEmail(EmailModel eMail);
        Task<Output> SendEmailAsync(EmailModel eMail);
    }
}
