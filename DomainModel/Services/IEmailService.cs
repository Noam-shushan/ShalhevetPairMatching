using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public interface IEmailService
    {
        Task<IEnumerable<EmailModel>> GetEmails();
        Task ResendEmail(EmailModel emailModel);
        Task<EmailModel> SendEmail(EmailModel email);
        Task VerifieyEmails();
    }
}
