using PairMatching.Configuration;
using PairMatching.Models;
using System;
using System.Collections.Generic;


namespace PairMatching.DomainModel.GoogleSheet
{
    public class HebrewDescriptor : IStudentDescriptor
    {
        /// <summary>
        /// The spreadsheet id
        /// </summary>
        public string SpreadsheetId { get; init; }

        /// <summary>
        /// The spreadsheet range of rows and columns 
        /// </summary>
        public string Range { get; init; }

        /// <summary>
        /// The spreadsheet name
        /// </summary>
        public string SheetName { get => "טופס רישום שלהבת תשפ\"א (תגובות)"; }


        public HebrewDescriptor(SpredsheetLastRange lastRange, MyConfiguration config)
        {
            Range = lastRange.HebrewSheets;
            SpreadsheetId = config.SpreadsheetsId["Hebrew"];
        }

        public EnglishLevels GetEnglishLevel(string value)
        {
            return value switch
            {
                "טובה" => EnglishLevels.GOOD,
                "לא כל כך טובה" => EnglishLevels.NOT_GOOD,
                "רמת שיחה" => EnglishLevels.TALK_LEVEL,
                _ => EnglishLevels.DONT_MATTER,
            };
        }

        public Genders GetGender(string value)
        {
            return value switch
            {
                "גבר" => Genders.MALE,
                "אישה" => Genders.FMALE,
                "לא משנה" => Genders.DONT_MATTER,
                _ => Genders.DONT_MATTER,
            };
        }

        public LearningStyles GetLearningStyle(string value)
        {
            return value switch
            {
                "לימוד איטי ומעמיק" => LearningStyles.DEEP_AND_SLOW,
                "לימוד מהיר, הספקי ומתקדם" => LearningStyles.PROGRESSED_FLOWING,
                "לימוד צמוד טקסט" => LearningStyles.TEXTUALL_CENTERED,
                "לימוד מעודד מחשבה מחוץ לטקסט, פילוסופי" => LearningStyles.FREE,
                _ => LearningStyles.DONT_MATTER,
            };
        }

        public Genders GetPrefferdGender(string value)
        {
            return value switch
            {
                "אני מעוניין ללמוד רק עם גבר" => Genders.MALE,
                "אני מעוניינת ללמוד רק עם אישה" => Genders.FMALE,
                "אין לי העדפה" => Genders.DONT_MATTER,
                _ => Genders.DONT_MATTER,
            };
        }

        private PrefferdTracks SwitchPrefferdTracks(string value)
        {
            return value.Replace(",", "").Trim() switch
            {
                "תניא" => PrefferdTracks.TANYA,
                "גמרא" => PrefferdTracks.TALMUD,
                "פרשת שבוע" => PrefferdTracks.PARASHA,
                "תפילה" => PrefferdTracks.PRAYER,
                "פרקי אבות" => PrefferdTracks.PIRKEY_AVOT,
                "אין לי העדפה" => PrefferdTracks.DONT_MATTER,
                _ => PrefferdTracks.DONT_MATTER,
            };
        }

        public List<PrefferdTracks> GetPrefferdTracks(string value)
        {
            var tracksInString = value.Split(',');
            var result = new List<PrefferdTracks>();
            foreach (var s in tracksInString)
            {
                result.Add(SwitchPrefferdTracks(s));
            }
            return result;
        }


        public SkillLevels GetSkillLevel(string value)
        {
            return value switch
            {
                "טובה" => SkillLevels.ADVANCED,
                "רמת שיחה (בינונית)" => SkillLevels.MODERATE,
                "מתחיל" => SkillLevels.BEGGINER,
                "אין לי העדפה" => SkillLevels.DONT_MATTER,
                _ => SkillLevels.DONT_MATTER,
            };
        }

        public TimeSpan GetStudentOffset(string value)
        {
            return TimeZoneInfo.Local.BaseUtcOffset;
        }

        public IEnumerable<TimesInDay> GetTimesInDey(string value)
        {
            var timesInString = value.Split(',');
            var result = new List<TimesInDay>();

            foreach (var s in timesInString)
            {
                switch (s.Replace(",", "").Trim())
                {
                    case "בוקר":
                        result.Add(TimesInDay.MORNING);
                        break;
                    case "צהריים":
                        result.Add(TimesInDay.NOON);
                        break;
                    case "ערב":
                        result.Add(TimesInDay.EVENING);
                        break;
                    case "לילה":
                        result.Add(TimesInDay.NIGHT);
                        break;
                    case "אין לי זמן ביום זה":
                        result.Add(TimesInDay.INCAPABLE);
                        break;
                }
            }
            return result;
        }

        public Days GetDay(int i)
        {
            return i switch
            {
                2 => Days.SUNDAY,
                3 => Days.MONDAY,
                4 => Days.TUESDAY,
                5 => Days.WEDNESDAY,
                6 => Days.THURSDAY,
                _ => Days.DONT_MATTER,
            };
        }

        public string GetCountryName(string value)
        {
            return "Israel";
        }

        public IEnumerable<string> GetLanguages(string value)
        {
            var lang = value.Split(',');
            List<string> result = new();

            foreach (var l in lang)
            {
                switch (l.Replace(",", "").Trim())
                {
                    case "ספרדית":
                        result.Add("Spanish");
                        break;
                    case "צרפתית":
                        result.Add("French");
                        break;
                    case "רוסית":
                        result.Add("Russian");
                        break;
                    case "גרמנית":
                        result.Add("German");
                        break;
                }
            }
            return result;
        }

        public MoreLanguages GetMoreLanguages(string value)
        {
            return value switch
            {
                "כן" => MoreLanguages.YES,
                "לא" or "" => MoreLanguages.NO,
                "אינני יודע אנגלית אך אני יכול ללמוד בשפות אחרות" => MoreLanguages.NOT_ENGLISH,
                _ => MoreLanguages.NO,
            };
        }

        public int GetPrefferdNumberOfMatchs(string value)
        {
            switch (value)
            {
                case "חברותא אחת":
                    return 1;
                case "2 חברותות":
                    return 2;
                case "3 חברותות":
                    return 3;
                default:
                    break;
            }
            return 1;
        }
    }
}
