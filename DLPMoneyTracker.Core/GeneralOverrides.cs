
using DLPMoneyTracker.Core.Models.LedgerAccounts;
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

        public static decimal ToDecimal(this string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return decimal.Zero;

            decimal.TryParse(val, out decimal result);
            return result;
        }


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

        public static string ToDisplayText(this BudgetTrackingType budgetType)
        {
            switch(budgetType)
            {
                case BudgetTrackingType.DO_NOT_TRACK:
                    return "DO NOT TRACK";
                case BudgetTrackingType.Fixed:
                    return "FIXED";
                case BudgetTrackingType.Variable:
                    return "VARIABLE";
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
