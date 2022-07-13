using PairMatching.Models;
using System;
using System.Collections.Generic;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public class TimeInterval
    {
        public TimeInterval(TimeSpan start, TimeSpan end, int day)
        {
            Start = start + TimeSpan.FromDays(day);
            End = end + TimeSpan.FromDays(day);
        }
        
        public TimeInterval() { }

        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public TimeSpan FitWith(TimeInterval other, TimeSpan diffrence)
        {
            var temp = Reduce(diffrence);
            
            var maxStart = TimeSpan.FromMinutes(Math.Max(temp.Start.TotalMinutes, other.Start.TotalMinutes));
            var  minEnd = TimeSpan.FromMinutes(Math.Min(temp.End.TotalMinutes, other.End.TotalMinutes));

            return minEnd - maxStart;
        }

        public TimeInterval Reduce(TimeSpan time)
        {
            return new TimeInterval
            {
                Start = Start + time,
                End = End + time
            };
        }
        
        public override string ToString()
        {
            return $"{Start} - {End}";
        }
    }
}
