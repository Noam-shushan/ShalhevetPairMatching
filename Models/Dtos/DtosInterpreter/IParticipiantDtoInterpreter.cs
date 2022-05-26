using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;

namespace PairMatching.Models.Dtos.DtosInterpreter
{
    public interface IParticipiantDtoInterpreter
    {
        Participant GetResult();
    }

    public class IsraelParticipiantDtoInterpreter : IParticipiantDtoInterpreter
    {
        public Participant GetResult()
        {
            throw new NotImplementedException();
        }
    }

    public static class EngHebDictionaries
    {
        public static Dictionary<string, string> OpenQuestionsHeader = new Dictionary<string, string>
        {
            ["Personal information"] = "Personal information (age, job, Jewish & community affiliation...)",
            ["Personality trates"] = "Personality traits, personal values, interests, hobbies",
            ["What are your hopes and expectations from this program"] = "What are your hopes and expectations from this program?",
            ["Additional information"] = "Additional information you would like us to know about you, or requests you have?",
            ["Who introduced you to this program"] = "Who introduced you to this program? Where did you hear about Shalhevet?",
            ["Country and City of residence"] = "Country & City of residence",
            ["Anything else you would like to tell us"] = "Anything else you would like to tell us?"
        };

        public static Dictionary<EnglishLevels, string> EnglishLevelsDict = new()
        {
            {EnglishLevels.Good, "Excellent (I don't know any Hebrew whatsoever)" },
            {EnglishLevels.ConversationalLevel, "Conversational level" },
            {EnglishLevels.NotGood, "Doesn't have to be perfect.\nI know some Hebrew" },
            {EnglishLevels.Defulte, "לא משנה" }
        };

        public static Dictionary<string, EnglishLevels> EnglishLevelsDictInverse =
            EnglishLevelsDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<LearningStyles, string> LearningStylesDict = new()
        {
            { LearningStyles.DeepAndSlow, "לימוד איטי ומעמיק" },
            { LearningStyles.Free, "לימוד מעודד מחשבה מחוץ לטקסט, פילוסופי" },
            { LearningStyles.ProgressedFlowing, "לימוד מהיר, הספקי ומתקדם" },
            { LearningStyles.TextCentered, "לימוד צמוד טקסט" },
            { LearningStyles.DontMatter, "לא משנה לי" }
        };

        public static Dictionary<string, LearningStyles> LearningStylesDictInverse =
            LearningStylesDict.ToDictionary((i) => i.Value, (i) => i.Key);



        public static Dictionary<SkillLevels, string> SkillLevelsDict = new()
        {
            { SkillLevels.Advanced, "מתקדם" },
            { SkillLevels.Moderate, "בינוני" },
            { SkillLevels.Beginner, "מתחיל" },
            { SkillLevels.DontMatter, "לא משנה" },
        };

        public static Dictionary<string, SkillLevels> SkillLevelsDictInverse =
    SkillLevelsDict.ToDictionary((i) => i.Value, (i) => i.Key);




        public static Dictionary<PrefferdTracks, string> PrefferdTracksDict = new()
        {
            [PrefferdTracks.Tanya] = "תניא",
            [PrefferdTracks.Talmud] = "גמרא",
            [PrefferdTracks.Parsha] = "פרשה",
            [PrefferdTracks.Payer] = "תפילה",
            [PrefferdTracks.PirkeiAvot] = "פרקי אבות",
            [PrefferdTracks.DONT_MATTER] = "לא משנה לי",
            [PrefferdTracks.IndependentLearning] = "Independent learning"
        };

        public static Dictionary<string, PrefferdTracks> PrefferdTracksDictInverse =
            PrefferdTracksDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<Days, string> DaysDict = new()
        {
            [Days.Sunday] = "ראשון",
            [Days.Monday] = "שני",
            [Days.Tuesday] = "שלישי",
            [Days.Wednesday] = "רביעי",
            [Days.Thursday] = "חמישי"
        };

        public static Dictionary<TimesInDay, string> TimesInDayDict = new()
        {
            [TimesInDay.Morning] = "בוקר",
            [TimesInDay.Noon] = "צהריים",
            [TimesInDay.Evening] = "ערב",
            [TimesInDay.Night] = "לילה",
            [TimesInDay.INCAPABLE] = "אין לי זמן ביום זה"
        };

        public static Dictionary<string, TimesInDay> TimesInDayDictInverse =
            TimesInDayDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<Genders, string> GendersDict = new()
        {
            [Genders.Male] = "גבר",
            [Genders.Female] = "אישה",
            [Genders.NoPrefrence] = "לא משנה לי"
        };

        public static Dictionary<string, Genders> GendersDictInverse =
             GendersDict.ToDictionary((i) => i.Value, (i) => i.Key);

    }


    public static class HebDictionaries
    {
        public static Dictionary<string, string> OpenQuestionsHeader = new ()
        {
            ["Personal information"] = "פרטים ביוגרפיים (גיל, מה עושה בחיים, רקע לימודי, השתייכות חברתית)",
            ["Personality trates"] = "תכונות אישיות, ערכים חשובים בשבילך, תחביבים ותחומי עניין",
            ["What are your hopes and expectations from this program"] = "מה מעניין אותך בהצטרפות לשלהבת?",
            ["Additional information"] = "דברים נוספים שהיית רוצה שנדע, או דברים שהיית רוצה לבקש מאיתנו?",
            ["Who introduced you to this program"] = "דרך מי (או דרך מה) הגעת לתכנית?"
        };

        public static Dictionary<LearningStyles, string> LearningStylesDict = new()
        {
            {LearningStyles.DeepAndSlow, "לימוד איטי ומעמיק" },
            {LearningStyles.Free, "לימוד מעודד מחשבה\n מחוץ לטקסט, פילוסופי" },
            {LearningStyles.ProgressedFlowing, "לימוד מהיר, הספקי ומתקדם" },
            {LearningStyles.TextCentered, "לימוד צמוד טקסט" },
            {LearningStyles.DontMatter, "לא משנה לי" }
        };

        public static Dictionary<string, LearningStyles> LearningStylesDictInverse =
            LearningStylesDict.ToDictionary((i) => i.Value, (i) => i.Key);



        public static Dictionary<SkillLevels, string> SkillLevelsDict = new()
        {
            {SkillLevels.Advanced, "מתקדם" },
            {SkillLevels.Moderate, "בינוני" },
            {SkillLevels.Beginner, "מתחיל" },
            {SkillLevels.DontMatter, "לא משנה" },
        };

        public static Dictionary<string, SkillLevels> SkillLevelsDictInverse =
    SkillLevelsDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<EnglishLevels, string> EnglishLevelsDict = new()
        {
            {EnglishLevels.Good, "טובה" },
            {EnglishLevels.ConversationalLevel, "רמת שיחה (בינונית)" },
            {EnglishLevels.NotGood, "לא כל כך טובה" },
            {EnglishLevels.Defulte, "לא משנה" }
        };

        public static Dictionary<string, EnglishLevels> EnglishLevelsDictInverse =
   EnglishLevelsDict.ToDictionary((i) => i.Value, (i) => i.Key);




        public static Dictionary<PrefferdTracks, string> PrefferdTracksDict = new()
            {
                [PrefferdTracks.Tanya] = "תניא",
                [PrefferdTracks.Talmud] = "גמרא",
                [PrefferdTracks.Parsha] = "פרשה",
                [PrefferdTracks.Payer] = "תפילה",
                [PrefferdTracks.PirkeiAvot] = "פרקי אבות",
                [PrefferdTracks.DONT_MATTER] = "לא משנה לי",
                [PrefferdTracks.IndependentLearning] = "Independent learning"
            };

        public static Dictionary<string, PrefferdTracks> PrefferdTracksDictInverse =
            PrefferdTracksDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<Days, string> DaysDict = new()
        {
            [Days.Sunday] = "ראשון",
            [Days.Monday] = "שני",
            [Days.Tuesday] = "שלישי",
            [Days.Wednesday] = "רביעי",
            [Days.Thursday] = "חמישי"
        };

        public static Dictionary<TimesInDay, string> TimesInDayDict = new()
            {
                [TimesInDay.Morning] = "בוקר",
                [TimesInDay.Noon] = "צהריים",
                [TimesInDay.Evening] = "ערב",
                [TimesInDay.Night] = "לילה",
                [TimesInDay.INCAPABLE] = "אין לי זמן ביום זה"
            };

        public static Dictionary<string, TimesInDay> TimesInDayDictInverse =
            TimesInDayDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<Genders, string> GendersDict = new()
            {
                [Genders.Male] = "גבר",
                [Genders.Female] = "אישה",
                [Genders.NoPrefrence] = "לא משנה לי"
            };

        public static Dictionary<string, Genders> GendersDictInverse =
             GendersDict.ToDictionary((i) => i.Value, (i) => i.Key);
    }
}
