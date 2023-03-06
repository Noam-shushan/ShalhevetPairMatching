using Newtonsoft.Json;
using PairMatching.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static PairMatching.Tools.Extensions;

namespace PairMatching.Models.Dtos
{
    public static class DtoExtentions
    {
        public static dynamic ToIsraelParticipantWixDto(this IsraelParticipant participant)
        {
            var name = participant.Name.Split();
            var result = new
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
            return result;
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
        
        public static Participant ToParticipant(this ParticipantWixDto wixDto)
        {
            var result = new Participant
            {
                Name = wixDto.fullName,
                WixId = wixDto.contactId,
                DateOfRegistered = wixDto._createdDate,
                Email = wixDto.email,
                WixIndex = wixDto.index,
                PhoneNumber = wixDto.tel,
                OtherLanguages = wixDto.otherLan,
                MoreLanguages = GetValueFromDescription<MoreLanguages>(wixDto.otherLang),
                Gender = GetValueFromDescription<Genders>(wixDto.gender),
                PairPreferences = new Preferences
                {
                    Gender = GetValueFromDescription<Genders>(wixDto.menOrWomen),
                    LearningStyle = GetValueFromDescription<LearningStyles>(wixDto.learningStyle),
                    NumberOfMatchs = int.Parse(wixDto.moreThanOneChevruta),
                    LearningTime = from day in Enum.GetValues(typeof(Days))
                                   .Cast<Days>()
                                   .Zip(new[] { wixDto.sunday, wixDto.monday, wixDto.tuseday, wixDto.wednesday, wixDto.thurseday, })
                                   where day.Second.Any()
                                   select new LearningTime
                                   {
                                       Day = day.First,
                                       TimeInDay = day.Second.Select(t => GetValueFromDescription<TimesInDay>(t))
                                   }
                }
            };
            if (result.OtherLanguages.Contains("Other") && !string.IsNullOrEmpty(wixDto.specifyLang))
            {
                result.OtherLanguages.Remove("Other");
                result.OtherLanguages.Add(wixDto.specifyLang);
            }
            return result;
        }

        public static IsraelParticipant ToIsraelParticipant(this IsraelParticipantWixDto wixDto)
        {
            var part = wixDto.ToParticipant()
                .CopyPropertiesToNew(typeof(IsraelParticipant)) as IsraelParticipant;
            
            part.OpenQuestions = new OpenQuestionsForIsrael
            {
                AdditionalInfo = wixDto.additionalInfo,
                BiographHeb = wixDto.biographHeb,
                PersonalTraits = wixDto.personalTraits,
                WhoIntroduced = wixDto.whoIntroduced,
                WhyJoinShalhevet = wixDto.whyJoinShalhevet
            };
            
            part.Country = "Israel";
            part.EnglishLevel = GetValueFromDescription<EnglishLevels>(wixDto.levOfEn);
            part.PairPreferences.Tracks = wixDto.preferredTrack.Select(t => GetValueFromDescription<PrefferdTracks>(t));
            part.DesiredSkillLevel = GetValueFromDescription<SkillLevels>(wixDto.chevrotaSkills);
            
            return part;
        }

        public static WorldParticipant ToWorldParticipant(this WorldParticipantWixDto wixDto)
        {
            var part = wixDto.ToParticipant()
                .CopyPropertiesToNew(typeof(WorldParticipant)) as WorldParticipant;
            
            part.Address = new Address
            {
                City = wixDto.city,
                State = wixDto.state,
            };
            
            part.OpenQuestions = new OpenQuestionsForWorld
            {
                AdditionalInfo = wixDto.additionalInfo,
                AnythingElse = wixDto.anythingElse,
                ConversionRabi = wixDto.conversionRabi,
                HopesExpectations = wixDto.hopesExpectations,
                WhoIntroduced = wixDto.whoIntroduced,
                RequestsFromPair = wixDto.requests,
                PersonalBackground = wixDto.background,
                Experience = wixDto.experience
            };
            if (part.OpenQuestions.HopesExpectations.Contains("Other")
                && !string.IsNullOrEmpty(wixDto.otherHopesAndExpectations))
            {
                part.OpenQuestions.HopesExpectations.Remove("Other");
                part.OpenQuestions.HopesExpectations.Add(wixDto.otherHopesAndExpectations);
            }

            if(int.TryParse(wixDto.age,out int age))
            {
                part.Age = age;
            }
            part.Profession = wixDto.profession;
            part.JewishAndComAff = wixDto.jewishAndComAff == "Other" ? wixDto.otherJewishAndComAff : wixDto.jewishAndComAff;
            part.DesiredEnglishLevel = GetValueFromDescription<EnglishLevels>(wixDto.levOfEn);
            part.SkillLevel = GetValueFromDescription<SkillLevels>(wixDto.learningSkill);
            part.Country = wixDto.utc;
            part.UtcOffset = TimeSpan.FromHours(wixDto.timeOffset);
            part.JewishAffiliation = wixDto.jewishAffiliation;
            part.PairPreferences.Tracks = wixDto.prefTra.Select(t => GetValueFromDescription<PrefferdTracks>(t));

            return part;
        }
    }
}
