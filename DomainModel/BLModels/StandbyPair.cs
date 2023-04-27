using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;
using PairMatching.DomainModel.MatchingCalculations;

namespace PairMatching.DomainModel.BLModels
{
    public class StandbyPair
    {
        public Pair Pair { get; set; }

        public PairSuggestion PairSuggestion { get; set; }

    }
}
