using PairMatching.DataAccess.Infrastructure;
using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System.Threading.Tasks;
using PairMatching.Configuration;

namespace PairMatching.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        const string studentsCollectionName = "Students";

        const string pairsCollectionName = "Pairs";

        public IModelRepository<Student> StudentRepositry { get; init; }

        public IModelRepository<Pair> PairRepositry { get; init; }

        public ConfigRepositry ConfigRepositry { get; init; }

        readonly TaskManeger _taskManeger = new();

        public UnitOfWork(Configuration.MyConfiguration configurations)
        {
            var dataAccess = new ContextFactory(configurations)
                .GetContext();

            StudentRepositry = new ModelRepositroy<Student>(dataAccess, studentsCollectionName, _taskManeger);

            PairRepositry = new ModelRepositroy<Pair>(dataAccess, pairsCollectionName, _taskManeger);

            ConfigRepositry = new ConfigRepositry(dataAccess);
        }

        public async Task Complete()
        {
            await _taskManeger.SaveChangesAsync();
        }
    }
}
