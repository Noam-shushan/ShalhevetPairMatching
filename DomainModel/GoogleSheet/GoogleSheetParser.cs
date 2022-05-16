using PairMatching.Models;
using PairMatching.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PairMatching.DomainModel.GoogleSheet
{
    /// <summary>
    /// Google Sheet parser that descript the values in the spreadsheet table.<br/>
    /// Save the description objects to the database.
    /// </summary>
    internal class GoogleSheetParser
    {
        const int TIME_COLUMN_START = 2;
        const int TIME_COLUMN_END = 7;

        // Reader for the spreadsheets
        private readonly GoogleSheetReader sheetReader;

        public List<Student> NewStudents { get; init; }

        /// <summary>
        /// Constructor for GoogleSheetParser
        /// Set the GoogleSheetReader that get the key for reading the spreadsheets.
        /// Clears data left over from previous reading
        /// <throw></throw>
        /// </summary>
        public GoogleSheetParser()
        {
            try
            {
                sheetReader = new GoogleSheetReader();
                NewStudents = new();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Read the data from the Google sheet.
        /// Decoder the data and create objects from the data.
        /// </summary>
        /// <param name="studentDescriptor">Descriptor from the data</param>
        /// <returns>the last data of update of the spreadsheet</returns>
        public async Task<string> ReadAsync(IStudentDescriptor studentDescriptor)
        {
            string result = studentDescriptor.Range;
            
            int rowNumber = GetRowCurrentNumber(studentDescriptor.Range);
            
            var table = await Task.Run(() => sheetReader.ReadEntries(studentDescriptor));

            if (table == null)
                return result;

            // save the last range that read in the spreadsheet
            result = $"A{table.Count + rowNumber}:Z";

            if (studentDescriptor is HebrewDescriptor)
            {
                CreateDataFromHebrewSheet(table, studentDescriptor);
            }
            else if (studentDescriptor is EnglishDiscriptor)
            {
                CreateDataFromEnglishSheet(table, studentDescriptor);
            }
            return result;
        }

        private static int GetRowCurrentNumber(string range)
        {
            var rowNumStr = range
                .Substring(range.IndexOf("A") + 1,
                range.IndexOf(":") - 1);
            int rowNumber = int.Parse(rowNumStr);
            return rowNumber;
        }

        private void CreateDataFromHebrewSheet(List<List<string>> table, IStudentDescriptor studentDescriptor)
        {
            foreach (var row in table)
            {
                try
                {
                    NewStudents.Add(new Student
                    {
                        DateOfRegistered = GetDate(row[0]),
                        Name = row[columnIndexerHebrewSheet["Name"]],
                        DesiredLearningTime = GetLearningTime(row, studentDescriptor),
                        OpenQuestions = GetQandAheb(row),
                        PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[columnIndexerHebrewSheet["PrefferdTracks"]]),
                        PrefferdGender = studentDescriptor.GetPrefferdGender(row[columnIndexerHebrewSheet["PrefferdGender"]]),
                        EnglishLevel = studentDescriptor.GetEnglishLevel(row[columnIndexerHebrewSheet["EnglishLevel"]]),
                        DesiredSkillLevel = studentDescriptor.GetSkillLevel(row[columnIndexerHebrewSheet["DesiredSkillLevel"]]),
                        LearningStyle = studentDescriptor.GetLearningStyle(row[columnIndexerHebrewSheet["LearningStyle"]]),
                        Gender = studentDescriptor.GetGender(row[columnIndexerHebrewSheet["Gender"]]),
                        Country = studentDescriptor.GetCountryName(null),
                        PhoneNumber = row[columnIndexerHebrewSheet["PhoneNumber"]],
                        Email = row[columnIndexerHebrewSheet["Email"]],
                        MoreLanguages = studentDescriptor.GetMoreLanguages(row[columnIndexerHebrewSheet["MoreLanguages"]]),
                        Languages = studentDescriptor.GetLanguages(row[columnIndexerHebrewSheet["Languages"]]),
                        PrefferdNumberOfMatchs = studentDescriptor
                            .GetPrefferdNumberOfMatchs(row[columnIndexerHebrewSheet["PrefferdNumberOfMatchs"]])
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private void CreateDataFromEnglishSheet(List<List<string>> table, IStudentDescriptor studentDescriptor)
        {
            foreach (var row in table)
            {
                try
                {
                    NewStudents.Add(new Student
                    {
                        DateOfRegistered = GetDate(row[0]),
                        Name = row[columnIndexerEnglishSheet["Name"]],
                        DesiredLearningTime = GetLearningTime(row, studentDescriptor),
                        OpenQuestions = GetQandAeng(row),
                        PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[columnIndexerEnglishSheet["PrefferdTracks"]]),
                        PrefferdGender = studentDescriptor.GetPrefferdGender(row[columnIndexerEnglishSheet["PrefferdGender"]]),
                        DesiredEnglishLevel = studentDescriptor.GetEnglishLevel(row[columnIndexerEnglishSheet["DesiredEnglishLevel"]]),
                        SkillLevel = studentDescriptor.GetSkillLevel(row[columnIndexerEnglishSheet["SkillLevel"]]),
                        LearningStyle = studentDescriptor.GetLearningStyle(row[columnIndexerEnglishSheet["LearningStyle"]]),
                        Gender = studentDescriptor.GetGender(row[columnIndexerEnglishSheet["Gender"]]),
                        Country = studentDescriptor.GetCountryName(row[columnIndexerEnglishSheet["Country"]]).SpliceText(3),
                        UtcOffset = studentDescriptor.GetStudentOffset(row[columnIndexerEnglishSheet["UtcOffset"]]),
                        PhoneNumber = row[columnIndexerEnglishSheet["PhoneNumber"]],
                        Email = row[columnIndexerEnglishSheet["Email"]],
                        MoreLanguages = studentDescriptor.GetMoreLanguages(row[columnIndexerEnglishSheet["MoreLanguages"]]),
                        Languages = studentDescriptor.GetLanguages(row[columnIndexerEnglishSheet["Languages"]]),
                        PrefferdNumberOfMatchs = studentDescriptor
                            .GetPrefferdNumberOfMatchs(row[columnIndexerEnglishSheet["PrefferdNumberOfMatchs"]])

                    });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private DateTime GetDate(string dateFormat)
        {
            return !DateTime.TryParse(dateFormat, out DateTime result) ? new DateTime() : result;
        }

        private IEnumerable<LearningTime> GetLearningTime(List<string> row, IStudentDescriptor studentDescriptor)
        {
            var result = new List<LearningTime>();
            for (int i = TIME_COLUMN_START; i < TIME_COLUMN_END; i++)
            {
                result.Add(new LearningTime
                {
                    Day = studentDescriptor.GetDay(i),
                    TimeInDay = studentDescriptor.GetTimesInDey(row[i])
                });
            }
            return result;
        }

        private IEnumerable<OpenQuestion> GetQandAheb(List<string> row)
        {
            var result = new List<OpenQuestion>
            {
                new OpenQuestion
                {
                    Answer = row[columnIndexerHebrewSheet["Personal information"]],
                    Question = "Personal information"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerHebrewSheet["What are your hopes and expectations from this program"]],
                    Question = "What are your hopes and expectations from this program"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerHebrewSheet["Personality trates"]],
                    Question = "Personality trates"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerHebrewSheet["Who introduced you to this program"]],
                    Question = "Who introduced you to this program"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerHebrewSheet["Additional information"]],
                    Question = "Additional information"
                }
            };

            return result;
        }

        private IEnumerable<OpenQuestion> GetQandAeng(List<string> row)
        {
            var result = new List<OpenQuestion>
            {
                new OpenQuestion
                {
                    Answer = row[columnIndexerEnglishSheet["Personal information"]],
                    Question = "Personal information"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerEnglishSheet["What are your hopes and expectations from this program"]],
                    Question = "What are your hopes and expectations from this program"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerEnglishSheet["Personality trates"]],
                    Question = "Personality trates"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerEnglishSheet["Who introduced you to this program"]],
                    Question = "Who introduced you to this program"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerEnglishSheet["Additional information"]],
                    Question = "Additional information"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerEnglishSheet["Country and City of residence"]],
                    Question = "Country and City of residence"
                },

                new OpenQuestion
                {
                    Answer = row[columnIndexerEnglishSheet["Anything else you would like to tell us"]],
                    Question = "Anything else you would like to tell us"
                }
            };
            return result;
        }

        /// <summary>
        /// indexer of the values in the Hebrew spreadsheet
        /// </summary>
        readonly Dictionary<string, int> columnIndexerHebrewSheet = new()
        {
            { "Name", 1 },
            { "PrefferdTracks", 7 },
            { "PrefferdGender", 8 },
            { "EnglishLevel", 9 },
            { "DesiredSkillLevel", 10 },
            { "LearningStyle", 11 },
            { "Gender", 12 },
            { "PhoneNumber", 13 },
            { "Email", 14 },
            { "Personal information", 15 },
            { "What are your hopes and expectations from this program", 16 },
            { "Personality trates", 17 },
            { "Who introduced you to this program", 18 },
            { "Additional information", 19 },
            { "PrefferdNumberOfMatchs", 20 },
            { "MoreLanguages", 21 },
            { "Languages", 22 }
        };

        /// <summary>
        /// indexer of the values in the English spreadsheet
        /// </summary>
        readonly Dictionary<string, int> columnIndexerEnglishSheet = new()
        {
            { "Name", 1 },
            { "PrefferdTracks", 7 },
            { "PrefferdGender", 8 },
            { "DesiredEnglishLevel", 9 },
            { "SkillLevel", 10 },
            { "LearningStyle", 11 },
            { "Gender", 12 },
            { "Country", 13 },
            { "UtcOffset", 13 },
            { "PhoneNumber", 14 },
            { "Email", 15 },
            { "Country and City of residence", 16 },
            { "Personal information", 17 },
            { "Personality trates", 18 },
            { "Additional information", 19 },
            { "What are your hopes and expectations from this program", 20 },
            { "Anything else you would like to tell us", 21 },
            { "Who introduced you to this program", 22 },
            { "PrefferdNumberOfMatchs", 23 },
            { "MoreLanguages", 24 },
            { "Languages", 25 }
        };
    }
}
