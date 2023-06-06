using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DLPMoneyTracker2.Core.Converters
{
    public class JournalPlanTypeToBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Brushes.White;
            if (value is JournalPlanType planType)
            {
                switch(planType)
                {
                    case JournalPlanType.Receivable:
                        return Brushes.Green;
                    case JournalPlanType.Payable:
                        return Brushes.Red;
                    case JournalPlanType.Transfer:
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
