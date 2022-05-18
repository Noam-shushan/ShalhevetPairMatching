using System;

namespace PairMatching.Models
{
    public class MatchingHistoryDetails
    {
        public DateTime DateOfMatch { get; set; }

        public DateTime DateOfActive { get; set; }

        public DateTime DateOfUnMatch { get; set; }

        public PrefferdTracks Track { get; set; }
    }
}