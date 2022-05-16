using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using static PairMatching.Tools.HelperFunction;

namespace PairMatching.Models
{
    public class Student
    {
        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// is this student as deleted from the database
        /// </summary>
        public bool IsDeleted { get; set; }

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

        public bool IsSimpleStudent { get; set; } = false;

        /// <summary>
        /// Preferred tracks of learning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public IEnumerable<PrefferdTracks> PrefferdTracks { get; set; }

        public List<StudentMatchingHistory> MatchingHistories { get; set; }
            = new();

        /// <summary>
        /// the preferred gender to learn with
        /// </summary>
        public Genders PrefferdGender { get; set; }

        /// <summary>
        ///  Desired level of English from the other pair
        /// </summary>
        public EnglishLevels DesiredEnglishLevel { get; set; }

        /// <summary>
        ///  level of English
        /// </summary>
        public EnglishLevels EnglishLevel { get; set; }

        /// <summary>
        ///  Desired level of skill from the other pair
        /// </summary>
        public SkillLevels DesiredSkillLevel { get; set; }

        /// <summary>
        /// level of skill 
        /// </summary>
        public SkillLevels SkillLevel { get; set; }

        /// <summary>
        /// learning style 
        /// </summary>
        public LearningStyles LearningStyle { get; set; }


        /// <summary>
        /// the utc offset of the student
        /// </summary>
        public TimeSpan UtcOffset { get; set; }

        /// <summary>
        /// the id of the students that match to this.
        /// </summary>
        public List<int> MatchTo { get; set; } = new();

        /// <summary>
        /// Desired learning time and day
        /// </summary>
        public IEnumerable<LearningTime> DesiredLearningTime { get; set; }

        public IEnumerable<OpenQuestion> OpenQuestions { get; set; }

        public int PrefferdNumberOfMatchs { get; set; }

        // TODO remove this
        public string InfoAbout { get; set; } = "";

        public MoreLanguages MoreLanguages { get; set; }

        public bool IsInArchive { get; set; }

        public IEnumerable<string> Languages { get; set; }

        public List<Note> Notes { get; set; } = new();

        [BsonIgnore]
        public bool IsKnowMoreLanguages { get => Languages.Any(); }

        [BsonIgnore]
        public bool IsFromIsrael => Country == "Israel";

        [BsonIgnore]
        public IEnumerable<Student> FirstSuggestStudents { get; set; }

        [BsonIgnore]
        public IEnumerable<Student> SecondeSuggestStudents { get; set; }

        [BsonIgnore]
        public int DiffFromIsrael { get => GetDifferenceUtc(UtcOffset).Hours; }

        [BsonIgnore]
        public bool IsOpenToMatch
        {
            get => !IsSimpleStudent && (MatchTo.Count < PrefferdNumberOfMatchs);
        }
        public bool IsMatch { get; set; }

        [BsonIgnore]
        public IEnumerable<Student> MatchStudents { get; set; }
    }
}
