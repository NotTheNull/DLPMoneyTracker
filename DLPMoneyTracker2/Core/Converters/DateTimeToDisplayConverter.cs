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

            DateTime date;
            if(value is DateTime?)
            {
                DateTime? work = (DateTime?)value;
                if (!work.HasValue) return string.Empty;
                date = work.Value;
            }
            else if (value is DateTime work)
            {
                date = work;
            }
            else
            {
                return string.Empty;
            }

            if (parameter is null)
            {
                return string.Format("{0:yyyy/MM/dd}", date);
            }
            else
            {
                return date.ToString(parameter.ToString());
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}