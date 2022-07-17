using PairMatching.DataAccess.Infrastructure;
using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System.Threading.Tasks;
using PairMatching.Configurations;

namespace PairMatching.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        const string studentsCollection = "Students";
        
        const string israelParticipantsCollection = "IsraelParticipants";
        
        const string worldparticipantsCollection = "WorldParticipants";

        const string emailsCollection = "Emails";

        const string matchingHistoryCollection = "MatchingHistory";

        const string pairsCollection = "Pairs";
        
        const string newPairsCollection = "NewPairs";

        public UnitOfWork(MyConfiguration configurations)
        {
            var dataAccess = new ContextFactory(configurations)
                .GetContext();

            StudentRepositry = new ModelRepositroy<Student>(dataAccess, studentsCollection, _taskManeger);

            OldPairsRepositry = new ModelRepositroy<OldPairDto>(dataAccess, pairsCollection, _taskManeger);

            ConfigRepositry = new ConfigRepositry(dataAccess);

            IsraelParticipantsRepositry = new ModelRepositroy<IsraelParticipant>(dataAccess, israelParticipantsCollection, _taskManeger);
            
            WorldParticipantsRepositry = new ModelRepositroy<WorldParticipant>(dataAccess, worldparticipantsCollection, _taskManeger);

            PairsRepositry = new ModelRepositroy<Pair>(dataAccess, newPairsCollection, _taskManeger);

            EmailRepositry = new ModelRepositroy<EmailModel>(dataAccess, emailsCollection, _taskManeger);

            MatchingHistorisRepositry = new ModelRepositroy<MatchingHistory>(dataAccess, matchingHistoryCollection, _taskManeger);
        }

        public IModelRepository<IsraelParticipant> IsraelParticipantsRepositry { get; init; }
        
        public IModelRepository<WorldParticipant> WorldParticipantsRepositry { get; init; }

        public IModelRepository<Student> StudentRepositry { get; init; }

        public IModelRepository<OldPairDto> OldPairsRepositry { get; init; }
        
        public IModelRepository<Pair> PairsRepositry { get; init; }

        public IModelRepository<MatchingHistory> MatchingHistorisRepositry { get; init; }

        public IModelRepository<EmailModel> EmailRepositry { get; init; }

        public ConfigRepositry ConfigRepositry { get; init; }
        
        readonly TaskManeger _taskManeger = new();

        public async Task Complete()
        {
            await _taskManeger.SaveChangesAsync();
        }


    }
}
