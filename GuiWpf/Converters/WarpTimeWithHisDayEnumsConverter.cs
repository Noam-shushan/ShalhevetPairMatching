using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PairMatching.Tools;
using PairMatching.Models;

namespace GuiWpf.Converters
{
    public class WarpTimeWithHisDayEnumsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return null;

            if (values[0] is string dayStr && values[1] is string timeStr)
            {
                var day = Extensions.GetValueFromDescription<Days>(dayStr);
                var time = Extensions.GetValueFromDescription<TimesInDay>(timeStr);
                return new Tuple<Days, TimesInDay>(day, time);
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class IsTimeAndDaySetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3)
                return null;

            if (values[0] is string dayStr 
                && values[1] is string timeStr 
                && values[2] is Dictionary<Tuple<Days, TimesInDay>, bool> openTimes)
            {
                var day = Extensions.GetValueFromDescription<Days>(dayStr);
                var time = Extensions.GetValueFromDescription<TimesInDay>(timeStr);
                return openTimes.ContainsKey(new Tuple<Days, TimesInDay>(day, time));
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
