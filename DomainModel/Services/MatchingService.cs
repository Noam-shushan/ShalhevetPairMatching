using PairMatching.DataAccess.UnitOfWork;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.DomainModel.MatchingCalculations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PairMatching.Models;

namespace PairMatching.DomainModel.Services
{
    public class MatchingService : IMatchingService
    {
        readonly IUnitOfWork _unitOfWork;

        BuildSuggestions _suggestionsBuilder;

        public MatchingService(IDataAccessFactory dataAccessFactory)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();
        }
       
        public async Task<IEnumerable<PairSuggestion>> GetAllPairSuggestions()
        {
            var sb = await GetSuggestionsBuilder();

            return sb.GetPairSuggestions();
        }

        public async Task<IEnumerable<PairSuggestion>> GetMaxOptMatching()
        {
            var sb = await GetSuggestionsBuilder();

            return sb.FindMaxOptPairs();
        }

        public async Task<IEnumerable<PairSuggestion>> GetMaxMatching()
        {
            var sb = await GetSuggestionsBuilder();

            return sb.FindMaxPairs();
        }
        
        public void SetSuggestions(IEnumerable<IsraelParticipant> israeliParticipants, IEnumerable<WorldParticipant> worldParticipants)
        {
            _suggestionsBuilder = new BuildSuggestions(israeliParticipants, worldParticipants);
        }

        public async Task Refresh()
        {
            var israelParticipants = await _unitOfWork
                 .IsraelParticipantsRepositry
                 .GetAllAsync();
            var worldParticipants = await _unitOfWork
                .WorldParticipantsRepositry
                .GetAllAsync();

            _suggestionsBuilder = new BuildSuggestions(israelParticipants.Where(i => i.IsOpenToMatch),
                worldParticipants.Where(i => i.IsOpenToMatch));
        }

        async Task<BuildSuggestions> GetSuggestionsBuilder()
        {
            if(_suggestionsBuilder is null)
                await Refresh();
            return _suggestionsBuilder;
        }

        public async Task AddMatchingHistory(PairSuggestion pairSuggestion)
        {
            await _unitOfWork.MatchingHistorisRepositry.Insert(null);
        }
              
    }
}
