using PairMatching.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models.Dtos
{
    public static class DtoExtentions
    {
        public static dynamic ToIsraelParticipantWixDto(this IsraelParticipant participant)
        {
            var name = participant.Name.Split();
            return new
            {
                additionalInfo = participant.OpenQuestions.AdditionalInfo,
                biographHeb = participant.OpenQuestions.BiographHeb,
                chevrotaSkills = participant.DesiredSkillLevel.GetDescriptionFromEnumValue(),
                email = participant.Email,
                gender = participant.Gender.GetDescriptionFromEnumValue(),
                levOfEn = participant.EnglishLevel.GetDescriptionFromEnumValue(),
                menOrWomen = participant.PairPreferences.Gender.GetDescriptionFromEnumValue(),
                preferredTrack = participant.PairPreferences.Tracks.Select(p => p.GetDescriptionFromEnumValue()).ToList(),
                learningStyle = participant.PairPreferences.LearningStyle.GetDescriptionFromEnumValue(),
                otherLan = participant.OtherLanguages,
                tel = participant.PhoneNumber,
                whoIntroduced = participant.OpenQuestions.WhoIntroduced,
                moreThanOneChevruta = participant.PairPreferences.NumberOfMatchs.ToString(),
                personalTraits = participant.OpenQuestions.PersonalTraits,
                whyJoinShalhevet = participant.OpenQuestions.WhyJoinShalhevet,
                
                sunday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Sunday)
                .GetLearningTimeAsStringList(),
                
                monday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Monday)
                .GetLearningTimeAsStringList(),
                
                thurseday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Thursday)
                .GetLearningTimeAsStringList(),
                
                tuseday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Tuesday).
                GetLearningTimeAsStringList(),
                
                wednesday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Wednesday)
                .GetLearningTimeAsStringList(),
                
                formLanguage = "Hebrew",
                fName = name.First(),
                lName = string.Join(" ", name.Skip(1))
            };
        }
        
        public static dynamic ToWorldParticipantWixDto(this WorldParticipant participant)
        {
            var name = participant.Name.Split();
            return new
            {
                additionalInfo = participant.OpenQuestions.AdditionalInfo,
                anythingElse = participant.OpenQuestions.AnythingElse,
                hopesExpectations = participant.OpenQuestions.HopesExpectations,
                learningSkill = participant.SkillLevel.GetDescriptionFromEnumValue("eng"),
                prefTra = participant.PairPreferences.Tracks.Select(t => t.GetDescriptionFromEnumValue("eng")).ToList(),
                fName = name.First(),
                lName = string.Join(" ", name.Skip(1)),
                email = participant.Email,
                gender = participant.Gender.GetDescriptionFromEnumValue("eng"),
                menOrWomen = participant.PairPreferences.Gender.GetDescriptionFromEnumValue("eng"),
                levOfEn = participant.DesiredEnglishLevel.GetDescriptionFromEnumValue("eng"),
                utc = participant.UtcOffset.ToString(),
                learningStyle = participant.PairPreferences.LearningStyle.GetDescriptionFromEnumValue("eng"),
                otherLan = participant.OtherLanguages,
                tel = participant.PhoneNumber,
                whoIntroduced = participant.OpenQuestions.WhoIntroduced,
                moreThanOneChevruta = participant.PairPreferences.NumberOfMatchs.ToString(),
                
                sunday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Sunday)
                .GetLearningTimeAsStringList(engOrHeb: "eng"),

                monday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Monday)
                .GetLearningTimeAsStringList(engOrHeb: "eng"),

                thurseday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Thursday)
                .GetLearningTimeAsStringList(engOrHeb: "eng"),

                tuseday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Tuesday).
                GetLearningTimeAsStringList(engOrHeb: "eng"),

                wednesday = participant
                .PairPreferences
                .LearningTime
                .Where(l => l.Day == Days.Wednesday)
                .GetLearningTimeAsStringList(engOrHeb:"eng"),
                
                formLanguage = "English",
            };
        }

        public static List<string> GetLearningTimeAsStringList(this IEnumerable<LearningTime> learningTimes, string engOrHeb = "heb")
        {
            return (from l in learningTimes
                    from t in l.TimeInDay
                    select t.GetDescriptionFromEnumValue(engOrHeb))
                   .ToList();
        }
    }
}
