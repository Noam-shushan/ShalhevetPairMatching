using System;
using System.Collections.Generic;
using System.Linq;

namespace PairMatching.Models
{
    public class LearningTime : IEquatable<LearningTime>
    {
        public IEnumerable<TimesInDay> TimeInDay { get; set; }
        public Days Day { get; set; }

        public bool Equals(LearningTime other)
        {
            //Check whether the compared object is null.
            if (ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (ReferenceEquals(this, other)) return true;

            return Day == other.Day
                && !TimeInDay.Except(other.TimeInDay).Any()
                && !other.TimeInDay.Except(TimeInDay).Any();
        }

        public override bool Equals(object obj) => Equals(obj as LearningTime);

        public override int GetHashCode() => base.GetHashCode();
    }
}