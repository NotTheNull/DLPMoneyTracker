using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Collections.Generic;
using System.Linq;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class ExpenseJournalEntryVM : BaseRecordJournalEntryVM
    {
        public ExpenseJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config, new List<JournalAccountType>() { JournalAccountType.Payable }, new List<JournalAccountType>() { JournalAccountType.Bank, JournalAccountType.LiabilityCard})
        {
            
        }

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedDebitAccount.JournalType == JournalAccountType.Payable
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }

        public override string CreditHeader
        { get { return "Money Account"; } }
        public override string DebitHeader
        { get { return "Payable"; } }

    }
}