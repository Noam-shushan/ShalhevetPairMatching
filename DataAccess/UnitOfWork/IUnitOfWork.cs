using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        ConfigRepositry ConfigRepositry { get; init; }

        IModelRepository<Pair> PairsRepositry { get; init; }

        IModelRepository<Student> StudentRepositry { get; init; }

        IModelRepository<Participant> ParticipantsRepositry { get; init; }

        Task Complete();
    }
}