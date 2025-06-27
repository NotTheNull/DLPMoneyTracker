using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DLPMoneyTracker2.Core.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility hide = Visibility.Collapsed;
            string args = parameter?.ToString() ?? string.Empty;

            if (args.Contains('H'))
            {
                hide = Visibility.Hidden;
            }

            if (value is null) return hide;
            if (value is bool flag)
            {
                if (args.Contains('R'))
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