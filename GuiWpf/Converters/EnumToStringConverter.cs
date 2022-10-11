using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PairMatching.Models;
using static PairMatching.Tools.Extensions;


namespace GuiWpf.Converters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null)
            {
                return "";
            }
            if(value is Enum enumValue)
            {
                return enumValue.GetDescriptionFromEnumValue();
            }
            if(value is IEnumerable enumValues)
            {
                return enumValues.Cast<Enum>().Select(e => e.GetDescriptionFromEnumValue());
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value?.ToString();
            var result = GetValueFromDescription(val, targetType);
            return result;
        }
    }
}
