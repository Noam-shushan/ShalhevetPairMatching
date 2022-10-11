using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public record Pair : BaseModel
    {
        [BsonIgnore]
        public int OldId { get; set; }
        
        public string FromIsraelId { get; set; }

        public string FromWorldId { get; set; }

        public DateTime DateOfCreate { get; set; }

        public IEnumerable<TrackHistory> TrackHistories { get; set; }

        public DateTime DateOfDelete { get; set; }

        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Preferred tracks of learning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public PrefferdTracks Track { get; set; }

        public PairStatus Status { get; set; }

        [BsonIgnore]
        public Participant FromIsrael { get; set; }

        [BsonIgnore]
        public Participant FromWorld { get; set; }
    }

    public class TrackHistory
    {
        public PrefferdTracks Track { get; set; }

        public DateTime DateOfUpdate { get; set; }
    }
}
