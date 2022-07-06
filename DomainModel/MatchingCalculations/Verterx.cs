using System;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public enum Kind
    {
        FromIsrael,
        FromWorld,
        Source,
        Target
    }
    public class Verterx : IEquatable<Verterx>
    {
        public string PartId { get; set; }

        public int Index { get; set; }

        public Kind Kind { get; set; }

        public Tuple<Verterx, Edge> Pred { get; set; }

        public bool Equals(Verterx other)
        {
            if (other is null)
                return false;

            return PartId == other.PartId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Verterx);
        }

        public override int GetHashCode()
        {
            return PartId.GetHashCode();
        }
    }
}
