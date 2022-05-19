using PairMatching.DataAccess.Infrastructure;
using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System.Threading.Tasks;
using PairMatching.Configurations;

namespace PairMatching.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        const string studentsCollectionName = "Students";
        
        const string participantsCollectionName = "Participants";

        const string emailsCollectionName = "Emails";

        const string matchingHistoryCollectionName = "MatchingHistory";

        const string pairsCollectionName = "Pairs";

        public UnitOfWork(MyConfiguration configurations)
        {
            var dataAccess = new ContextFactory(configurations)
                .GetContext();

            StudentRepositry = new ModelRepositroy<Student>(dataAccess, studentsCollectionName, _taskManeger);

            PairsRepositry = new ModelRepositroy<Pair>(dataAccess, pairsCollectionName, _taskManeger);

            ConfigRepositry = new ConfigRepositry(dataAccess);

            ParticipantsRepositry = new ModelRepositroy<Participant>(dataAccess, participantsCollectionName, _taskManeger);

            EmailsRepositry = new ModelRepositroy<Email>(dataAccess, emailsCollectionName, _taskManeger);

            MatchingHistorisRepositry = new ModelRepositroy<MatchingHistory>(dataAccess, matchingHistoryCollectionName, _taskManeger);
        }

        public IModelRepository<Student> StudentRepositry { get; init; }

        public IModelRepository<Pair> PairsRepositry { get; init; }

        public IModelRepository<Participant> ParticipantsRepositry { get; init; }

        public IModelRepository<MatchingHistory> MatchingHistorisRepositry { get; init; }

        public IModelRepository<Email> EmailsRepositry { get; init; }

        public ConfigRepositry ConfigRepositry { get; init; }

        readonly TaskManeger _taskManeger = new();


        public async Task Complete()
        {
            await _taskManeger.SaveChangesAsync();
        }
    }
}
