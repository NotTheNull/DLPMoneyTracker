using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
	public class CorrectionJournalEntryVM : BaseRecordJournalEntryVM
	{
		public CorrectionJournalEntryVM(
			IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveTransactionUseCase saveMoneyRecordUseCase) : 
			base(
				getAccountsByTypeUseCase, 
				getAccountByUIDUseCase, 
				saveMoneyRecordUseCase,
				new List<LedgerType>() { LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan, LedgerType.Payable, LedgerType.Receivable }, 
				new List<LedgerType>(),
				TransactionType.Correction)
		{
			this.SelectedCreditAccount = SpecialAccount.UnlistedAdjusment;
		}

		// One side will always be the Special Account "Unlisted Adjustment"
		public override bool IsCreditEnabled { get { return false; } }

		public override bool IsValidTransaction
		{
			get
			{
				return this.SelectedDebitAccount != null
					&& !string.IsNullOrWhiteSpace(this.Description)
					&& this.Amount != decimal.Zero;
			}
		}

		public override string CreditHeader { get { return string.Empty; } }

		public override string DebitHeader { get { return "Account"; } }
	}
}