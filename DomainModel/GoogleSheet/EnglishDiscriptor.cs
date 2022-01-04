using Microsoft.Extensions.Configuration;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairMatching.DomainModel.GoogleSheet
{
    /// <summary>
    /// Descriptor for the values in the spreadsheet of the students from around the world
    /// </summary>
    public class EnglishDiscriptor : IStudentDescriptor
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
        public string SheetName => "Shalhevet Regestration form תשפ\"א(תגובות)";

        public EnglishDiscriptor(SpredsheetLastRange lastRange, IConfiguration config)
        {
            Range = lastRange.EnglishSheets;
            SpreadsheetId = config.GetSection("SpreadsheetsId")["English"];
        }

        public EnglishLevels GetEnglishLevel(string value)
        {
            switch (value.ToLower())
            {
                case "excellent (i don't know any hebrew whatsoever)":
                    return EnglishLevels.GOOD;
                case "doesn't have to be perfect. i know some Hebrew":
                    return EnglishLevels.NOT_GOOD;
                case "conversational level":
                    return EnglishLevels.TALK_LEVEL;
            }
            return EnglishLevels.DONT_MATTER;
        }

        public Genders GetGender(string value)
        {
            switch (value.ToLower())
            {
                case "male":
                    return Genders.MALE;
                case "female":
                    return Genders.FMALE;
                case "prefer not to say":
                    return Genders.DONT_MATTER;
            }
            return default;
        }

        public LearningStyles GetLearningStyle(string value)
        {
            switch (value.ToLower())
            {
                case "deep and slow":
                    return LearningStyles.DEEP_AND_SLOW;
                case "progressed, flowing":
                    return LearningStyles.PROGRESSED_FLOWING;
                case "text centered":
                    return LearningStyles.TEXTUALL_CENTERED;
                case "philosofical, free talking, deriving from text into thought":
                    return LearningStyles.FREE;
            }
            return LearningStyles.DONT_MATTER;
        }

        public Genders GetPrefferdGender(string value)
        {
            switch (value.ToLower())
            {
                case "only with men":
                    return Genders.MALE;
                case "only with women":
                    return Genders.FMALE;
                case "no prefrence":
                    return Genders.DONT_MATTER;
            }
            return Genders.DONT_MATTER;
        }

        private PrefferdTracks SwitchPrefferdTracks(string value)
        {
            switch (value.Replace(",", "").Trim().ToLower())
            {
                case "tanya":
                    return PrefferdTracks.TANYA;
                case "talmud":
                    return PrefferdTracks.TALMUD;
                case "parsha":
                    return PrefferdTracks.PARASHA;
                case "tefilah (prayer)":
                    return PrefferdTracks.PRAYER;
                case "pirkey avot (ethics of the fathers)":
                    return PrefferdTracks.PIRKEY_AVOT;
                case "no preference":
                    return PrefferdTracks.DONT_MATTER;
                case "independent learning subject":
                    return PrefferdTracks.IndependentLearning;
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
            switch (value.ToLower())
            {
                case "advanced":
                    return SkillLevels.ADVANCED;
                case "moderate":
                    return SkillLevels.MODERATE;
                case "begginer":
                    return SkillLevels.BEGGINER;
            }
            return SkillLevels.DONT_MATTER;
        }

        public TimeSpan GetStudentOffset(string value)
        {
            string timeFormat = Regex.Replace(value, "[^0-9.:-]", "");
            return TimeSpan.Parse(timeFormat);
        }

        public IEnumerable<TimesInDay> GetTimesInDey(string value)
        {
            var timesInString = value
                .Replace("Late", "")
                .Split(',');
            var result = new List<TimesInDay>();

            foreach (var s in timesInString)
            {
                switch (s.Replace(",", "").Trim().ToLower())
                {
                    case "morning":
                        result.Add(TimesInDay.MORNING);
                        break;
                    case "noon":
                        result.Add(TimesInDay.NOON);
                        break;
                    case "evening":
                        result.Add(TimesInDay.EVENING);
                        break;
                    case "night":
                        result.Add(TimesInDay.NIGHT);
                        break;
                    case "this day is not available for me":
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
            var rgx = new Regex("[^a-zA-Z ]");
            return rgx.Replace(value, "").Trim();
        }

        public IEnumerable<string> GetLanguages(string value)
        {
            return from l in value.Split(',')
                   select l.Replace(",", "").Trim();
        }

        public MoreLanguages GetMoreLanguages(string value)
        {
            switch (value.ToLower())
            {
                case "yes":
                    return MoreLanguages.YES;
                case "no":
                    return MoreLanguages.NO;
                case "i don't know English but i can learn in other languages":
                    return MoreLanguages.NOT_ENGLISH;
            }
            return MoreLanguages.NO;
        }

        public int GetPrefferdNumberOfMatchs(string value)
        {
            switch (value.ToLower())
            {
                case "one is enough":
                    return 1;
                case "i would like to have 2":
                    return 2;
                case "i would like to have 3":
                    return 3;
                default:
                    break;
            }
            return 1;
        }
    }
}