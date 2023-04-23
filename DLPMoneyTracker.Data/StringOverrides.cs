using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data
{

    public static class StringExtensions
    {
        public static string ToDisplayText(this MoneyAccountType actType)
        {
            switch (actType)
            {
                case MoneyAccountType.Checking:
                    return "Checking";

                case MoneyAccountType.CreditCard:
                    return "Credit Card";

                case MoneyAccountType.Loan:
                    return "Loan";

                case MoneyAccountType.Savings:
                    return "Savings";

                default:
                    return "*N/A*";
            }
        }

        public static string ToDisplayText(this CategoryType catType)
        {
            switch (catType)
            {
                case CategoryType.Expense:
                    return "Expense";

                case CategoryType.Income:
                    return "Income";

                case CategoryType.UntrackedAdjustment:
                    return "Adjustment";

                default:
                    return "*N/A*";
            }
        }

        public static string ToDisplayText(this RecurrenceFrequency recurType)
        {
            switch (recurType)
            {
                case RecurrenceFrequency.Annual:
                    return "Annual";

                case RecurrenceFrequency.SemiAnnual:
                    return "Semi-Annual";

                case RecurrenceFrequency.Monthly:
                    return "Monthly";

                default:
                    return "*N/A*";
            }
        }
    }
}
