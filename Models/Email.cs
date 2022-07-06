using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public class Email
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public IEnumerable<EmailAddress> SendTo { get; set; }

        public IEnumerable<EmailAddress> To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTime SendingDate { get; set; }
    }
}
