using PairMatching.Configurations;
using PairMatching.Models;
using System;
using System.Collections.Generic;


namespace PairMatching.GoogleSheet
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
                "טובה" => EnglishLevels.Good,
                "לא כל כך טובה" => EnglishLevels.NotGood,
                "רמת שיחה" => EnglishLevels.ConversationalLevel,
                _ => EnglishLevels.Defulte,
            };
        }

        public Genders GetGender(string value)
        {
            return value switch
            {
                "גבר" => Genders.Male,
                "אישה" => Genders.Female,
                "לא משנה" => Genders.NoPrefrence,
                _ => Genders.NoPrefrence,
            };
        }

        public LearningStyles GetLearningStyle(string value)
        {
            return value switch
            {
                "לימוד איטי ומעמיק" => LearningStyles.DeepAndSlow,
                "לימוד מהיר, הספקי ומתקדם" => LearningStyles.ProgressedFlowing,
                "לימוד צמוד טקסט" => LearningStyles.TextCentered,
                "לימוד מעודד מחשבה מחוץ לטקסט, פילוסופי" => LearningStyles.Free,
                _ => LearningStyles.NoPrefrence,
            };
        }

        public Genders GetPrefferdGender(string value)
        {
            return value switch
            {
                "אני מעוניין ללמוד רק עם גבר" => Genders.Male,
                "אני מעוניינת ללמוד רק עם אישה" => Genders.Female,
                "אין לי העדפה" => Genders.NoPrefrence,
                _ => Genders.NoPrefrence,
            };
        }

        private PrefferdTracks SwitchPrefferdTracks(string value)
        {
            return value.Replace(",", "").Trim() switch
            {
                "תניא" => PrefferdTracks.Tanya,
                "גמרא" => PrefferdTracks.Talmud,
                "פרשת שבוע" => PrefferdTracks.Parsha,
                "תפילה" => PrefferdTracks.Payer,
                "פרקי אבות" => PrefferdTracks.PirkeiAvot,
                "אין לי העדפה" => PrefferdTracks.NoPrefrence,
                _ => PrefferdTracks.NoPrefrence,
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
                "טובה" => SkillLevels.Advanced,
                "רמת שיחה (בינונית)" => SkillLevels.Moderate,
                "מתחיל" => SkillLevels.Beginner,
                "אין לי העדפה" => SkillLevels.NoPrefrence,
                _ => SkillLevels.NoPrefrence,
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
                        result.Add(TimesInDay.Morning);
                        break;
                    case "צהריים":
                        result.Add(TimesInDay.Noon);
                        break;
                    case "ערב":
                        result.Add(TimesInDay.Evening);
                        break;
                    case "לילה":
                        result.Add(TimesInDay.Night);
                        break;
                    case "אין לי זמן ביום זה":
                        result.Add(TimesInDay.Incapable);
                        break;
                }
            }
            return result;
        }

        public Days GetDay(int i)
        {
            return i switch
            {
                2 => Days.Sunday,
                3 => Days.Monday,
                4 => Days.Tuesday,
                5 => Days.Wednesday,
                6 => Days.Thursday,
                _ => Days.Defulte,
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
                "כן" => MoreLanguages.Yes,
                "לא" or "" => MoreLanguages.No,
                "אינני יודע אנגלית אך אני יכול ללמוד בשפות אחרות" => MoreLanguages.NotEnglish,
                _ => MoreLanguages.No,
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
