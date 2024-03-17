using System;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
	public class ReceivableAccount : IJournalAccount, ILedgerAccount
	{
		public Guid Id { get; set; }

		public string Description { get; set; }

		public LedgerType JournalType
		{ get { return LedgerType.Receivable; } }

		public int OrderBy { get; set; }

		public DateTime? DateClosedUTC { get; set; }

		public decimal MonthlyBudgetAmount { get; set; }

		public bool ExcludeFromBudget { get; set; }

		public ReceivableAccount()
		{
			Id = Guid.NewGuid();
		}

		public ReceivableAccount(IJournalAccount cpy)
		{
			this.Copy(cpy);
		}

		public void Copy(IJournalAccount cpy)
		{
			if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Receivable Account");

			this.Id = cpy.Id;
			this.Description = cpy.Description;
			this.OrderBy = cpy.OrderBy;
			this.DateClosedUTC = cpy.DateClosedUTC;

			if (cpy is ILedgerAccount ledger)
			{
				this.MonthlyBudgetAmount = ledger.MonthlyBudgetAmount;
			}
		}

	}
}