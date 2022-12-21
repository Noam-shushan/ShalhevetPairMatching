using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PairMatching.Loggin
{
    public record Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Message { get; set; } = "";

        public DateTime Date { get; set; }

        public string Source { get; set; } = "";

        public string Type { get; set; } = "";

        public ExceptionDto Exception { get; set; }

    }
}