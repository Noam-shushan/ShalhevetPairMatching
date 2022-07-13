using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public class BuildSuggestions
    {
        readonly IEnumerable<IsraelParticipant> _israeliParticipants;
        readonly IEnumerable<WorldParticipant> _worldParticipants;

        public BuildSuggestions(IEnumerable<IsraelParticipant> israeliParticipants, IEnumerable<WorldParticipant> worldParticipants)
        {
            _israeliParticipants = israeliParticipants;
            _worldParticipants = worldParticipants;
        }
        
        public IEnumerable<PairSuggestion> FindMaxPairs()
        {
            var pairSugges = BuildPairSuggestions();
            
            var allParts = _worldParticipants.Select(p => p as Participant)
                .Union(_israeliParticipants.Select(p => p as Participant));
            
            var bm = new BipartiteMatching(pairSugges, allParts);
            
            return null;
        }

        public IEnumerable<PairSuggestion> BuildPairSuggestions()
        {
            var timeIntervalFactory = new TimeIntervalFactory();
            foreach (var ip in _israeliParticipants)
            {
                foreach(var wp in _worldParticipants)
                {
                    var pairSuggestion = new PairSuggestionBulider(ip, wp, timeIntervalFactory)
                        .Build();
                    if(pairSuggestion != null)
                    {
                        yield return pairSuggestion;
                    }
                }
            }
        }     
    }
}
