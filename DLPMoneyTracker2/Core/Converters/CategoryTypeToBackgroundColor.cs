using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DLPMoneyTracker2.Core.Converters
{
    public class CategoryTypeToBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Brushes.Transparent;
            if (value is CategoryType catType)
            {
                switch (catType)
                {
                    case CategoryType.Expense:
                        return Brushes.Red;

                    case CategoryType.Income:
                        return Brushes.Green;

                    case CategoryType.Payment: // Debt Payment
                        return Brushes.Orange;

                    case CategoryType.TransferFrom:
                        return Brushes.Red;

                    case CategoryType.TransferTo:
                        return Brushes.Green;

                    case CategoryType.UntrackedAdjustment:
                        return Brushes.Blue;

                    default:
                        return Brushes.White;
                }
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}