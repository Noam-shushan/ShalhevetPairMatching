using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PairMatching.Models;

namespace GuiWpf.Converters
{
    public class EnglishLevelsHebConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum.TryParse(value.ToString(), out EnglishLevels englishLevel);
            return englishLevel switch
            {
                EnglishLevels.GOOD => "טובה",
                EnglishLevels.NOT_GOOD => "לא טובה",
                EnglishLevels.TALK_LEVEL => "רמת שיחה",
                EnglishLevels.DONT_MATTER => "לא משנה",
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class GendersHebConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum.TryParse(value.ToString(), out Genders gender);
            return gender switch
            {
                Genders.MALE => "גבר",
                Genders.FMALE => "אישה",
                Genders.DONT_MATTER => "לא משנה",
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class SkillLevelsHebConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum.TryParse(value.ToString(), out SkillLevels skillLevel);
            return skillLevel switch
            {
                SkillLevels.BEGGINER => "מתחיל",
                SkillLevels.MODERATE => "בינוני",
                SkillLevels.DONT_MATTER => "לא משנה",
                SkillLevels.ADVANCED => "מתקדם",
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class LearningStylesHebConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum.TryParse(value.ToString(), out LearningStyles learningStyle);
            return learningStyle switch
            {
                LearningStyles.DEEP_AND_SLOW => "לימוד איטי ומעמיק",
                LearningStyles.PROGRESSED_FLOWING => "לימוד מהיר, הספקי ומתקדם",
                LearningStyles.TEXTUALL_CENTERED => "לימוד צמוד טקסט",
                LearningStyles.FREE => "לימוד מעודד מחשבה מחוץ לטקסט, פילוסופי",
                LearningStyles.DONT_MATTER => "לא משנה",
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
