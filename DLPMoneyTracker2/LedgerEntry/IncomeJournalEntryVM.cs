using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class IncomeJournalEntryVM : BaseRecordJournalEntryVM
    {
        public IncomeJournalEntryVM(ITrackerConfig config, IJournal journal) : 
            base(
                journal, 
                config, 
                new List<JournalAccountType>() { JournalAccountType.Bank }, 
                new List<JournalAccountType>() { JournalAccountType.Receivable },
                DLPMoneyTracker.Data.TransactionModels.TransactionType.Income)
        {
        }

        public override string DebitHeader
        { get { return "Bank"; } }

        public override string CreditHeader
        { get { return "Receivable"; } }

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedDebitAccount.JournalType == JournalAccountType.Bank
                    && this.SelectedCreditAccount.JournalType == JournalAccountType.Receivable
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }
    }
}