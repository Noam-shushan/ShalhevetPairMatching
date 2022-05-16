using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        ConfigRepositry ConfigRepositry { get; init; }
        IModelRepository<Pair> PairRepositry { get; init; }
        IModelRepository<Student> StudentRepositry { get; init; }

        Task Complete();
    }
}