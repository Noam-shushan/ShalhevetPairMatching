using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;
using JsonSubTypes;
using Newtonsoft.Json;
using PairMatching.Tools;
using static PairMatching.Tools.Extensions;

namespace PairMatching.Models.Dtos
{
    [JsonConverter(typeof(JsonSubtypes))]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(IsraelParticipantWixDto), "chevrotaSkills")]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(WorldParticipantWixDto), "city")]
    public class ParticipantWixDto
    {
        public List<string> sunday { get; set; }
        public List<string> thurseday { get; set; }
        public List<string> tuseday { get; set; }
        public string fullName { get; set; }
        public List<string> wednesday { get; set; }
        public List<string> otherLan { get; set; }
        public string otherLang { get; set; }
        public string _id { get; set; }
        public string _owner { get; set; }
        public DateTime _createdDate { get; set; }
        
        /// <summary>
        /// [Advanced, Moderate, Beginner]
        /// [גבוהה, בינונית, לא מכיר/ה. מאוד מתעיינ/ת, אין לי מסלול מועדף]
        /// </summary>
        public string learningSkill { get; set; }
        public string whoIntroduced { get; set; }
        public string email { get; set; }
        public int index { get; set; }
        public string gender { get; set; }
        public string additionalInfo { get; set; }
        public List<string> monday { get; set; }
        public string learningStyle { get; set; }
        public string menOrWomen { get; set; }
        public DateTime _updatedDate { get; set; }
        public string levOfEn { get; set; }
        public string specifyLang { get; set; }
        public string tel { get; set; }
        public string moreThanOneChevruta { get; set; }
        public string contactId { get; set; }

        //public virtual Participant ToParticipant()
        //{
        //    var result = new Participant
        //    {
        //        Name = fullName,
        //        WixId = contactId,
        //        DateOfRegistered = _createdDate,
        //        Email = email,
        //        WixIndex = index,
        //        PhoneNumber = tel,
        //        OtherLanguages = otherLan,
        //        MoreLanguages = GetValueFromDescription<MoreLanguages>(otherLang),
        //        Gender = GetValueFromDescription<Genders>(gender),
        //        PairPreferences = new Preferences
        //        {
        //            Gender = GetValueFromDescription<Genders>(menOrWomen),
        //            LearningStyle = GetValueFromDescription<LearningStyles>(learningStyle),
        //            NumberOfMatchs = int.Parse(moreThanOneChevruta),
        //            LearningTime = from day in Enum.GetValues(typeof(Days))
        //                           .Cast<Days>()
        //                           .Zip(new[] { sunday, monday, tuseday, wednesday, thurseday, })
        //                           where day.Second.Any()
        //                           select new LearningTime
        //                           {
        //                               Day = day.First,
        //                               TimeInDay = day.Second.Select(t => GetValueFromDescription<TimesInDay>(t))
        //                           }
        //        }
        //    };
        //    if(result.OtherLanguages.Contains("Other") && !string.IsNullOrEmpty(specifyLang))
        //    {
        //        result.OtherLanguages.Remove("Other");
        //        result.OtherLanguages.Add(specifyLang);
        //    }
        //    return result; 
        //}
    }

    public class IsraelParticipantWixDto : ParticipantWixDto
    {
        public string personalTraits { get; set; }
        public string biographHeb { get; set; }
        public string whyJoinShalhevet { get; set; }
        public List<string> preferredTrack { get; set; }
        public string chevrotaSkills { get; set; }

        //public override Participant ToParticipant()
        //{
        //    var part = base.ToParticipant()
        //        .CopyPropertiesToNew(typeof(IsraelParticipant)) as IsraelParticipant;
        //    part.OpenQuestions = new OpenQuestionsForIsrael
        //    {
        //        AdditionalInfo = additionalInfo,
        //        BiographHeb = biographHeb,
        //        PersonalTraits = personalTraits,
        //        WhoIntroduced = whoIntroduced,
        //        WhyJoinShalhevet = whyJoinShalhevet
        //    };
        //    part.Country = "Israel";
        //    part.EnglishLevel = GetValueFromDescription<EnglishLevels>(levOfEn);
        //    part.PairPreferences.Tracks = preferredTrack.Select(t => GetValueFromDescription<PrefferdTracks>(t));
        //    part.DesiredSkillLevel = GetValueFromDescription<SkillLevels>(chevrotaSkills);
        //    return part;
        //}
    }

    public class WorldParticipantWixDto : ParticipantWixDto
    {
        public string city { get; set; }     
        public string anythingElse { get; set; }
        public List<string> hopesExpectations { get; set; }
        public string state { get; set; }    
        public string background { get; set; }
        public string jewishAndComAff { get; set; }        
        public string otherHopesAndExpectations { get; set; }

        // [Jewish, In a conversion proccess] 
        public string jewishAffiliation { get; set; }
        public int age { get; set; }      
        public string utc { get; set; }
        public string conversionRabi { get; set; }
        public string experience { get; set; }
        public string otherJewishAndComAff { get; set; }
        public string profession { get; set; }
        public List<string> prefTra { get; set; }
        public string requests { get; set; }

        public int timeOffset { get; set; }

        //public override Participant ToParticipant()
        //{
        //    var part =  base.ToParticipant()
        //        .CopyPropertiesToNew(typeof(WorldParticipant)) as WorldParticipant;
        //    part.Address = new Address
        //    {
        //        City = city,
        //        State = state,
        //    };
        //    part.Age = age;
        //    part.OpenQuestions = new OpenQuestionsForWorld
        //    {
        //        AdditionalInfo = additionalInfo,
        //        AnythingElse = anythingElse,
        //        ConversionRabi = conversionRabi,
        //        HopesExpectations = hopesExpectations,
        //        WhoIntroduced = whoIntroduced,
        //        RequestsFromPair = requests,
        //        PersonalBackground = background,
        //        Experience = experience
        //    };
        //    if (part.OpenQuestions.HopesExpectations.Contains("Other") 
        //        && !string.IsNullOrEmpty(otherHopesAndExpectations))
        //    {
        //        part.OpenQuestions.HopesExpectations.Remove("Other");
        //        part.OpenQuestions.HopesExpectations.Add(otherHopesAndExpectations);
        //    }
        //    part.Profession = profession;
        //    part.JewishAndComAff = jewishAndComAff == "Other" ? otherJewishAndComAff : jewishAndComAff;
        //    part.DesiredEnglishLevel = GetValueFromDescription<EnglishLevels>(levOfEn);
        //    part.SkillLevel = GetValueFromDescription<SkillLevels>(learningSkill);
        //    part.Country = utc;
        //    part.UtcOffset = TimeSpan.FromHours(timeOffset);
        //    part.JewishAffiliation = jewishAffiliation;
        //    part.PairPreferences.Tracks = prefTra.Select(t => GetValueFromDescription<PrefferdTracks>(t));

        //    return part;
        //}
    }
}
