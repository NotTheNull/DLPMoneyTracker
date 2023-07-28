using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
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

        private const string PLANTYPE_PAY = "Payable";
        private const string PLANTYPE_RECIEVE = "Receivable";
        private const string PLANTYPE_DEBT = "Debt Payment";
        private const string PLANTYPE_XFER = "Transfer";
        private const string PLANTYPE_NOTSET = "*N/A*";
        public static string ToDiplayText(this JournalPlanType planType)
        {
            switch(planType)
            {
                case JournalPlanType.Payable:
                    return PLANTYPE_PAY;
                case JournalPlanType.Receivable:
                    return PLANTYPE_RECIEVE;
                case JournalPlanType.DebtPayment:
                    return PLANTYPE_DEBT;
                case JournalPlanType.Transfer:
                    return PLANTYPE_XFER;
                default:
                    return PLANTYPE_NOTSET;
            }
        }

        public static JournalPlanType ToPlanType(this string planType)
        {
            switch(planType)
            {
                case PLANTYPE_PAY:
                    return JournalPlanType.Payable;
                case PLANTYPE_RECIEVE:
                    return JournalPlanType.Receivable;
                case PLANTYPE_DEBT:
                    return JournalPlanType.DebtPayment;
                case PLANTYPE_XFER:
                    return JournalPlanType.Transfer;
                default:
                    return JournalPlanType.NotSet;

            }
        }

        private const string FREQ_YEAR = "Annually";
        private const string FREQ_SEMI = "Semi-Annually";
        private const string FREQ_MONTH = "Monthly";
        private const string FREQ_NOTSET = "*N/A*";
        public static string ToDisplayText(this RecurrenceFrequency freq)
        {
            switch(freq)
            {
                case RecurrenceFrequency.Annual:
                    return FREQ_YEAR;
                case RecurrenceFrequency.SemiAnnual:
                    return FREQ_SEMI;
                case RecurrenceFrequency.Monthly:
                    return FREQ_MONTH;
                default:
                    return FREQ_NOTSET;
            }
        }

        public static RecurrenceFrequency ToRecurrenceFrequency(this string freq)
        {
            switch(freq)
            {
                case FREQ_YEAR:
                    return RecurrenceFrequency.Annual;
                case FREQ_SEMI:
                    return RecurrenceFrequency.SemiAnnual;
                case FREQ_MONTH:
                    return RecurrenceFrequency.Monthly;
                default:
                    return RecurrenceFrequency.NotSet;
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

    }
}
