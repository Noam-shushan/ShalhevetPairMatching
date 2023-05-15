using System;

namespace PairMatching.Models
{
    public record Note
    {
        public string Content { get; set; }

        public string Author { get; set; }

        public DateTime Date { get; set; }

        public string Subject { get; set; }
    }
}