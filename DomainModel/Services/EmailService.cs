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
using PairMatching.Loggin;
using System.Reflection;
using System.Text.RegularExpressions;
using PairMatching.Tools;

namespace PairMatching.DomainModel.Services
{
    public class EmailService : IEmailService
    {
        readonly WixDataReader _wix;

        readonly IUnitOfWork _unitOfWork;

        readonly Logger _logger;

        public EmailService(IDataAccessFactory dataAccessFactory, MyConfiguration configuration, Logger logger)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();
            _wix = new WixDataReader(configuration);
            _logger = logger;
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
            try
            {
                //var propsMenthined = GetProps(emailModel.Body);
                //if (propsMenthined.Any())
                //{
                //    var recipients = _unitOfWork.IsraelParticipantsRepositry.
                //    await SendManyWithTemplte(emailModel, )
                //}
                //var temp = ConfigEmail(emailModel);
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
                emailModel.SendingDate = DateTime.Now;
                
                var newEmail = await _unitOfWork
                    .EmailRepositry
                    .Insert(emailModel);
                
                _logger.LogInformation($"Send email, Id: {newEmail.Id}");
                
                return newEmail;

            }
            catch (Exception ex)
            {
                _logger.LogError("Can not send email from wix", ex);
                throw new UserException("Can not send email from wix");
            }          
        }

        private MatchCollection GetProps(string body)
        {
            return Regex.Matches(body, @"(?<!\w)@\w+");
        }

        private List<EmailModel> ConfigEmail(EmailModel emailModel, IEnumerable<Participant> participants, MatchCollection propsMenthined)
        {
            var emails = new List<EmailModel>();
            foreach (var m in participants)
            {
                var singleEmail = emailModel.Clone();
                foreach (var propName in propsMenthined.ToList())
                {
                    var propVal = m.GetType()
                        .GetProperty(propName.Value.Replace("@", ""));
                    if (propVal != null)
                    {
                        singleEmail.Body = singleEmail.Body.Replace(propName.Value, propVal.GetValue(m).ToString());
                    }
                }
                emails.Add(singleEmail);
            }
            return emails;
        }

        public async Task ResendEmail(EmailModel emailModel) 
        {

            try
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
                _logger.LogInformation($"Resend email, Id: {emailModel.Id}");

                emailModel.WixId = emailWixId;
                emailModel.IsVerified = false;

                await _unitOfWork
                    .EmailRepositry
                    .Update(emailModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not resend email '{emailModel.Id}' from wix", ex);
                throw new UserException("Can not resend email from wix");
            }
        }

        public async Task VerifieyEmails()
        {
            try
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
                if(tasks.Count > 0)
                    _logger.LogInformation($"verifiey emails: {tasks.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Can not verifiey emails", ex);
                throw new UserException("Can not verifiey emails");
            }
        }
    }
}
