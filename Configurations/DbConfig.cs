﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Configurations
{
    public class DbConfig
    {
        [BsonId]
        public string Id { get; set; }

        public int WixIndex { get; set; }
    }
}
