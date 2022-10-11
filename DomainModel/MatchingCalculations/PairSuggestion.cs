using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public class PairSuggestion
    {
        public Participant FromIsrael { get; set; }

        public Participant FromWorld { get; set; }

        public List<MatchingTime> MatchingTimes { get; set; } = new List<MatchingTime>();

        public bool IsEnglishLevelMatch { get; set; }

        public bool IsSkillLevelMatch { get; set; }

        public bool IsLearningStyleMatch { get; set; }

        public bool IsGenderMatch { get; set; }

        public int MatchingScore { get; set; }

        public double MatchingPercent => Math.Round((double)(100 * MatchingScore) / 26);

        public IEnumerable<PrefferdTracks> PrefferdTrack { get; set; }

        public PrefferdTracks ChosenTrack { get; set; }

        public bool IsTrackMatch { get; internal set; }

        public bool IsMinmunMatch 
        {
            get =>
                MatchingTimes.Count > 0 &&
                IsEnglishLevelMatch &&
                IsSkillLevelMatch &&
                IsGenderMatch &&
                IsTrackMatch;
        }
    }
}
