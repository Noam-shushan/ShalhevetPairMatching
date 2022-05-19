using System.Collections.Generic;

namespace PairMatching.Models
{
    public class Preferences
    {
        /// <summary>
        /// Preferred tracks of learning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public IEnumerable<PrefferdTracks> Tracks { get; set; }

        /// <summary>
        /// the preferred gender to learn with
        /// </summary>
        public Genders Gender { get; set; }

        /// <summary>
        ///  level of English
        /// </summary>
        public EnglishLevels EnglishLevel { get; set; }

        /// <summary>
        /// level of skill 
        /// </summary>
        public SkillLevels SkillLevel { get; set; }

        /// <summary>
        /// learning style 
        /// </summary>
        public LearningStyles LearningStyle { get; set; }

        /// <summary>
        /// Desired learning time and day
        /// </summary>
        public IEnumerable<LearningTime> LearningTime { get; set; }

        public int NumberOfMatchs { get; set; }
    }
}
