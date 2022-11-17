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
        Task<Pair> ActivePair(Pair pair);
        Task<Pair> AddNewPair(PairSuggestion pairSuggestion, PrefferdTracks track = PrefferdTracks.NoPrefrence);
        Task AddNote(Note newNote, Pair pairModel);
        Task CancelPair(Pair pair);
        Task ChangeTrack(Pair pair, PrefferdTracks track);
        Task DeleteNote(Note selectedNote, Pair pair);
        Task DeletePair(Pair pair);
        Task<IEnumerable<Pair>> GetAllPairs();
        
        Task<IEnumerable<StandbyPair>> GetAllStandbyPairs();
        Task UpdatePair(Pair pair);
        Task VerifieyNewPairsInWix();
    }

}
