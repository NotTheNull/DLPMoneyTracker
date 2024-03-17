using System;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public enum LedgerType
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
        LedgerType JournalType { get; }
        int OrderBy { get; }
        DateTime? DateClosedUTC { get; set; }


        void Copy(IJournalAccount cpy);
    }

    public interface IMoneyAccount 
    {
		public DateTime? PreviousBankReconciliationStatementDate { get; set; }
	}
    public interface ILedgerAccount 
    {
		decimal MonthlyBudgetAmount { get; } // Exclusive for Variable Expense accounts
		bool ExcludeFromBudget { get; }
	}
    public interface IDebtAccount { }




    public sealed class JournalAccountJSON : IJournalAccount, IMoneyAccount, ILedgerAccount, IDebtAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get; set; }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }
		public DateTime? PreviousBankReconciliationStatementDate { get; set; }

		
		public decimal MonthlyBudgetAmount { get; set; }
        public bool ExcludeFromBudget { get; set; }

        public void Copy(IJournalAccount cpy)
        {
            Id = cpy.Id;
            Description = cpy.Description;
            JournalType = cpy.JournalType;
            OrderBy = cpy.OrderBy;
            DateClosedUTC = cpy.DateClosedUTC;

            if(cpy is IMoneyAccount money)
            {
                PreviousBankReconciliationStatementDate = money.PreviousBankReconciliationStatementDate;
            }
            else if(cpy is ILedgerAccount ledger)
            {
                MonthlyBudgetAmount = ledger.MonthlyBudgetAmount;
                ExcludeFromBudget = ledger.ExcludeFromBudget;
            }
        }
    }
}