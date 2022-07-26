using System;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public class Edge : IEquatable<Edge>
    {
        public Verterx V1 { get; set; }
        
        public Verterx V2 { get; set; }

        public int Capacity { get; set; }

        public int Flow { get; set; }

        public int Weight { get; set; }

        public bool Equals(Edge other)
        {
            if (other is null)
                return false;

            return V1.Equals(other.V1) && V2.Equals(other.V2);
        }

        public override string ToString()
        {
            return $"{V1.PartId}->{V2.PartId}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Edge);
        }

        public override int GetHashCode()
        {
            return V1.GetHashCode() ^ V2.GetHashCode();
        }
    }
}
