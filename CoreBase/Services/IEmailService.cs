using CoreBase.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBase.Services
{
    public interface IEmailService
    {
        Task SendTestEmail(EmailOption emailOptions);
    }
}
