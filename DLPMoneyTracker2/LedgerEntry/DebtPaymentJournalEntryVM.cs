using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
	public class DebtPaymentJournalEntryVM : BaseRecordJournalEntryVM
	{
		public DebtPaymentJournalEntryVM(ITrackerConfig config, IJournal journal) :
			base(
				journal,
				config,
				new List<LedgerType>() { LedgerType.LiabilityCard, LedgerType.LiabilityLoan },
				new List<LedgerType>() { LedgerType.Bank },
				DLPMoneyTracker.Data.TransactionModels.TransactionType.DebtPayment)
		{
		}

		public override bool IsValidTransaction
		{
			get
			{
				return this.SelectedCreditAccount.JournalType == LedgerType.Bank
					&& (this.SelectedDebitAccount.JournalType == LedgerType.LiabilityCard || this.SelectedDebitAccount.JournalType == LedgerType.LiabilityLoan)
					&& !string.IsNullOrWhiteSpace(this.Description)
					&& this.Amount > decimal.Zero;
			}
		}

		public override string CreditHeader { get { return "Bank"; } }

		public override string DebitHeader { get { return "Liability"; } }
	}
}