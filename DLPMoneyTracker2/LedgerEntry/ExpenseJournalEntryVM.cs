using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class ExpenseJournalEntryVM : BaseRecordJournalEntryVM
    {
        public ExpenseJournalEntryVM(ITrackerConfig config, IJournal journal) : 
            base(
                journal, 
                config, 
                new List<LedgerType>() { LedgerType.Payable }, 
                new List<LedgerType>() { LedgerType.Bank, LedgerType.LiabilityCard },
                DLPMoneyTracker.Data.TransactionModels.TransactionType.Expense)
        {
        }

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedDebitAccount.JournalType == LedgerType.Payable
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