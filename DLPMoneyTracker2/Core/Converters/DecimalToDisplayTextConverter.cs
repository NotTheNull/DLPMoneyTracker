using System;
using System.Globalization;
using System.Windows.Data;

namespace DLPMoneyTracker2.Core.Converters
{
    public class DecimalToDisplayTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            if (value is decimal number)
            {
                if (parameter is not null)
                {
                    return number.ToString(parameter.ToString());
                }
                else
                {
                    return string.Format("{0:#,###.00###}", number);
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return decimal.Zero;

            if (decimal.TryParse(value.ToString(), out decimal number))
            {
                return number;
            }

            return decimal.Zero;
        }
    }
}