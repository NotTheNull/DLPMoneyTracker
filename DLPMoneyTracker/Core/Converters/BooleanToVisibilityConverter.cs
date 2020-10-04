using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace DLPMoneyTracker.Core.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility hide = Visibility.Collapsed;
            string args = string.Empty;
            if(!(parameter is null))
            {
                args = parameter.ToString();
                if(args.Contains("H"))
                {
                    hide = Visibility.Hidden;
                }
            }

            if (value is null) return hide;

            if(value is bool flag)
            {
                if (args.Contains("R"))
                    return flag ? hide : Visibility.Visible;
                else
                    return flag ? Visibility.Visible : hide;
            }

            return hide;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
