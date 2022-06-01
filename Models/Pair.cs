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
        public string ParticipantFromIsraelId { get; set; }

        public string ParticipantFromWorldId { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime DateOfUpdate { get; set; }

        public DateTime DateOfDelete { get; set; }

        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Preferred tracks of learning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public PrefferdTracks Track { get; set; }

        [BsonIgnore]
        public Participant ParticipantFromIsrael { get; set; }

        [BsonIgnore]
        public Participant ParticipantFromWorld { get; set; }
    }
}
