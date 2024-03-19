
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
	public class DebtPaymentJournalEntryVM : BaseRecordJournalEntryVM
	{
		public DebtPaymentJournalEntryVM(
            IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveTransactionUseCase saveMoneyRecordUseCase) :
            base(
                getAccountsByTypeUseCase,
                getAccountByUIDUseCase,
                saveMoneyRecordUseCase,
                new List<LedgerType>() { LedgerType.LiabilityCard, LedgerType.LiabilityLoan },
				new List<LedgerType>() { LedgerType.Bank },
				TransactionType.DebtPayment)
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