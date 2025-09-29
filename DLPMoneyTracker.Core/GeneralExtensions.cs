using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.Core;

public static class GeneralExtensions
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
        if (DateTime.TryParse(date, out DateTime result)) return result;
        return DateTime.MinValue;
    }

    public static int ToInt(this string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return 0;
        if (int.TryParse(word, out int result)) return result;

        return 0;
    }
    public static decimal ToDecimal(this string val)
    {
        if (string.IsNullOrWhiteSpace(val)) return decimal.Zero;

        if (decimal.TryParse(val, out decimal result)) return result;
        return decimal.Zero;
    }

    public static string ToDisplayText(this LedgerType journalType)
    {
        return journalType switch
        {
            LedgerType.Payable => "Accounts Payable",
            LedgerType.LiabilityLoan => "Loan",
            LedgerType.LiabilityCard => "Credit Card",
            LedgerType.Receivable => "Accounts Receivable",
            LedgerType.Bank => "Bank",
            _ => "*N/A*",
        };
    }

    public static string ToDisplayText(this BudgetTrackingType budgetType)
    {
        return budgetType switch
        {
            BudgetTrackingType.DO_NOT_TRACK => "DO NOT TRACK",
            BudgetTrackingType.Fixed => "FIXED",
            BudgetTrackingType.Variable => "VARIABLE",
            _ => string.Empty,
        };
    }

    public static int ToLedgerNumber(this LedgerType journalType)
    {
        return journalType switch
        {
            LedgerType.Bank => 1,
            LedgerType.LiabilityCard => 2,
            LedgerType.LiabilityLoan => 3,
            LedgerType.Receivable => 4,
            LedgerType.Payable => 5,
            LedgerType.NotSet => 6,
            _ => 0,
        };
    }

    public static DLPDataSource ToDataSource(this string arg)
    {
        return arg switch
        {
            "json" => DLPDataSource.JSON,
            "db" => DLPDataSource.Database,
            _ => DLPDataSource.NotSet,
        };
    }
}