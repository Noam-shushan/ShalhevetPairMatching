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
                EnglishLevels.Good => "טובה",
                EnglishLevels.NotGood => "לא טובה",
                EnglishLevels.ConversationalLevel => "רמת שיחה",
                EnglishLevels.Defulte => "לא משנה",
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
                Genders.Male => "גבר",
                Genders.Female => "אישה",
                Genders.NoPrefrence => "לא משנה",
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
                SkillLevels.Beginner => "מתחיל",
                SkillLevels.Moderate => "בינוני",
                SkillLevels.DontMatter => "לא משנה",
                SkillLevels.Advanced => "מתקדם",
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
                LearningStyles.DeepAndSlow => "לימוד איטי ומעמיק",
                LearningStyles.ProgressedFlowing => "לימוד מהיר, הספקי ומתקדם",
                LearningStyles.TextCentered => "לימוד צמוד טקסט",
                LearningStyles.Free => "לימוד מעודד מחשבה מחוץ לטקסט, פילוסופי",
                LearningStyles.DontMatter => "לא משנה",
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
