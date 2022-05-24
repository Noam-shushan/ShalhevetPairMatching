using System;

namespace PairMatching.Models
{
    public record NoteOld
    {
        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime Date { get; set; }
    }
}