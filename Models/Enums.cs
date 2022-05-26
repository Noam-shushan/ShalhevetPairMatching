using PairMatching.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public enum Genders 
    {
        [EnumDescription("גבר", "Male", "only men")]
        Male,
        [EnumDescription("אישה", "Female", "only women")]
        Female,
        [EnumDescription("לא משנה", "no prefrence")]
        NoPrefrence 
    }

    public enum Days 
    { 
        [EnumDescription("ראשון", "Sunday")]
        Sunday,

        [EnumDescription("שני", "Monday")]
        Monday,

        [EnumDescription("שלישי", "Tuesday")]
        Tuesday,

        [EnumDescription("רביעי", "Wednesday")]
        Wednesday,

        [EnumDescription("חמישי", "Thursday")]
        Thursday,

        [EnumDescription("אין", "nop")]
        Defulte
    }
    public enum TimesInDay 
    {
        [EnumDescription("בוקר", "Morning")]
        Morning,

        [EnumDescription("צהריים", "Noon")]
        Noon,
        
        [EnumDescription("ערב", "Evening")]
        Evening,

        [EnumDescription("מאוחר בלילה", "Late night")]
        Night,

        [EnumDescription("אין", "nop")]
        Defulte,

        [EnumDescription("לא יכול ביום זה", "nop")]
        INCAPABLE 
    }

    public enum PrefferdTracks 
    { 
        [EnumDescription("תניא", "Tanya")]
        Tanya,
        
        [EnumDescription("גמרא", "Talmud")]
        Talmud,
        
        [EnumDescription("פרשה", "Parsha")]
        Parsha,
        
        [EnumDescription("תפילה", "Payer")]
        Payer,
        
        [EnumDescription("פרקי אבות", "Pirkei Avot (Ethics of the Fathers)")]
        PirkeiAvot,
        
        DONT_MATTER,
        
        [EnumDescription("עצמאי" ,"Independent learning subject")]
        IndependentLearning
    }
    public enum EnglishLevels 
    { 
        [EnumDescription("לא כל כך טובה", "Doesn't have to be perfect. I know some Hebrew")]
        NotGood,

        [EnumDescription("רמת שיחה (בינונית)", "Conversational level")]
        ConversationalLevel,
        
        [EnumDescription("טובה", "Excellent (I don't know any Hebrew whatsoever)")]
        Good,

        [EnumDescription("אין", "nop")]
        Defulte 
    }

    public enum SkillLevels 
    { 
        [EnumDescription("מתחיל", "Beginner", "לא מכיר/ה. מאוד מתעיינ/ת")]
        Beginner,
        
        [EnumDescription("בינונית", "Moderate")]
        Moderate,
        
        [EnumDescription("גבוהה", "Advanced")]
        Advanced,
        
        [EnumDescription("אין העדפה" ,"אין לי מסלול מועדף")]
        DontMatter 
    }

    public enum LearningStyles 
    { 
        [EnumDescription("לימוד איטי ומעמיק", "Deep and slow")]
        DeepAndSlow, 

        [EnumDescription("לימוד מהיר, הספקי ומתקדם", "Progressed, flowing")]
        ProgressedFlowing,
        
        [EnumDescription("", "Text centered")]
        TextCentered,
        
        [EnumDescription("לימוד מעודד מחשבה מחוץ לטקסט, פילוסופי", "Philosophical, free talking, deriving from text into thought")]
        Free, 
        
        [EnumDescription("אין לי העדפה", "No significant or particular style")]
        DontMatter 
    }

    public enum MoreLanguages 
    { 
        [EnumDescription("כן", "Yes")]
        Yes,
        
        [EnumDescription("לא", "No")]
        No,
        
        [EnumDescription("איני יודע/ת אנגלית אך אני יכול/ה ללמוד בשפות אחרות", "I don't know English but I can learn in other languages")]
        NotEnglish 
    }
    
}
