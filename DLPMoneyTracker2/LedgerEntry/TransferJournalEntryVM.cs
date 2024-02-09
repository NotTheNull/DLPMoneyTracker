using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class TransferJournalEntryVM : BaseRecordJournalEntryVM
    {
        public TransferJournalEntryVM(ITrackerConfig config, IJournal journal) : 
            base(
                journal, 
                config, 
                new List<JournalAccountType>() { JournalAccountType.Bank }, 
                new List<JournalAccountType>() { JournalAccountType.Bank },
                DLPMoneyTracker.Data.TransactionModels.TransactionType.Transfer)
        {
        }

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedCreditAccount.JournalType == JournalAccountType.Bank
                    && this.SelectedDebitAccount.JournalType == JournalAccountType.Bank
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }

        public override string CreditHeader
        { get { return "Xfer From"; } }

        public override string DebitHeader
        { get { return "Xfer To"; } }
    }
}