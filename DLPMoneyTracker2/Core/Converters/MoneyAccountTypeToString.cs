using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DLPMoneyTracker.Core.Converters
{
    public class MoneyAccountTypeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            if (value is MoneyAccountType acctType)
            {
                return acctType.ToDisplayText();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}