using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Linq;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class TransferJournalEntryVM : BaseRecordJournalEntryVM
    {
        public TransferJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
        {
            _validDebitTypes.Add(JournalAccountType.Bank);
            _validCreditTypes.Add(JournalAccountType.Bank);
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