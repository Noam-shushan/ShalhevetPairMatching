using Microsoft.Extensions.Configuration;
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


        public HebrewDescriptor(SpredsheetLastRange lastRange, IConfiguration config)
        {
            Range = lastRange.HebrewSheets;
            SpreadsheetId = config.GetSection("SpreadsheetsId")["Hebrew"];
        }

        public EnglishLevels GetEnglishLevel(string value)
        {
            switch (value)
            {
                case "טובה":
                    return EnglishLevels.GOOD;
                case "לא כל כך טובה":
                    return EnglishLevels.NOT_GOOD;
                case "רמת שיחה":
                    return EnglishLevels.TALK_LEVEL;
            }
            return EnglishLevels.DONT_MATTER;
        }

        public Genders GetGender(string value)
        {
            switch (value)
            {
                case "גבר":
                    return Genders.MALE;
                case "אישה":
                    return Genders.FMALE;
                case "לא משנה":
                    return Genders.DONT_MATTER;
            }
            return Genders.DONT_MATTER;
        }

        public LearningStyles GetLearningStyle(string value)
        {
            switch (value)
            {
                case "לימוד איטי ומעמיק":
                    return LearningStyles.DEEP_AND_SLOW;
                case "לימוד מהיר, הספקי ומתקדם":
                    return LearningStyles.PROGRESSED_FLOWING;
                case "לימוד צמוד טקסט":
                    return LearningStyles.TEXTUALL_CENTERED;
                case "לימוד מעודד מחשבה מחוץ לטקסט, פילוסופי":
                    return LearningStyles.FREE;
            }
            return LearningStyles.DONT_MATTER;
        }

        public Genders GetPrefferdGender(string value)
        {
            switch (value)
            {
                case "אני מעוניין ללמוד רק עם גבר":
                    return Genders.MALE;
                case "אני מעוניינת ללמוד רק עם אישה":
                    return Genders.FMALE;
                case "אין לי העדפה":
                    return Genders.DONT_MATTER;
            }
            return Genders.DONT_MATTER;
        }

        private PrefferdTracks SwitchPrefferdTracks(string value)
        {
            switch (value.Replace(",", "").Trim())
            {
                case "תניא":
                    return PrefferdTracks.TANYA;
                case "גמרא":
                    return PrefferdTracks.TALMUD;
                case "פרשת שבוע":
                    return PrefferdTracks.PARASHA;
                case "תפילה":
                    return PrefferdTracks.PRAYER;
                case "פרקי אבות":
                    return PrefferdTracks.PIRKEY_AVOT;
                case "אין לי העדפה":
                    return PrefferdTracks.DONT_MATTER;
            }
            return PrefferdTracks.DONT_MATTER;
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
            switch (value)
            {
                case "טובה":
                    return SkillLevels.ADVANCED;
                case "רמת שיחה (בינונית)":
                    return SkillLevels.MODERATE;
                case "מתחיל":
                    return SkillLevels.BEGGINER;
                case "אין לי העדפה":
                    return SkillLevels.DONT_MATTER;
            }
            return SkillLevels.DONT_MATTER;
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
            switch (i)
            {
                case 2:
                    return Days.SUNDAY;
                case 3:
                    return Days.MONDAY;
                case 4:
                    return Days.TUESDAY;
                case 5:
                    return Days.WEDNESDAY;
                case 6:
                    return Days.THURSDAY;

            }
            return Days.DONT_MATTER;
        }

        public string GetCountryName(string value)
        {
            return "Israel";
        }

        public IEnumerable<string> GetLanguages(string value)
        {
            var lang = value.Split(',');
            List<string> result = new List<string>();

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
            switch (value)
            {
                case "כן":
                    return MoreLanguages.YES;
                case "לא":
                case "":
                    return MoreLanguages.NO;
                case "אינני יודע אנגלית אך אני יכול ללמוד בשפות אחרות":
                    return MoreLanguages.NOT_ENGLISH;
            }
            return MoreLanguages.NO;
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
