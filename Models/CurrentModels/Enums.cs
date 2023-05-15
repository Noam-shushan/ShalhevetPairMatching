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

        Defulte,

        [EnumDescription("לא יכול ביום זה", "nop")]
        Incapable 
    }

    public enum PrefferdTracks
    {
        [EnumDescription("חסידות", "Chassidic Thought", "df6ce1e8-1839-4749-bd4f-495295d75657")]
        //[EnumDescription(DescriptionId = "e9a52d6e-5510-4259-a157-c661e9ff95e9", HebDescription = "חסידות", EngDescriptions = { "Chassidic Thought"})]
        Tanya,
        
        [EnumDescription("גמרא", "Talmud", "e9a52d6e-5510-4259-a157-c661e9ff95e9")]
        Talmud,
        
        [EnumDescription("פרשה", "Weekly Parsha", "c44f84d4-a7b3-4125-9e1d-2000f9afb76e")]
        Parsha,
        
        [EnumDescription("תפילה", "Prayer", "c01b5f93-3797-473e-9eff-17bd7bddf736")]
        Payer,
        
        [EnumDescription("פרקי אבות", "Pirkei Avot", "8fc9e767-d4bf-4093-ad17-bb366ca31adf")]
        PirkeiAvot,

        [EnumDescription("לא משנה", "Dont Prefferd")]
        NoPrefrence,
        
        [EnumDescription("עצמאי" , "Independent", "788830c2-45f4-471d-aa0d-8c7412826562")]
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
        
        Defulte,
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
        NoPrefrence
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
        NoPrefrence
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

    public enum PairStatus
    {
        Defulte,
        [EnumDescription("המתנה")]
        Standby,
        [EnumDescription("פעיל")]
        Active,
        [EnumDescription("לומדים בפועל")]
        Learning
    }
    
}
