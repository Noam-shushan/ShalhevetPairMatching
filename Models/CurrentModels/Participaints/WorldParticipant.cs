﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using static PairMatching.Tools.HelperFunction;

namespace PairMatching.Models
{
    public record WorldParticipant : Participant
    {
        public OpenQuestionsForWorld OpenQuestions { get; set; } = new();

        /// <summary>
        /// the utc offset of the student
        /// </summary>
        public TimeSpan UtcOffset { get; set; }

        [BsonIgnore]
        public TimeSpan DiffFromIsrael { get => GetDifferenceUtc(UtcOffset); }

        /// <summary>
        /// level of skill 
        /// </summary>
        public SkillLevels SkillLevel { get; set; }

        /// <summary>
        /// Desired level of English
        /// </summary>
        public EnglishLevels DesiredEnglishLevel { get; set; }

        public string JewishAndComAff { get; set; } = "";

        /// <summary>
        /// Can by Jewish or in a conversion proccess
        /// </summary>
        public string JewishAffiliation { get; set; } = "";

        public string Profession { get; set; } = "";

        public int Age { get; set; }

        public int RealAge { get => DateTime.Now.Year - Age; }

        public Address Address { get; set; } = new();

        public string FullCountryName { get; set; } = "";
    }

    public class Address
    {
        public string City { get; set; } = "";
        public string State { get; set; } = "";

    }
}
