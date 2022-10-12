using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public class EmailModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string WixId { get; set; }

        public IEnumerable<string> SendTo { get; set; }

        public IEnumerable<EmailAddress> To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
        
        public string HtmlBody { get; set; }

        public bool HasHtmlBody { get; set; }

        public string Language { get; set; }

        public IEnumerable<string> Links { get; set; }

        public DateTime SendingDate { get; set; }

        public bool IsVerified { get; set; }

        [BsonIgnore]
        public IEnumerable<EmailAddress> MissSentAddress 
        {
            get => To.Where(e => !SendTo.Contains(e.ParticipantWixId));
        }

        [BsonIgnore]
        public bool IsSentForAll { get => !MissSentAddress.Any(); }
    }
}
