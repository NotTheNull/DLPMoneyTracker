using System;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class BankAccount : IJournalAccount, IMoneyAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public JournalAccountType JournalType
        { get { return JournalAccountType.Bank; } }
        public int OrderBy { get; set; }
        public DateTime? DateClosedUTC { get; set; }


        public decimal MonthlyBudgetAmount
        { get { return decimal.Zero; } }

        public bool ExcludeFromBudget
        { get { return false; } }

		public DateTime? PreviousBankReconciliationStatementDate { get; set; }

		public BankAccount()
        {
            Id = Guid.NewGuid();
        }

        public BankAccount(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Bank Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
        }

    }
}