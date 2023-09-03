using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Linq;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class TransferJournalEntryVM : BaseRecordJournalEntryVM
    {
        public TransferJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
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

        public override void LoadAccounts()
        {
            this.ValidCreditAccounts.Clear();
            this.ValidDebitAccounts.Clear();
            var listBanks = _config.LedgerAccountsList.Where(x => x.JournalType == JournalAccountType.Bank);
            if (listBanks?.Any() == true)
            {
                foreach (var b in listBanks.OrderBy(o => o.Description))
                {
                    this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(b.Description, b));
                    this.ValidDebitAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(b.Description, b));
                }
            }
        }
    }
}