using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class TransferJournalEntryVM(
            IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveTransactionUseCase saveMoneyRecordUseCase) :
        BaseRecordJournalEntryVM(
            getAccountsByTypeUseCase,
            getAccountByUIDUseCase,
            saveMoneyRecordUseCase,
            [LedgerType.Bank],
            [LedgerType.Bank],
            TransactionType.Transfer)
    {
        public override string Title => "Enter Transfer";

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedCreditAccount?.JournalType == LedgerType.Bank
                    && this.SelectedDebitAccount?.JournalType == LedgerType.Bank
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }

        public override string CreditHeader => "Xfer From";

        public override string DebitHeader => "Xfer To";
    }
}