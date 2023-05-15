using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PairMatching.Loggin
{
    public record Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string Message { get; set; } = "";

        public DateTime Date { get; set; }

        public string Source { get; set; } = "";
        
        public string Type { get; set; } = "";

        // database misstake
        public MyException Exception { get; set; }

    }
    
    public class MyException
    {
        public string StackTrace { get; set; }
    }



    public record ErrorLog : Log
    {
        public string ExceptionSource { get; set; }

        public string StackTreac { get; set; }

        public bool IsChecked { get; set; }
    }
}