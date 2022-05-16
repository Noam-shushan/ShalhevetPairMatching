using PairMatching.DataAccess.Repositories;
using PairMatching.DataAccess.UnitOfWork;
using PairMatching.DomainModel.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Domains
{
    public class Testing
    {
        readonly IUnitOfWork _unitOfWork;

        readonly SendEmail _emailSender;

        public Testing(IUnitOfWork unitOfWork, SendEmail emailSender)
        {
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        public async Task TestEmailToOne()
        {
            var student = await _unitOfWork
                .StudentRepositry.GetByIdAsync(3);

            await _emailSender.To(student.Email)
                .SendAutoEmailAsync(student, Templates.SuccessfullyRegisteredHebrew);

        }

        public async Task TestEmailToMany()
        {
            var students = await _unitOfWork
                .StudentRepositry.GetAllAsync();

            var emails = from s in students
                         select s.Email;

            await _emailSender.To(emails.ToArray())
                .Subject("Test many: subject")
                .Body("Test many: body")
                .SendOpenMailAsync(new List<string>
                {
                    @"C:\Users\Asuspcc\Desktop\מכון לב\מסמכים\appendix11702.pdf"
                });
        }
    }
}
