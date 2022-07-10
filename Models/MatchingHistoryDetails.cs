using System;

namespace PairMatching.Models
{
    public class MatchingHistoryDetails : IEquatable<MatchingHistoryDetails>
    {
        public DateTime DateOfMatch { get; set; }

        public DateTime DateOfActive { get; set; }

        public DateTime DateOfUnMatch { get; set; }

        public PrefferdTracks Track { get; set; }

        public bool Equals(MatchingHistoryDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return DateOfMatch == other.DateOfMatch &&
                   Track == other.Track;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MatchingHistoryDetails);
        }

        public override int GetHashCode()
        {
            return (DateOfMatch, Track).GetHashCode();
        }
    }
}