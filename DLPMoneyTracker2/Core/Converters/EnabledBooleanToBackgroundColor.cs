using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DLPMoneyTracker2.Core.Converters
{
    public class EnabledBooleanToBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush disabled = (SolidColorBrush)System.Windows.Application.Current.FindResource("DisabledTextBox");
            SolidColorBrush enabled = (SolidColorBrush)System.Windows.Application.Current.FindResource("EnabledTextBox");

            if (value is null) return disabled;
            if (value is bool isEnabled)
            {
                return isEnabled ? enabled : disabled;
            }

            return disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}