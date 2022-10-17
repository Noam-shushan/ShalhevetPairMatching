using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWorks;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.WixApi;
using System;
using System.Collections.Generic;
using System.Linq;
using PairMatching.Models;
using PairMatching.Models.Dtos;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public class EmailService : IEmailService
    {
        readonly WixDataReader _wix;

        readonly IUnitOfWork _unitOfWork;

        public EmailService(IDataAccessFactory dataAccessFactory, MyConfiguration configuration)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();
            _wix = new WixDataReader(configuration);
        }

        public async Task<IEnumerable<EmailModel>> GetEmails()
        {
            var emails = await _unitOfWork
                .EmailRepositry
                .GetAllAsync(e => e.IsVerified);
            
            return emails;
        }

        public async Task<EmailModel> SendEmail(EmailModel emailModel)
        {
            var email = new
            {
                to = emailModel.To.Select(e => e.ParticipantWixId),
                subject = emailModel.Subject,
                body = emailModel.Body,
                htmlBody = emailModel.HtmlBody,
                hasHtmlBody = emailModel.HasHtmlBody,
                link = emailModel.Links,
                language = emailModel.Language
            };

            var emailWixId = await _wix.SendEmail(email);

            emailModel.WixId = emailWixId;
            
            return await _unitOfWork
                .EmailRepositry
                .Insert(emailModel);           
        }

        public async Task VerifieyEmails()
        {
            var emails = await _unitOfWork
                .EmailRepositry
                .GetAllAsync(e => !e.IsVerified);

            var tasks = new List<Task>();

            foreach (var email in emails)
            {
                var emailRecipents = await _wix.VerifieyEmail(email.WixId);
                if(emailRecipents.Any())
                {
                    email.SendTo = from e in emailRecipents
                                   where e.IsSent
                                   select e.WixId;
                    email.IsVerified = true;
                    tasks.Add(_unitOfWork.EmailRepositry.Update(email));
                }
            }

            await Task.WhenAll(tasks);
        }
    }

    public class NotValidAddress
    {
        public string ParticipaintName { get; set; }

        public EmailAddress EmailAddress { get; set; }
    }
}
