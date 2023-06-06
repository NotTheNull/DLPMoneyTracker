using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DLPMoneyTracker2.Core.Converters
{
    public class AccountTypeToBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Brushes.White;
            if (value is JournalAccountType acctType)
            {
                Color primaryColor = Colors.White;
                switch (acctType)
                {
                    case JournalAccountType.Bank:
                        primaryColor = Colors.LightGreen;
                        break;

                    case JournalAccountType.LiabilityCard:
                        primaryColor = Colors.Yellow;
                        break;

                    case JournalAccountType.LiabilityLoan:
                        primaryColor = Colors.Orange;
                        break;

                    default:
                        return Brushes.White;
                }

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