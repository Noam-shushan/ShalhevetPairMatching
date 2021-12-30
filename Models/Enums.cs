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
}
