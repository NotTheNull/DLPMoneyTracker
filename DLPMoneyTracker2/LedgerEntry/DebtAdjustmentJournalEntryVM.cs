
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
	public class DebtAdjustmentJournalEntryVM : BaseRecordJournalEntryVM
	{
		public DebtAdjustmentJournalEntryVM(
            IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveTransactionUseCase saveMoneyRecordUseCase) :
            base(
                getAccountsByTypeUseCase,
                getAccountByUIDUseCase,
                saveMoneyRecordUseCase,
                new List<LedgerType>() { LedgerType.LiabilityLoan, LedgerType.LiabilityCard },
				new List<LedgerType>() { LedgerType.NotSet },
				TransactionType.DebtAdjustment)
		{
		}


		public override bool IsValidTransaction
		{
			get
			{
				return this.SelectedCreditAccount != null
					&& this.SelectedDebitAccount != null
					&& !string.IsNullOrWhiteSpace(this.Description)
					&& this.Amount > decimal.Zero;
			}
		}

		public override string DebitHeader { get { return "Debt Account"; } }

		public override string CreditHeader { get { return "Action"; } }

		public override void LoadAccounts()
		{
			base.LoadAccounts();

			this.ValidCreditAccounts.Clear();
			this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(SpecialAccount.DebtInterest.Description, SpecialAccount.DebtInterest));
			this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(SpecialAccount.DebtReduction.Description, SpecialAccount.DebtReduction));
		}

		/// <summary>
		/// NOTE: This View Model swaps things around for UX purposes
		/// i.e. the Credit Card account is in the Credit but needs to be set to the Debit.
		///      Same for the Bank Dates
		/// </summary>
		/// <param name="entry"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public override void LoadTransaction(IMoneyTransaction entry)
		{
			if (entry is null) throw new ArgumentNullException(nameof(IMoneyTransaction));

			IJournalAccount action, liability;
			DateTime? actionDate, liabilityDate;

			var account = entry.CreditAccount;
			var account2 = entry.DebitAccount;
			if(account.JournalType == LedgerType.NotSet)
			{
				action = account;
				actionDate = entry.CreditBankDate;
				liability = account2;
				liabilityDate = entry.DebitBankDate;
			}
			else
			{
				liability = account;
				liabilityDate = entry.CreditBankDate;
				action = account2;
				actionDate = entry.DebitBankDate;
			}

			
			this.ExistingTransactionId = entry.UID;
			this.TransactionDate = entry.TransactionDate;
			this.Amount = entry.TransactionAmount;
			this.Description = entry.Description;
			this.SelectedCreditAccount = action;
			this.SelectedDebitAccount = liability;
			this.CreditBankDate = actionDate;
			this.DebitBankDate = liabilityDate;
		}


		/// <summary>
		/// Which account is Credit and which is debit will be determined by the Action.
		/// </summary>
		public override void SaveTransaction()
		{
			if (!IsValidTransaction) return;

			var liability = this.SelectedDebitAccount;
			var action = this.SelectedCreditAccount;
			bool isSwapAccounts = (action.Id == SpecialAccount.DebtInterest.Id);

			MoneyTransaction record = new MoneyTransaction()
			{
				JournalEntryType = this.JournalEntryType,
				TransactionAmount = this.Amount,
				TransactionDate = this.TransactionDate,
				Description = this.Description,
				CreditAccount = isSwapAccounts ? liability : action,
				CreditBankDate = isSwapAccounts ? this.DebitBankDate : this.CreditBankDate,
				DebitAccount = isSwapAccounts ? action : liability,
				DebitBankDate = isSwapAccounts ? this.CreditBankDate : this.DebitBankDate
			};

			if (this.ExistingTransactionId.HasValue)
			{
				record.UID = this.ExistingTransactionId.Value;
			}

			saveMoneyRecordUseCase.Execute(record);
		}
	}
}