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

        readonly string _logId;

        public EmailService(IDataAccessFactory dataAccessFactory, MyConfiguration configuration)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();
            _wix = new WixDataReader(configuration);
            _logId = configuration.LogIdForError;
        }

        public async Task<IEnumerable<EmailModel>> GetEmails()
        {
            var emails = await _unitOfWork
                .EmailRepositry
                .GetAllAsync();

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
                link = emailModel.Link,
                language = emailModel.Language
            };

            var emailWixId = await _wix.SendEmail(email);

            emailModel.WixId = emailWixId;
            emailModel.IsVerified = false;
            emailModel.Date = DateTime.Now;
            
            return await _unitOfWork
                .EmailRepositry
                .Insert(emailModel);           
        }

        public async Task ResendEmail(EmailModel emailModel) 
        {
            var email = new
            {
                to = emailModel.MissSentAddress.Select(e => e.ParticipantWixId),
                subject = emailModel.Subject,
                body = emailModel.Body,
                htmlBody = emailModel.HtmlBody,
                hasHtmlBody = emailModel.HasHtmlBody,
                link = emailModel.Link,
                language = emailModel.Language
            };

            var emailWixId = await _wix.SendEmail(email);

            emailModel.WixId = emailWixId;
            emailModel.IsVerified = false;

            await _unitOfWork
                .EmailRepositry
                .Update(emailModel);
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
                if (emailRecipents.Any())
                {
                    email.SendTo = (from e in emailRecipents
                                    where e.IsSent
                                    select e.WixId).ToList();
                    email.IsVerified = true;
                    tasks.Add(_unitOfWork.EmailRepositry.Update(email));
                }
            }

            await Task.WhenAll(tasks);
        }

        public async Task LogErrorToDev(Exception exception)
        {
            var email = new
            {
                to = new string[]{ _logId },
                subject = "Bug in PairMatching",
                body = $"Message: {exception.Message}\nSource: {exception.Source}\nStack Trace: {exception.StackTrace}",
                htmlBody = "",
                hasHtmlBody = false,
                link = "",
                language = "en"
            };

            var emailWixId = await _wix.SendEmail(email);
        }
    }
}
