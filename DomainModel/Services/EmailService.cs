using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWork;
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
                .GetAllAsync();
            return emails;
        }

        public async Task SendEmail(EmailModel email)
        {
            var sendTo = await _wix.SendEmail(email);
            if(!sendTo.Any())
            {
                return;
            }
            email.SendTo = sendTo;
            await _unitOfWork
                .EmailRepositry
                .Insert(email);           
        }

        public async Task<IEnumerable<NotValidAddress>> GetNotValidAddress()
        {
            var emails = await _unitOfWork
                .EmailRepositry
                .GetAllAsync();
            return null;
        }
    }

    public class NotValidAddress
    {
        public string ParticipaintName { get; set; }

        public EmailAddress EmailAddress { get; set; }
    }
}
