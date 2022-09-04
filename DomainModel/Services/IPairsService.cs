using PairMatching.DomainModel.BLModels;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public interface IPairsService
    {
        Task<Pair> AddNewPair(PairSuggestion pairSuggestion);
        
        Task<IEnumerable<Pair>> GetAllPairs();
        
        Task<IEnumerable<StandbyPair>> GetAllStandbyPairs();
    }

}
