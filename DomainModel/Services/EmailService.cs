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
    // creat class that implament IEmailService
    public class EmailService : IEmailService
    {
        readonly WixDataReader _wix;

        readonly IUnitOfWork _unitOfWork;

        public EmailService(IDataAccessFactory dataAccessFactory, MyConfiguration configuration)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();
            _wix = new WixDataReader(configuration);
        }

        public async Task SendEmailTest(IEnumerable<string> to, string subject, string body)
        {
            await _wix.SendEmail(new EmailWixDto
            {
                to = to,
                subject = subject,
                body = body 
            });
        }

    }
}
