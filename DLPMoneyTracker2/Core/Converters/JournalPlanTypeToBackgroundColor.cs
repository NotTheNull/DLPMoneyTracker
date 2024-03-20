
using DLPMoneyTracker.Core.Models.BudgetPlan;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DLPMoneyTracker2.Core.Converters
{
    public class JournalPlanTypeToBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Brushes.White;
            if (value is BudgetPlanType planType)
            {
                switch (planType)
                {
                    case BudgetPlanType.Receivable:
                        return Brushes.Green;

                    case BudgetPlanType.Payable:
                        return Brushes.Red;

                    case BudgetPlanType.Transfer:
                        return Brushes.Blue;

                    default:
                        return Brushes.White;
                }
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}