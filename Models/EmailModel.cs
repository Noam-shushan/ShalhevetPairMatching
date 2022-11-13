using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
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

        public List<string> SendTo { get; set; } = new();

        public List<EmailAddress> To { get; set; } = new();

        public string Subject { get; set; }

        public string Body { get; set; }
        
        public string HtmlBody { get; set; }

        public bool HasHtmlBody { get; set; }

        public string Language { get; set; }

        public string Link { get; set; }

        public DateTime SendingDate { get; set; }

        public bool IsVerified { get; set; }

        public DateTime Date { get; set; }

        [JsonIgnore]
        [BsonIgnore]
        public IEnumerable<EmailAddress> MissSentAddress 
        {
            get => To.Where(e => !SendTo.Contains(e.ParticipantWixId));
        }
        
        [JsonIgnore]
        [BsonIgnore]
        public bool IsSentForAll { get => !MissSentAddress.Any(); }
    }
}
