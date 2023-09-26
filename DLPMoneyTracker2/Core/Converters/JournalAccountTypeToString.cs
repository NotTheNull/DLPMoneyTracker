﻿using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DLPMoneyTracker2.Core.Converters
{
    public class JournalAccountTypeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            if (value is JournalAccountType acctType)
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