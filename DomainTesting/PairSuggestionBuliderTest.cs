using NUnit.Framework;
using PairMatching.DomainModel.MatchingCalculations;
using PairMatching.Models;

namespace DomainTesting
{
    [TestFixture]
    internal class PairSuggestionBuliderTest
    {

        [Test]
        public void MatchCase()
        {
            var ip = new IsraelParticipant
            {
                Country = "Israel",
                DesiredSkillLevel = SkillLevels.Moderate, // 1
                EnglishLevel = EnglishLevels.Good, // 1
                Gender = Genders.Male,
                PairPreferences = new Preferences
                {
                    Gender = Genders.Male, // 1
                    LearningStyle = LearningStyles.Free,
                    Tracks = new List<PrefferdTracks> { PrefferdTracks.Tanya }, // 1
                    LearningTime = new List<LearningTime>
                    {
                        new LearningTime
                        {
                            Day = Days.Sunday, // Match 1
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon,
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Monday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime // Match day before 1
                        {
                            Day = Days.Tuesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Wednesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Thursday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Incapable
                            }
                        },

                    }
                }
            };
            var wp = new WorldParticipant
            {
                Country = "US",
                Gender = Genders.Male,
                DesiredEnglishLevel = EnglishLevels.ConversationalLevel, // 1
                SkillLevel = SkillLevels.Advanced, // 1
                UtcOffset = TimeSpan.FromHours(-5),
                PairPreferences = new Preferences
                {
                    Gender = Genders.Male, // 1
                    LearningStyle = LearningStyles.NoPrefrence,
                    Tracks = new List<PrefferdTracks> { PrefferdTracks.Tanya }, // 1
                    LearningTime = new List<LearningTime>
                    {
                        new LearningTime // Match 1
                        {
                            Day = Days.Sunday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime // Match day after 1
                        {
                            Day = Days.Monday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                                TimesInDay.Night
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Tuesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon,
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Wednesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon
                            }
                        },
                        new LearningTime
                        {
                            Day = Days.Thursday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning
                            }
                        },
                    }
                }
            };
            var ps = new PairSuggestionBulider(ip, wp, new TimeIntervalFactory())
                .Build();
            Assert.AreEqual(6, ps.MatchingScore);
        }

        [Test]
        public void NotMatchTrackCase()
        {
            var ip = new IsraelParticipant
            {
                Country = "Israel",
                DesiredSkillLevel = SkillLevels.Moderate, // 1
                EnglishLevel = EnglishLevels.Good, // 1
                Gender = Genders.Male,
                PairPreferences = new Preferences
                {
                    Gender = Genders.Male, // 1
                    LearningStyle = LearningStyles.Free,
                    Tracks = new List<PrefferdTracks> { PrefferdTracks.Tanya }, // 1
                    LearningTime = new List<LearningTime>
                    {
                        new LearningTime
                        {
                            Day = Days.Sunday, // Match 1
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon,
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Monday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime // Match day before 1
                        {
                            Day = Days.Tuesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Wednesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Thursday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Incapable
                            }
                        },

                    }
                }
            };
            var wp = new WorldParticipant
            {
                Country = "US",
                Gender = Genders.Male,
                DesiredEnglishLevel = EnglishLevels.ConversationalLevel, // 1
                SkillLevel = SkillLevels.Advanced, // 1
                UtcOffset = TimeSpan.FromHours(-5),
                PairPreferences = new Preferences
                {
                    Gender = Genders.Male, // 1
                    LearningStyle = LearningStyles.NoPrefrence,
                    Tracks = new List<PrefferdTracks> { PrefferdTracks.Parsha }, // -1
                    LearningTime = new List<LearningTime>
                    {
                        new LearningTime // Match 1
                        {
                            Day = Days.Sunday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime // Match day after 1
                        {
                            Day = Days.Monday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Incapable
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Tuesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon,
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Wednesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon
                            }
                        },
                        new LearningTime
                        {
                            Day = Days.Thursday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning
                            }
                        },
                    }
                }
            };
            var ps = new PairSuggestionBulider(ip, wp, new TimeIntervalFactory())
               .Build();
            Assert.Null(ps);
        }

        [Test]
        public void NotMatchTimeCase()
        {
            var ip = new IsraelParticipant
            {
                Country = "Israel",
                DesiredSkillLevel = SkillLevels.Moderate, // 1
                EnglishLevel = EnglishLevels.Good, // 1
                Gender = Genders.Male,
                PairPreferences = new Preferences
                {
                    Gender = Genders.Male, // 1
                    LearningStyle = LearningStyles.Free,
                    Tracks = new List<PrefferdTracks> { PrefferdTracks.Tanya }, // 1
                    LearningTime = new List<LearningTime>
                    {
                        new LearningTime
                        {
                            Day = Days.Sunday, 
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Night,
                            }
                        },
                        new LearningTime 
                        {
                            Day = Days.Monday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime 
                        {
                            Day = Days.Tuesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime 
                        {
                            Day = Days.Wednesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon
                            }
                        },
                        new LearningTime 
                        {
                            Day = Days.Thursday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Incapable
                            }
                        },

                    }
                }
            };
            var wp = new WorldParticipant
            {
                Country = "US",
                Gender = Genders.Male,
                DesiredEnglishLevel = EnglishLevels.ConversationalLevel, // 1
                SkillLevel = SkillLevels.Advanced, // 1
                UtcOffset = TimeSpan.FromHours(-5),
                PairPreferences = new Preferences
                {
                    Gender = Genders.Male, // 1
                    LearningStyle = LearningStyles.NoPrefrence,
                    Tracks = new List<PrefferdTracks> { PrefferdTracks.NoPrefrence }, // -1
                    LearningTime = new List<LearningTime>
                    {
                        new LearningTime // Match 1
                        {
                            Day = Days.Sunday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning,
                            }
                        },
                        new LearningTime // Match day after 1
                        {
                            Day = Days.Monday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Incapable
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Tuesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon,
                            }
                        },
                        new LearningTime // No match
                        {
                            Day = Days.Wednesday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Noon
                            }
                        },
                        new LearningTime
                        {
                            Day = Days.Thursday,
                            TimeInDay = new List<TimesInDay>
                            {
                                TimesInDay.Morning
                            }
                        },
                    }
                }
            };
            var ps = new PairSuggestionBulider(ip, wp, new TimeIntervalFactory())
               .Build();
            Assert.Null(ps);
        }
    }
}
