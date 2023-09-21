using System;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public enum JournalAccountType
    {
        Bank,
        LiabilityCard,
        LiabilityLoan,
        Receivable,
        Payable,
        NotSet
    }

    public interface IJournalAccount
    {
        Guid Id { get; }
        string Description { get; }
        JournalAccountType JournalType { get; }
        int OrderBy { get; }
        DateTime? DateClosedUTC { get; set; }

        //string MoneyAccountId { get; }
        //MoneyAccountType AccountType { get; }
        //Guid CategoryId { get; } // Reference to legacy TransactionCategory
        decimal MonthlyBudgetAmount { get; } // Exclusive for Variable Expense accounts

        bool ExcludeFromBudget { get; }

        void Copy(IJournalAccount cpy);
    }

    public sealed class JournalAccountJSON : IJournalAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public JournalAccountType JournalType { get; set; }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        //public string MoneyAccountId { get; set; }

        //public MoneyAccountType AccountType { get; set; }

        //public Guid CategoryId { get; set; }

        public decimal MonthlyBudgetAmount { get; set; }
        public bool ExcludeFromBudget { get; set; }

        public void Copy(IJournalAccount cpy)
        {
            Id = cpy.Id;
            Description = cpy.Description;
            JournalType = cpy.JournalType;
            OrderBy = cpy.OrderBy;
            DateClosedUTC = cpy.DateClosedUTC;
            //MoneyAccountId = cpy.MoneyAccountId;
            //AccountType = cpy.AccountType;
            //CategoryId = cpy.CategoryId;
            MonthlyBudgetAmount = cpy.MonthlyBudgetAmount;
        }
    }
}