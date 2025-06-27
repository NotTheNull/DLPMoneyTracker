
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
	public class IncomeJournalEntryVM(
            IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveTransactionUseCase saveMoneyRecordUseCase) : 
		BaseRecordJournalEntryVM(
            getAccountsByTypeUseCase,
            getAccountByUIDUseCase,
            saveMoneyRecordUseCase,
            [LedgerType.Bank],
			[LedgerType.Receivable],
			TransactionType.Income)
	{
        public override string Title => "Enter Income";
        public override string DebitHeader { get { return "Bank"; } }

		public override string CreditHeader { get { return "Receivable"; } }

		public override bool IsValidTransaction
		{
			get
			{
				return this.SelectedDebitAccount?.JournalType == LedgerType.Bank
					&& this.SelectedCreditAccount?.JournalType == LedgerType.Receivable
					&& !string.IsNullOrWhiteSpace(this.Description)
					&& this.Amount > decimal.Zero;
			}
		}
	}
}