
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class ExpenseJournalEntryVM(
        IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
        IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
        ISaveTransactionUseCase saveMoneyRecordUseCase) : 
        BaseRecordJournalEntryVM(
            getAccountsByTypeUseCase,
            getAccountByUIDUseCase,
            saveMoneyRecordUseCase,
            [LedgerType.Payable],
            [LedgerType.Bank, LedgerType.LiabilityCard],
            TransactionType.Expense)
    {
        public override string Title => "Enter Expense";
        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedDebitAccount?.JournalType == LedgerType.Payable
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount != decimal.Zero; // Allow negative values for Cancellations / Returns / Refunds
            }
        }

        public override string CreditHeader => "Money Account";

        public override string DebitHeader => "Payable";
    }
}