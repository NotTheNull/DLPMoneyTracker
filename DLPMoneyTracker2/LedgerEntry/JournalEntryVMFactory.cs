using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.LedgerEntry
{
	public static class JournalEntryVMFactory
	{
		public static IJournalEntryVM BuildViewModel(IMoneyTransaction transaction)
		{
			if (transaction.JournalEntryType == null || transaction.JournalEntryType == TransactionType.NotSet) return GetViewModelByLogic(transaction);

			return GetViewModelByType(transaction);
		}

		private static IJournalEntryVM GetViewModelByType(IMoneyTransaction transaction)
		{
			IJournalEntryVM viewModel;
			switch(transaction.JournalEntryType)
			{
				case TransactionType.Expense:
					viewModel = UICore.DependencyHost.GetRequiredService<ExpenseJournalEntryVM>();
					break;
				case TransactionType.Income:
					viewModel = UICore.DependencyHost.GetRequiredService<IncomeJournalEntryVM>();
					break;
				case TransactionType.DebtPayment:
					viewModel = UICore.DependencyHost.GetRequiredService<DebtPaymentJournalEntryVM>();
					break;
				case TransactionType.DebtAdjustment:
					viewModel = UICore.DependencyHost.GetRequiredService<DebtAdjustmentJournalEntryVM>();
					break;
				case TransactionType.Transfer:
					viewModel = UICore.DependencyHost.GetRequiredService<TransferJournalEntryVM>();
					break;
				case TransactionType.Correction:
					viewModel = UICore.DependencyHost.GetRequiredService<CorrectionJournalEntryVM>();
					break;
				default:
					throw new InvalidOperationException(string.Format("Transaction Type [{0}] is not supported", transaction.JournalEntryType));
			}

			viewModel.LoadTransaction(transaction);
			return viewModel;
		}

		private static IJournalEntryVM GetViewModelByLogic(IMoneyTransaction transaction)
		{
			IJournalEntryVM viewModel;

			IJournalAccount debit = transaction.DebitAccount;
			IJournalAccount credit = transaction.CreditAccount;

			if(debit.JournalType == LedgerType.Bank && credit.JournalType == LedgerType.Receivable)
			{
				viewModel = UICore.DependencyHost.GetRequiredService<IncomeJournalEntryVM>();
			}
			else if(debit.JournalType == LedgerType.Payable && credit is IMoneyAccount)
			{
				viewModel = UICore.DependencyHost.GetRequiredService<ExpenseJournalEntryVM>();
			}
			else if(debit is ILiabilityAccount && credit.JournalType == LedgerType.Bank)
			{
				viewModel = UICore.DependencyHost.GetRequiredService<DebtPaymentJournalEntryVM>();
			}
			else if(debit is ILiabilityAccount && (credit.Id == SpecialAccount.DebtInterest.Id || credit.Id == SpecialAccount.DebtReduction.Id))
			{
				viewModel = UICore.DependencyHost.GetRequiredService<DebtAdjustmentJournalEntryVM>();
			}
			else if(debit.Id == SpecialAccount.UnlistedAdjusment.Id || credit.Id == SpecialAccount.UnlistedAdjusment.Id)
			{
				viewModel = UICore.DependencyHost.GetRequiredService<CorrectionJournalEntryVM>();
			}
			else if(debit.JournalType == LedgerType.Bank && credit.JournalType == LedgerType.Bank)
			{
				viewModel = UICore.DependencyHost.GetRequiredService<TransferJournalEntryVM>();
			}
			else
			{
				// This is likely the Initial Balance records which I should probably just edit by hand if they're a problem
				throw new InvalidOperationException("Transaction cannot be modified");
			}

			viewModel.LoadTransaction(transaction);
			viewModel.SaveTransaction();
			return viewModel;
		}

	}
}
