using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public class Pair
    {
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// flag that determine if the pair is deleted from the database 
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// the first student id 
        /// </summary>
        public int StudentFromIsraelId { get; set; }

        /// <summary>
        /// The student id for the first student
        /// </summary>
        public int StudentFromWorldId { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime DateOfCreate { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime DateOfUpdate { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime DateOfDelete { get; set; }

        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Preferred tracks of learning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public PrefferdTracks PreferredTrack { get; set; }

        public List<Note> Notes { get; set; } = new List<Note>();
    }
}
