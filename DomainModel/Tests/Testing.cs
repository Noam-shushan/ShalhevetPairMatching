using PairMatching.Configurations;
using PairMatching.DataAccess.UnitOfWork;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.DomainModel.Email;
using PairMatching.Models;
using PairMatching.Models.Dtos;
using PairMatching.Tools;
using PairMatching.WixApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Tests
{
    public class Testing
    {
        readonly IUnitOfWork _unitOfWork;

        readonly SendEmail _emailSender;

        readonly WixDataReader _wix;

        public Testing(SendEmail emailSender, MyConfiguration configuration, IDataAccessFactory dataAccessFactory)
        {
            _wix = new WixDataReader(configuration);
            _emailSender = emailSender;
            _unitOfWork = dataAccessFactory.GetDataAccess();
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

        public async Task NewPairToWixTest()
        {
            await _wix.NewPair(new NewPairWixDto
            {
                chevrutaIdFirst = "8f60532a-de89-427a-b628-ede463dcf093",
                chevrutaIdSecond = "e1bc6d5e-9913-4e2b-9fae-26e42bfcc556",
                date = DateTime.Now,
                track = PrefferdTracks.Payer.GetDescriptionFromEnumValue()
            });
        }

        // TODO : finish her
        public async Task MoveToNewDatabaseWithMatchingHistory()
        {
            var students = await _unitOfWork
                .StudentRepositry
                .GetAllAsync(s => !s.IsDeleted);

            var israelParts = await _unitOfWork
                .IsraelParticipantsRepositry
                .GetAllAsync();

            var worldParts = await _unitOfWork
                .WorldParticipantsRepositry
                .GetAllAsync();

            var matchingHistoryPairStudentId = students
                .Where(s => s.IsFromIsrael && s.MatchingHistories.Count > 0)
                .Select(s => new Tuple<int, List<StudentMatchingHistory>>(s.Id, s.MatchingHistories));

            var result = new List<MatchingHistory>();
            foreach (var mhs in matchingHistoryPairStudentId)
            {
                foreach (var mh in mhs.Item2)
                {
                    var ipId = israelParts.FirstOrDefault(p => p.OldId == mhs.Item1)?.Id;
                    var wpId = worldParts.FirstOrDefault(p => p.OldId == mh.MatchStudentId)?.Id;

                    if (ipId != null && wpId != null)
                    {
                        var m = new MatchingHistory
                        {
                            FromIsraelId = ipId,
                            FromWorldId = wpId,
                            History = from mht in mh.TracksHistory
                                      select
                                      new MatchingHistoryDetails
                                      {
                                          DateOfMatch = mh.DateOfMatch,
                                          DateOfUnMatch = mh.DateOfUnMatch,
                                          Track = mht.Item2
                                      },
                            IsActive = mh.IsActive,
                            IsUnMatch = mh.IsUnMatch,
                        };
                        result.Add(m);
                    }
                }
            }

            await _unitOfWork.MatchingHistorisRepositry.Insert(result);
        }
    }
}
