using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class DebtPaymentJournalEntryVM(
        IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
        IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
        ISaveTransactionUseCase saveMoneyRecordUseCase) : 
        BaseRecordJournalEntryVM(
            getAccountsByTypeUseCase,
            getAccountByUIDUseCase,
            saveMoneyRecordUseCase,
            [LedgerType.LiabilityCard, LedgerType.LiabilityLoan],
            [LedgerType.Bank],
            TransactionType.DebtPayment)
    {
        public override string Title => "Enter Debt Payment";

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedCreditAccount?.JournalType == LedgerType.Bank
                    && (this.SelectedDebitAccount?.JournalType == LedgerType.LiabilityCard || this.SelectedDebitAccount?.JournalType == LedgerType.LiabilityLoan)
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }

        public override string CreditHeader => "Bank";

        public override string DebitHeader => "Liability";
    }
}