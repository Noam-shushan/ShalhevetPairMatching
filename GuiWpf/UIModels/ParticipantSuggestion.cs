using PairMatching.DomainModel.MatchingCalculations;

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
