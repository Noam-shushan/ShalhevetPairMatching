using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PairMatching.Models
{
    public class Participant
    {
        [BsonId]
        public string Id { get; set; }
    }
}
