using System;
using System.Globalization;
using System.Windows.Data;

namespace DLPMoneyTracker2.Core.Converters
{
    public class DateTimeToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;

            if (value is DateTime date)
            {
                if (parameter is null)
                {
                    return string.Format("{0:yyyy/MM/dd}", date);
                }
                else
                {
                    return date.ToString(parameter.ToString());
                }
            }

            return DateTime.MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}