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
        private const string JACCOUNT_NOTSET = "*N/A*";
        private const string JACCOUNT_PAYABLE = "Accounts Payable";
        private const string JACCOUNT_RECEIVE = "Receivable";
        private const string JACCOUNT_BANK = "Bank";
        private const string JACCOUNT_LOAN = "Loan";
        private const string JACCOUNT_CREDITCARD = "Credit Card";

        public static string ToDisplayText(this JournalAccountType journalType)
        {
            switch(journalType)
            {
                case JournalAccountType.Payable:
                    return JACCOUNT_PAYABLE;
                case JournalAccountType.LiabilityLoan:
                    return JACCOUNT_LOAN;
                case JournalAccountType.LiabilityCard:
                    return JACCOUNT_CREDITCARD;
                case JournalAccountType.Receivable:
                    return JACCOUNT_RECEIVE;
                case JournalAccountType.Bank:
                    return JACCOUNT_BANK;
                default:
                    return JACCOUNT_NOTSET;
            }
        }

        public static JournalAccountType ToAccountType(this string actType)
        {
            switch(actType)
            {
                case JACCOUNT_BANK:
                    return JournalAccountType.Bank;
                case JACCOUNT_CREDITCARD:
                    return JournalAccountType.LiabilityCard;
                case JACCOUNT_LOAN:
                    return JournalAccountType.LiabilityLoan;
                case JACCOUNT_PAYABLE:
                    return JournalAccountType.Payable;
                case JACCOUNT_RECEIVE:
                    return JournalAccountType.Receivable;
                default:
                    return JournalAccountType.NotSet;
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
