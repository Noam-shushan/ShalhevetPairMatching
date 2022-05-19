using System;
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

        public MatchingTimes MatchingTimes { get; set; }

        public bool IsEnglishLevelMatch { get; set; }

        public bool IsSkillLevelMatch { get; set; }

        public bool IsLearningStyleMatch { get; set; }

        public bool IsGenderMatch { get; set; }

    }
}
