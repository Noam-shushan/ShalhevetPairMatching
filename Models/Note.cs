using System;

namespace PairMatching.Models
{
    public class Note : IEquatable<Note>
    {
        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime Date { get; set; }

        public bool Equals(Note other)
        {
            if (other == null)
            {
                return false;
            }
            return Text == other.Text && Author == other.Author && Date == other.Date;
        }

        public override bool Equals(object obj) => Equals(obj as Note);

        public override int GetHashCode() => (Text, Author, Date).GetHashCode();
    }
}