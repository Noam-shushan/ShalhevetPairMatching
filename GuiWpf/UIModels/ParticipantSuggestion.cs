using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;
using System.Collections.Generic;

namespace GuiWpf.UIModels
{
    public class ParticipantSuggestion
    {
        public string Id { get; set; }

        public string Country { get; set; }

        public string Name { get; set; }

        public double MatchingPercent { get; set; }

        public PairSuggestion Original { get; set; }
    }
}
