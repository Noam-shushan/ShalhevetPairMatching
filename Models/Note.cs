using System;

namespace PairMatching.Models
{
    public record Note
    {
        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime Date { get; set; }
    }
}