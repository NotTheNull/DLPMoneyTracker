using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace DLPMoneyTracker.Core.Converters
{
    public class MoneyAccountTypeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            if(value is MoneyAccountType acctType)
            {
                switch(acctType)
                {
                    case MoneyAccountType.Checking:
                        return "Checking";
                    case MoneyAccountType.CreditCard:
                        return "Credit Card";
                    case MoneyAccountType.Loan:
                        return "Loan";
                    case MoneyAccountType.Savings:
                        return "Savings";
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
