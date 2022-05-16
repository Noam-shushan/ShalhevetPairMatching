using MongoDB.Bson.Serialization.Attributes;
using System;


namespace PairMatching.Models
{
    public class SpredsheetLastRange : IEquatable<SpredsheetLastRange>
    {
        [BsonId]
        public int Id { get; } = 2;

        private const string DEFULT_RANGE = "A2:Z";

        public string HebrewSheets { get; set; } = DEFULT_RANGE;

        public string EnglishSheets { get; set; } = DEFULT_RANGE;

        public bool Equals(SpredsheetLastRange other)
        {
            //Check whether the compared object is null.
            if (other is null)
                return false;

            //Check whether the compared object references the same data.
            if (ReferenceEquals(this, other))
                return true;

            return HebrewSheets == other.HebrewSheets
                && EnglishSheets == other.EnglishSheets;
        }

        public override bool Equals(object obj) => Equals(obj as SpredsheetLastRange);

        public override int GetHashCode() => (HebrewSheets, EnglishSheets).GetHashCode();
    }
}
