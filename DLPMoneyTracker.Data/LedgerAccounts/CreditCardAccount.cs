using System;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class CreditCardAccount : IJournalAccount, IMoneyAccount, IDebtAccount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType
        { get { return LedgerType.LiabilityCard; } }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

		public DateTime? PreviousBankReconciliationStatementDate { get; set; }
        

        public decimal MonthlyBudgetAmount
        { get { return decimal.Zero; } }

        public bool ExcludeFromBudget
        { get { return false; } }


		public CreditCardAccount()
        {
            Id = Guid.NewGuid();
        }

        public CreditCardAccount(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }

        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Credit Card Account");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
        }

    }
}