using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PairMatching.Configurations;

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

        public EnglishDiscriptor(SpredsheetLastRange lastRange, MyConfiguration config)
        {
            Range = lastRange.EnglishSheets;
            SpreadsheetId = config.SpreadsheetsId["English"];
        }

        public EnglishLevels GetEnglishLevel(string value)
        {
            return value.ToLower() switch
            {
                "excellent (i don't know any hebrew whatsoever)" => EnglishLevels.Good,
                "doesn't have to be perfect. i know some Hebrew" => EnglishLevels.NotGood,
                "conversational level" => EnglishLevels.ConversationalLevel,
                _ => EnglishLevels.Defulte,
            };
        }

        public Genders GetGender(string value)
        {
            return value.ToLower() switch
            {
                "male" => Genders.Male,
                "female" => Genders.Female,
                "prefer not to say" => Genders.NoPrefrence,
                _ => default,
            };
        }

        public LearningStyles GetLearningStyle(string value)
        {
            return value.ToLower() switch
            {
                "deep and slow" => LearningStyles.DeepAndSlow,
                "progressed, flowing" => LearningStyles.ProgressedFlowing,
                "text centered" => LearningStyles.TextCentered,
                "philosofical, free talking, deriving from text into thought" => LearningStyles.Free,
                _ => LearningStyles.DontMatter,
            };
        }

        public Genders GetPrefferdGender(string value)
        {
            return value.ToLower() switch
            {
                "only with men" => Genders.Male,
                "only with women" => Genders.Female,
                "no prefrence" => Genders.NoPrefrence,
                _ => Genders.NoPrefrence,
            };
        }

        private PrefferdTracks SwitchPrefferdTracks(string value)
        {
            return value.Replace(",", "").Trim().ToLower() switch
            {
                "tanya" => PrefferdTracks.Tanya,
                "talmud" => PrefferdTracks.Talmud,
                "parsha" => PrefferdTracks.Parsha,
                "tefilah (prayer)" => PrefferdTracks.Payer,
                "pirkey avot (ethics of the fathers)" => PrefferdTracks.PirkeiAvot,
                "no preference" => PrefferdTracks.DONT_MATTER,
                "independent learning subject" => PrefferdTracks.IndependentLearning,
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
            return value.ToLower() switch
            {
                "advanced" => SkillLevels.Advanced,
                "moderate" => SkillLevels.Moderate,
                "begginer" => SkillLevels.Beginner,
                _ => SkillLevels.DontMatter,
            };
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
                        result.Add(TimesInDay.Morning);
                        break;
                    case "noon":
                        result.Add(TimesInDay.Noon);
                        break;
                    case "evening":
                        result.Add(TimesInDay.Evening);
                        break;
                    case "night":
                        result.Add(TimesInDay.Night);
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
            return value.ToLower() switch
            {
                "yes" => MoreLanguages.Yes,
                "no" => MoreLanguages.No,
                "i don't know English but i can learn in other languages" => MoreLanguages.NotEnglish,
                _ => MoreLanguages.No,
            };
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