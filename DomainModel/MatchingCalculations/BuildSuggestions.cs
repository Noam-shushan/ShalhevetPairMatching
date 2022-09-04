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

        readonly IEnumerable<PairSuggestion> _pairSuggestions;

        readonly BipartiteMatching _bipartiteMatching;

        public BuildSuggestions(IEnumerable<IsraelParticipant> israeliParticipants, IEnumerable<WorldParticipant> worldParticipants)
        {
            _israeliParticipants = israeliParticipants;
            _worldParticipants = worldParticipants;
            
            _pairSuggestions = BuildPairSuggestions();          

            _bipartiteMatching = new BipartiteMatching(_pairSuggestions);
        }
        
        public IEnumerable<PairSuggestion> FindMaxPairs()
        {         
            var edges = _bipartiteMatching.EdmoudnsKarp();

            return from p in _pairSuggestions
                   where edges.Any(e => e.V1.PartId == p.FromIsrael.Id && e.V2.PartId == p.FromWorld.Id)
                   select p;
        }

        public IEnumerable<PairSuggestion> FindMaxOptPairs()
        {
            return _bipartiteMatching.HungarianAlgo();
        }

        public IEnumerable<PairSuggestion> GetPairSuggestions()
        {
            return _pairSuggestions;
        }

        IEnumerable<PairSuggestion> BuildPairSuggestions()
        {
            var timeIntervalFactory = new TimeIntervalFactory();
            foreach (var ip in _israeliParticipants)
            {
                foreach (var wp in _worldParticipants)
                {
                    var pairSuggestion = new PairSuggestionBulider(ip, wp, timeIntervalFactory)
                        .Build();
                    if (pairSuggestion != null)
                    {
                        yield return pairSuggestion;
                    }
                }
            }
        }
    }
}
