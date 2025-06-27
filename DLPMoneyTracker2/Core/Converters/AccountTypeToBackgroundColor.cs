using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DLPMoneyTracker2.Core.Converters
{
    public class AccountTypeToBackgroundColor : IValueConverter
    {
        private static readonly List<LedgerType> validAccounts = [LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan];

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Brushes.White;
            if (value is LedgerType acctType)
            {
                if (!validAccounts.Contains(acctType)) return Brushes.White;

                Color primaryColor = acctType switch
                {
                    LedgerType.Bank => Colors.LightGreen,
                    LedgerType.LiabilityCard => Colors.Yellow,
                    LedgerType.LiabilityLoan => Colors.Orange,
                    _ => Colors.White
                };

                LinearGradientBrush brush = new LinearGradientBrush()
                {
                    StartPoint = new System.Windows.Point(1, 0),
                    EndPoint = new System.Windows.Point(1, 1)
                };
                brush.GradientStops.Add(new GradientStop(Colors.White, 0.0d));
                brush.GradientStops.Add(new GradientStop(primaryColor, 0.33d));
                brush.GradientStops.Add(new GradientStop(primaryColor, 0.66d));
                brush.GradientStops.Add(new GradientStop(Colors.White, 1.0d));
                return brush;
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}