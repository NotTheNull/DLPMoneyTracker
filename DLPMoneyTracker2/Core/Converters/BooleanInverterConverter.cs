﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace DLPMoneyTracker2.Core.Converters
{
    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            if (value is bool b)
            {
                return !b;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            if (value is bool b)
            {
                return !b;
            }

            return false;
        }
    }
}