using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.UIModels
{
    public class ParticipaintWithSuggestions
    {
        public Participant Participant { get; set; }

        List<ParticipantSuggestion> _suggestions;
        public List<ParticipantSuggestion> Suggestions
        {
            get => _suggestions;
            set
            {
                _suggestions = new(value.OrderByDescending(s => s.MatchingPercent));
            }
        }

        public string SearchParticipaintWordForFreeMatch { get; set; } = "";
    }
}
