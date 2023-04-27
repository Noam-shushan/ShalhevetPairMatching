using MongoDB.Bson.Serialization.Attributes;
using PairMatching.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PairMatching.Models
{
    public record Participant : BaseModel
    {
        public int WixIndex { get; set; }

        public string WixId { get; set; } = "";

        public string _WixId { get; set; } = "";


        // hop i dont need it
        public int OldId { get; set; }

        public Preferences PairPreferences { get; set; }

        [ExportProperty("חברותא")]
        public List<string> MatchTo { get; set; } = new List<string>();


        /// <summary>
        /// the name of the student
        /// </summary>
        [ExportProperty("שם")]
        public string Name { get; set; } = "";

        /// <summary>
        /// the country of the student
        /// </summary>
        [ExportProperty("ארץ")]
        public string Country { get; set; } = "";

        /// <summary>
        /// the email of the student
        /// </summary>
        [ExportProperty("אימייל")]
        public string Email { get; set; } = "";

        /// <summary>
        /// the phone number of the student
        /// </summary>
        [ExportProperty("טלפון")]
        public string PhoneNumber { get; set; } = "";

        /// <summary>
        /// the gender of the student
        /// </summary>
        [ExportProperty("מין")]
        public Genders Gender { get; set; }

        [ExportProperty("תאריך הרשמה")]
        public DateTime DateOfRegistered { get; set; }

        public bool IsInArchive { get; set; }

        public List<string> OtherLanguages { get; set; } = new();

        public MoreLanguages MoreLanguages { get; set; }

        [BsonIgnore]
        public bool IsKnowMoreLanguages { get => OtherLanguages.Any(); }

        [BsonIgnore]
        public bool IsFromIsrael => Country == "Israel";

        [BsonIgnore]
        public bool IsOpenToMatch => !IsDeleted 
            && !IsInArchive 
            && MatchTo.Count < PairPreferences?.NumberOfMatchs;

        [BsonIgnore]
        public bool IsMatch => MatchTo.Count > 0;
    }
}
