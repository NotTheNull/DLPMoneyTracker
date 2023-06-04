using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.LedgerAccounts;
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
        public static string ToDisplayText(this IJournalAccount account)
        {
            switch(account.JournalType)
            {
                case JounalAccountType.Payable:
                    return "Accounts Payable";
                case JounalAccountType.LiabilityLoan:
                    return "Loan";
                case JounalAccountType.LiabilityCard:
                    return "Credit Card";
                case JounalAccountType.Receivable:
                    return "Accounts Receivable";
                case JounalAccountType.Bank:
                    return "Bank";
                default:
                    return "*N/A*";
            }
        }

        //public static string ToDisplayText(this MoneyAccountType actType)
        //{
        //    switch (actType)
        //    {
        //        case MoneyAccountType.Checking:
        //            return "Checking";

        //        case MoneyAccountType.CreditCard:
        //            return "Credit Card";

        //        case MoneyAccountType.Loan:
        //            return "Loan";

        //        case MoneyAccountType.Savings:
        //            return "Savings";

        //        default:
        //            return "*N/A*";
        //    }
        //}

        //public static string ToDisplayText(this CategoryType catType)
        //{
        //    switch (catType)
        //    {
        //        case CategoryType.Expense:
        //            return "Expense";

        //        case CategoryType.Income:
        //            return "Income";

        //        case CategoryType.UntrackedAdjustment:
        //            return "Adjustment";

        //        default:
        //            return "*N/A*";
        //    }
        //}

        //public static CategoryType ToCategoryType(this string catType)
        //{
        //    switch(catType)
        //    {
        //        case "Expense":
        //            return CategoryType.Expense;
        //        case "Income":
        //            return CategoryType.Income;
        //        case "Adjustment":
        //            return CategoryType.UntrackedAdjustment;
        //        default:
        //            return CategoryType.NotSet;
        //    }
        //}

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
