﻿
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core
{
    public static class GeneralOverrides
    {
        public static string RemoveQuotes(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            string data = str.Trim();
            data = data.Replace("\"", "");
            data = data.Replace("\'", "");
            data = data.Replace("`", "");

            return data;
        }

        public static DateTime ToDateTime(this string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return DateTime.MinValue;

            DateTime.TryParse(date, out DateTime result);
            return result;
        }

        public static string ToDisplayText(this DateTime date)
        {
            return string.Format("{0:yyyy/MM/dd}", date);
        }

        public static decimal ToDecimal(this string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return decimal.Zero;

            decimal.TryParse(val, out decimal result);
            return result;
        }

        public static string ToDisplayText(this decimal val)
        {
            return string.Format("{0:c}", val);
        }


        public static string ToDisplayText(this LedgerType journalType)
        {
            switch (journalType)
            {
                case LedgerType.Payable:
                    return "Expense";

                case LedgerType.LiabilityLoan:
                    return "Loan";

                case LedgerType.LiabilityCard:
                    return "Credit Card";

                case LedgerType.Receivable:
                    return "Income";

                case LedgerType.Bank:
                    return "Bank";

                default:
                    return "*N/A*";
            }
        }

        public static string ToDisplayText(this BudgetTrackingType budgetType)
        {
            switch(budgetType)
            {
                case BudgetTrackingType.DO_NOT_TRACK:
                    return "Untracked";
                case BudgetTrackingType.Fixed:
                    return "Fixed";
                case BudgetTrackingType.Variable:
                    return "Variable";
                default:
                    return string.Empty;
            }
        }

        public static string ToDisplayText(this BudgetPlanType planType)
        {
            switch(planType)
            {
                case BudgetPlanType.DebtPayment:
                    return "Debt Payment";
                case BudgetPlanType.Payable:
                    return "Expense";
                case BudgetPlanType.Receivable:
                    return "Income";
                case BudgetPlanType.Transfer:
                    return "Xfer";
                default:
                    return string.Empty;
            }
        }

        public static string ToDisplayText(this RecurrenceFrequency freq)
        {
            switch(freq)
            {
                case RecurrenceFrequency.Annual:
                    return "Annual";
                case RecurrenceFrequency.Monthly:
                    return "Monthly";
                case RecurrenceFrequency.SemiAnnual:
                    return "Semi-Annual";
                default:
                    return string.Empty;
            }
        }

        public static int ToLedgerNumber(this LedgerType journalType)
        {
            switch(journalType)
            {
                case LedgerType.Bank:
                    return 1;
                case LedgerType.LiabilityCard:
                    return 2;
                case LedgerType.LiabilityLoan:
                    return 3;
                case LedgerType.Receivable:
                    return 4;
                case LedgerType.Payable:
                    return 5;
                case LedgerType.NotSet:
                    return 6;
                default:
                    return 0;
            }
        }


        public static DLPDataSource ToDataSource(this string arg)
        {
            switch(arg)
            {
                case "json": return DLPDataSource.JSON;
                case "db": return DLPDataSource.Database;
                default: return DLPDataSource.NotSet;
            }
        }
    }
}
