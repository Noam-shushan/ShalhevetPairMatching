using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public enum Genders { MALE, FMALE, DONT_MATTER }
    public enum Days { SUNDAY, MONDAY, TUESDAY, WEDNESDAY, THURSDAY, DONT_MATTER }
    public enum TimesInDay { MORNING, NOON, EVENING, NIGHT, DONT_MATTER, INCAPABLE }
    public enum PrefferdTracks { TANYA, TALMUD, PARASHA, PRAYER, PIRKEY_AVOT, DONT_MATTER, IndependentLearning }
    public enum EnglishLevels { NOT_GOOD, TALK_LEVEL, GOOD, DONT_MATTER }
    public enum SkillLevels { BEGGINER, MODERATE, ADVANCED, DONT_MATTER }
    public enum LearningStyles { DEEP_AND_SLOW, PROGRESSED_FLOWING, TEXTUALL_CENTERED, FREE, DONT_MATTER }
    public enum MoreLanguages { YES, NO, NOT_ENGLISH }

    public static class Dictionaries
    {
        public static Dictionary<string, string> OpenQuestionsHeaderInHebrow = new Dictionary<string, string>
        {
            ["Personal information"] = "פרטים ביוגרפיים (גיל, מה עושה בחיים, רקע לימודי, השתייכות חברתית)",
            ["Personality trates"] = "תכונות אישיות, ערכים חשובים בשבילך, תחביבים ותחומי עניין",
            ["What are your hopes and expectations from this program"] = "מה מעניין אותך בהצטרפות לשלהבת?",
            ["Additional information"] = "דברים נוספים שהיית רוצה שנדע, או דברים שהיית רוצה לבקש מאיתנו?",
            ["Who introduced you to this program"] = "דרך מי (או דרך מה) הגעת לתכנית?"
        };

        public static Dictionary<string, string> OpenQuestionsHeaderInEnglish = new Dictionary<string, string>
        {
            ["Personal information"] = "Personal information (age, job, Jewish & community affiliation...)",
            ["Personality trates"] = "Personality traits, personal values, interests, hobbies",
            ["What are your hopes and expectations from this program"] = "What are your hopes and expectations from this program?",
            ["Additional information"] = "Additional information you would like us to know about you, or requests you have?",
            ["Who introduced you to this program"] = "Who introduced you to this program? Where did you hear about Shalhevet?",
            ["Country and City of residence"] = "Country & City of residence",
            ["Anything else you would like to tell us"] = "Anything else you would like to tell us?"
        };

        public static Dictionary<LearningStyles, string> LearningStylesDict = new
            Dictionary<LearningStyles, string>()
        {
            {LearningStyles.DEEP_AND_SLOW, "לימוד איטי ומעמיק" },
            {LearningStyles.FREE, "לימוד מעודד מחשבה\n מחוץ לטקסט, פילוסופי" },
            {LearningStyles.PROGRESSED_FLOWING, "לימוד מהיר, הספקי ומתקדם" },
            {LearningStyles.TEXTUALL_CENTERED, "לימוד צמוד טקסט" },
            {LearningStyles.DONT_MATTER, "לא משנה לי" }
        };

        public static Dictionary<string, LearningStyles> LearningStylesDictInverse =
            LearningStylesDict.ToDictionary((i) => i.Value, (i) => i.Key);



        public static Dictionary<SkillLevels, string> SkillLevelsDict = new
            Dictionary<SkillLevels, string>()
        {
            {SkillLevels.ADVANCED, "מתקדם" },
            {SkillLevels.MODERATE, "בינוני" },
            {SkillLevels.BEGGINER, "מתחיל" },
            {SkillLevels.DONT_MATTER, "לא משנה" },
        };

        public static Dictionary<string, SkillLevels> SkillLevelsDictInverse =
    SkillLevelsDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<EnglishLevels, string> EnglishLevelsDict = new
            Dictionary<EnglishLevels, string>()
        {
            {EnglishLevels.GOOD, "טובה" },
            {EnglishLevels.TALK_LEVEL, "רמת שיחה (בינונית)" },
            {EnglishLevels.NOT_GOOD, "לא כל כך טובה" },
            {EnglishLevels.DONT_MATTER, "לא משנה" }
        };

        public static Dictionary<string, EnglishLevels> EnglishLevelsDictInverse =
   EnglishLevelsDict.ToDictionary((i) => i.Value, (i) => i.Key);


        public static Dictionary<EnglishLevels, string> DesiredEnglishLevelsDict = new
            Dictionary<EnglishLevels, string>()
        {
            {EnglishLevels.GOOD, "Excellent\n(I don't know any Hebrew whatsoever)" },
            {EnglishLevels.TALK_LEVEL, "Conversational level" },
            {EnglishLevels.NOT_GOOD, "Doesn't have to be perfect.\nI know some Hebrew" },
            {EnglishLevels.DONT_MATTER, "לא משנה" }
        };

        public static Dictionary<PrefferdTracks, string> PrefferdTracksDict =
            new Dictionary<PrefferdTracks, string>()
            {
                [PrefferdTracks.TANYA] = "תניא",
                [PrefferdTracks.TALMUD] = "גמרא",
                [PrefferdTracks.PARASHA] = "פרשה",
                [PrefferdTracks.PRAYER] = "תפילה",
                [PrefferdTracks.PIRKEY_AVOT] = "פרקי אבות",
                [PrefferdTracks.DONT_MATTER] = "לא משנה לי",
                [PrefferdTracks.IndependentLearning] = "Independent learning"
            };

        public static Dictionary<string, PrefferdTracks> PrefferdTracksDictInverse =
            PrefferdTracksDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<Days, string> DaysDict = new Dictionary<Days, string>()
        {
            [Days.SUNDAY] = "ראשון",
            [Days.MONDAY] = "שני",
            [Days.TUESDAY] = "שלישי",
            [Days.WEDNESDAY] = "רביעי",
            [Days.THURSDAY] = "חמישי"
        };

        public static Dictionary<TimesInDay, string> TimesInDayDict =
            new Dictionary<TimesInDay, string>()
        {
            [TimesInDay.MORNING] = "בוקר",
            [TimesInDay.NOON] = "צהריים",
            [TimesInDay.EVENING] = "ערב",
            [TimesInDay.NIGHT] = "לילה",
            [TimesInDay.INCAPABLE] = "אין לי זמן ביום זה" 
        };

        public static Dictionary<string, TimesInDay> TimesInDayDictInverse =
            TimesInDayDict.ToDictionary((i) => i.Value, (i) => i.Key);

        public static Dictionary<Genders, string> GendersDict =
            new Dictionary<Genders, string>()
            {
                [Genders.MALE] = "גבר",
                [Genders.FMALE] = "אישה" ,
                [Genders.DONT_MATTER] = "לא משנה לי" 
            };

        public static Dictionary<string, Genders> GendersDictInverse =
             GendersDict.ToDictionary((i) => i.Value, (i) => i.Key);

        
    }
}
