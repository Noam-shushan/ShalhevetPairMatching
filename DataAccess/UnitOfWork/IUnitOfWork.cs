using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        ConfigRepositry ConfigRepositry { get; init; }

        IModelRepository<OldPairDto> PairsRepositry { get; init; }

        IModelRepository<Student> StudentRepositry { get; init; }

        IModelRepository<Participant> ParticipantsRepositry { get; init; }

        IModelRepository<MatchingHistory> MatchingHistorisRepositry { get; init; }

        IModelRepository<Email> EmailsRepositry { get; init; }


        Task Complete();
    }
}