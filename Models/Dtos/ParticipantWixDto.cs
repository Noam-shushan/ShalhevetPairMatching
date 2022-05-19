using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Models;

namespace PairMatching.Models.Dtos
{
    public class ParticipantWixDto
    {
        public List<object> sunday { get; set; }

        public List<object> thurseday { get; set; }

        public string personalTraits { get; set; }

        public string specifyLang { get; set; }

        public string biographHeb { get; set; }

        public string tel { get; set; }

        public string whoIntroduced { get; set; }

        public string email { get; set; }

        public string moreThanOneChevruta { get; set; }

        public string whyJoinShalhevet { get; set; }

        public string _id { get; set; }

        public List<object> tuseday { get; set; }

        public string _owner { get; set; }

        public DateTime _createdDate { get; set; }

        public string learningSkill { get; set; }

        public string fullName { get; set; }

        public List<object> wednesday { get; set; }

        public List<object> otherLan { get; set; }

        public string otherLang { get; set; }

        public string levOfEn { get; set; }

        public DateTime _updatedDate { get; set; }

        public List<object> monday { get; set; }

        public string learningStyle { get; set; }

        public string menOrWomen { get; set; }

        public List<string> preferredTrack { get; set; }

        public string additionalInfo { get; set; }

        public string chevrotaSkills { get; set; }

        public int index { get; set; }

        public string gender { get; set; }

        public Participant ToParticipant()
        {
            return new Participant
            {
                Name = fullName,
                WixId = _id,
                DateOfRegistered = _createdDate,
                Email = email,
                WixIndex = index,
                PhoneNumber = tel,
                Gender = Dictionaries.GendersDictInverse[gender],
                PairPreferences = new Preferences
                {
                    Gender = Dictionaries.GendersDictInverse[menOrWomen],
                    EnglishLevel = Dictionaries.EnglishLevelsDictInverse[levOfEn],
                    LearningStyle = Dictionaries.LearningStylesDictInverse[learningStyle],
                    SkillLevel = Dictionaries.SkillLevelsDictInverse[learningSkill],
                    NumberOfMatchs = int.Parse(moreThanOneChevruta),
                    Tracks = preferredTrack.Select(t => Dictionaries.PrefferdTracksDictInverse[t]),
                    LearningTime = from day in Enum.GetValues(typeof(Days)).Cast<Days>()
                                   from dayDto in new[] { sunday, monday, tuseday, wednesday, thurseday, }
                                   select new LearningTime
                                   {
                                       Day = day,
                                       TimeInDay = dayDto.Select(t => Dictionaries.TimesInDayDictInverse[t.ToString()])
                                   }
                                  
                }
            };
        }
    }
}
