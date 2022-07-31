using PairMatching.DataAccess.UnitOfWork;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.DomainModel.MatchingCalculations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public class MatchingService : IMatchingService
    {
        readonly IUnitOfWork _unitOfWork;      

        public MatchingService(IDataAccessFactory dataAccessFactory)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();
        }
       
        public async Task<IEnumerable<PairSuggestion>> GetAllPairSuggestions()
        {            
            var israelParticipants = await _unitOfWork
                .IsraelParticipantsRepositry
                .GetAllAsync();
            var worldParticipants = await _unitOfWork
                .WorldParticipantsRepositry
                .GetAllAsync();

            var suggestionsBuilder = new BuildSuggestions(israelParticipants, worldParticipants);

            return suggestionsBuilder.BuildPairSuggestions();
        } 
    }
}
