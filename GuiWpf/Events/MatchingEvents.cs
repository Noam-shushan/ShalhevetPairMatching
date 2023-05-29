using PairMatching.DomainModel.BLModels;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiWpf.Events
{
    public class NewMatchEvent : PubSubEvent<StandbyPair> { }

    public class RefreshMatchingEvent : PubSubEvent { }

    public class NewPairEvent : PubSubEvent<Pair> { }

    public class MatchEvent : PubSubEvent<bool> { }

    public class SearchParticipaintForFreeMatchEvent : PubSubEvent<string> { }

    public class ShowFullComparisonEvent : PubSubEvent<(IEnumerable<PairSuggestion>, string)> { }

}
