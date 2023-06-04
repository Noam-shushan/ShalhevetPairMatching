using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public record ParticipantInPair
    {
        public string Id { get; set; } = "";

        public string WixId { get; set; } = "";

        public string _WixId { get; set; } = "";

        public DateTime DateOfRegistered { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Country { get; set; } = "";

        public List<string> MatchTo { get; set; } = new List<string>();

        public bool IsResivePairingEmail { get; set; }

        [BsonIgnore]
        public bool IsFromIsrael => Country == "Israel";
    }
}
