namespace PairMatching.Models
{
    public record IsraelParticipant : Participant
    {

        public OpenQuestionsForIsrael OpenQuestions { get; set; } = new();

        /// <summary>
        ///  level of English
        /// </summary>
        public EnglishLevels EnglishLevel { get; set; }

        /// <summary>
        /// level of skill 
        /// </summary>
        public SkillLevels DesiredSkillLevel { get; set; }
    }
}
