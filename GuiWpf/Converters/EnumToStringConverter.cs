﻿using System;
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
            var result = (value as Enum).GetDescriptionFromEnumValue();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value?.ToString();
            var result = GetValueFromDescription(val, targetType);
            return result;
        }
    }
}
