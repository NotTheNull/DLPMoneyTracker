using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

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
                [LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan, LedgerType.Payable, LedgerType.Receivable],
                [],
                TransactionType.Correction)
        {
            this.SelectedCreditAccount = SpecialAccount.UnlistedAdjustment;
        }

        public override string Title => "Enter Journal Correction";

        // One side will always be the Special Account "Unlisted Adjustment"
        public override bool IsCreditEnabled => false; 

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedDebitAccount != null
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount != decimal.Zero;
            }
        }

        public override string CreditHeader => string.Empty;

        public override string DebitHeader => "Account";
    }
}