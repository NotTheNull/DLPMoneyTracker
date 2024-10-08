﻿using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;

namespace DLPMoneyTracker.Data
{
    public static class StringExtensions
    {
        public static string ToDisplayText(this LedgerType journalType)
        {
            switch (journalType)
            {
                case LedgerType.Payable:
                    return "Accounts Payable";

                case LedgerType.LiabilityLoan:
                    return "Loan";

                case LedgerType.LiabilityCard:
                    return "Credit Card";

                case LedgerType.Receivable:
                    return "Accounts Receivable";

                case LedgerType.Bank:
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