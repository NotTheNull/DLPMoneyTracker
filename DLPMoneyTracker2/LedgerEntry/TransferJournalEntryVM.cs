
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
	public class TransferJournalEntryVM : BaseRecordJournalEntryVM
	{
		public TransferJournalEntryVM(
            IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveTransactionUseCase saveMoneyRecordUseCase) :
            base(
                getAccountsByTypeUseCase,
                getAccountByUIDUseCase,
                saveMoneyRecordUseCase,
                new List<LedgerType>() { LedgerType.Bank },
				new List<LedgerType>() { LedgerType.Bank },
				TransactionType.Transfer)
		{
		}

        public override string Title => "Enter Transfer";
        public override bool IsValidTransaction
		{
			get
			{
				return this.SelectedCreditAccount.JournalType == LedgerType.Bank
					&& this.SelectedDebitAccount.JournalType == LedgerType.Bank
					&& !string.IsNullOrWhiteSpace(this.Description)
					&& this.Amount > decimal.Zero;
			}
		}

		public override string CreditHeader { get { return "Xfer From"; } }

		public override string DebitHeader { get { return "Xfer To"; } }
	}
}