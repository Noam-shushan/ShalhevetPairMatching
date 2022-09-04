using PairMatching.DomainModel.MatchingCalculations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public interface IMatchingService
    {
        Task<IEnumerable<PairSuggestion>> GetAllPairSuggestions();
        Task<IEnumerable<PairSuggestion>> GetMaxMatching();
        Task<IEnumerable<PairSuggestion>> GetMaxOptMatching();
        Task Refresh();
    }
}
