using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PairMatching.DomainModel.MatchingCalculations
{
    internal class BuildSuggestions
    {
        readonly List<IsraelParticipant> _israeliParticipants;
        readonly List<WorldParticipant> _worldParticipants;

        public BuildSuggestions(List<IsraelParticipant> israeliParticipants, List<WorldParticipant> worldParticipants)
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
            var TimeIntervalFactory = new TimeIntervalFactory();
            foreach (var ip in _israeliParticipants)
            {
                foreach(var wp in _worldParticipants)
                {
                    var pairSuggestion = new PairSuggestionBulider(ip, wp, TimeIntervalFactory)
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
