using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public class Pair : BaseModel
    {
        /// <summary>
        /// the first student id 
        /// </summary>
        public string ParticipantFromIsraelId { get; set; }

        /// <summary>
        /// The student id for the first student
        /// </summary>
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

        public bool Equals(Pair other)
        {
            if (other is null)
            {
                return false;
            } 
            return Id == other.Id;
        }
    }
}
