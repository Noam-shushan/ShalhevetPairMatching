﻿using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace PairMatching.Models
{
    public class MatchingHistory
    {
        [BsonId]
        public string Id { get; set; }

        public string FromIsraelId { get; set; }

        public string FromWorldId { get; set; }

        public IEnumerable<MatchingHistoryDetails> History { get; set; }

        public bool IsUnMatch { get; set; }

        public bool IsActive { get; set; }
    }
}