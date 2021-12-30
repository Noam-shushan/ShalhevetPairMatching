using System;
using System.Collections.Generic;

namespace PairMatching.Models
{
    public class StudentMatchingHistory
    {
        public DateTime DateOfMatch { get; set; }
        public DateTime DateOfUnMatch { get; set; }
        public List<Tuple<DateTime, PrefferdTracks>> TracksHistory { get; set; } 
            = new();
        public string MatchStudentName { get; set; }
        public int MatchStudentId { get; set; }
        public bool IsUnMatch { get; set; }
        public bool IsActive { get; set; }
    }
}