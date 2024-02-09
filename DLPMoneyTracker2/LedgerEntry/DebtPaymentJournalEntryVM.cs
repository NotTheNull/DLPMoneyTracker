using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class DebtPaymentJournalEntryVM : BaseRecordJournalEntryVM
    {
        public DebtPaymentJournalEntryVM(ITrackerConfig config, IJournal journal) :
            base(
                journal, 
                config, 
                new List<JournalAccountType>() { JournalAccountType.LiabilityCard, JournalAccountType.LiabilityLoan }, 
                new List<JournalAccountType>() { JournalAccountType.Bank }, 
                DLPMoneyTracker.Data.TransactionModels.TransactionType.DebtPayment)
        {
        }

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedCreditAccount.JournalType == JournalAccountType.Bank
                    && (this.SelectedDebitAccount.JournalType == JournalAccountType.LiabilityCard || this.SelectedDebitAccount.JournalType == JournalAccountType.LiabilityLoan)
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }

        public override string CreditHeader
        { get { return "Bank"; } }

        public override string DebitHeader
        { get { return "Liability"; } }
    }
}