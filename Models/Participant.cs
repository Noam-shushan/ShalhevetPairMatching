using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PairMatching.Tools.HelperFunction;

namespace PairMatching.Models
{
    public class Participant : BaseModel
    {
        public int WixIndex { get; set; }

        public string WixId { get; set; }

        // hop i dont need it
        public int OldId { get; set; }

        public Preferences PairPreferences { get; set; }

        public OpenQuestions OpenQuestions { get; set; }

        public IEnumerable<string> MatchTo { get; set; }

        /// <summary>
        /// the name of the student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the country of the student
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// the email of the student
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// the phone number of the student
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the gender of the student
        /// </summary>
        public Genders Gender { get; set; }

        public DateTime DateOfRegistered { get; set; }


        /// <summary>
        /// the utc offset of the student
        /// </summary>
        public TimeSpan UtcOffset { get; set; }

        public bool IsInArchive { get; set; }

        public IEnumerable<string> Languages { get; set; }

        [BsonIgnore]
        public bool IsKnowMoreLanguages { get => Languages.Any(); }

        [BsonIgnore]
        public bool IsFromIsrael => Country == "Israel";

        [BsonIgnore]
        public int DiffFromIsrael { get => GetDifferenceUtc(UtcOffset).Hours; }

        /// <summary>
        ///  level of English
        /// </summary>
        public EnglishLevels EnglishLevel { get; set; }

        /// <summary>
        /// level of skill 
        /// </summary>
        public SkillLevels SkillLevel { get; set; }

    }
}
