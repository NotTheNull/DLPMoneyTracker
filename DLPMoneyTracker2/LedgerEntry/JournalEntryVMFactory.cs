using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DLPMoneyTracker2.LedgerEntry
{
    public static class JournalEntryVMFactory
    {
        public static IJournalEntryVM BuildViewModel(IMoneyTransaction transaction)
        {
            if (transaction.JournalEntryType == TransactionType.NotSet) return GetViewModelByLogic(transaction);

            return GetViewModelByType(transaction);
        }

        private static IJournalEntryVM GetViewModelByType(IMoneyTransaction transaction)
        {
            IJournalEntryVM viewModel = transaction.JournalEntryType switch
            {
                TransactionType.Expense => UICore.DependencyHost.GetRequiredService<ExpenseJournalEntryVM>(),
                TransactionType.Income => UICore.DependencyHost.GetRequiredService<IncomeJournalEntryVM>(),
                TransactionType.DebtPayment => UICore.DependencyHost.GetRequiredService<DebtPaymentJournalEntryVM>(),
                TransactionType.DebtAdjustment => UICore.DependencyHost.GetRequiredService<DebtAdjustmentJournalEntryVM>(),
                TransactionType.Transfer => UICore.DependencyHost.GetRequiredService<TransferJournalEntryVM>(),
                TransactionType.Correction => UICore.DependencyHost.GetRequiredService<CorrectionJournalEntryVM>(),
                _ => throw new InvalidOperationException(string.Format("Transaction Type [{0}] is not supported", transaction.JournalEntryType)),
            };
            viewModel.LoadTransaction(transaction);
            return viewModel;
        }

        private static IJournalEntryVM GetViewModelByLogic(IMoneyTransaction transaction)
        {
            IJournalEntryVM viewModel;

            IJournalAccount debit = transaction.DebitAccount;
            IJournalAccount credit = transaction.CreditAccount;

            if (debit.JournalType == LedgerType.Bank && credit.JournalType == LedgerType.Receivable)
            {
                viewModel = UICore.DependencyHost.GetRequiredService<IncomeJournalEntryVM>();
            }
            else if (debit.JournalType == LedgerType.Payable && credit is IMoneyAccount)
            {
                viewModel = UICore.DependencyHost.GetRequiredService<ExpenseJournalEntryVM>();
            }
            else if (debit is ILiabilityAccount && credit.JournalType == LedgerType.Bank)
            {
                viewModel = UICore.DependencyHost.GetRequiredService<DebtPaymentJournalEntryVM>();
            }
            else if (debit is ILiabilityAccount && (credit.Id == SpecialAccount.DebtInterest.Id || credit.Id == SpecialAccount.DebtReduction.Id))
            {
                viewModel = UICore.DependencyHost.GetRequiredService<DebtAdjustmentJournalEntryVM>();
            }
            else if (debit.Id == SpecialAccount.UnlistedAdjustment.Id || credit.Id == SpecialAccount.UnlistedAdjustment.Id)
            {
                viewModel = UICore.DependencyHost.GetRequiredService<CorrectionJournalEntryVM>();
            }
            else if (debit.JournalType == LedgerType.Bank && credit.JournalType == LedgerType.Bank)
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