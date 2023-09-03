using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Linq;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class IncomeJournalEntryVM : BaseRecordJournalEntryVM
    {
        public IncomeJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
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

        public override void LoadAccounts()
        {
            this.ValidDebitAccounts.Clear();
            var listBanks = _config.LedgerAccountsList.Where(x => x.JournalType == JournalAccountType.Bank);
            if (listBanks?.Any() == true)
            {
                foreach (var b in listBanks.OrderBy(o => o.Description))
                {
                    this.ValidDebitAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(b.Description, b));
                }
            }

            this.ValidCreditAccounts.Clear();
            var listReceivables = _config.LedgerAccountsList.Where(x => x.JournalType == JournalAccountType.Receivable);
            if (listReceivables?.Any() == true)
            {
                foreach (var r in listReceivables.OrderBy(o => o.Description))
                {
                    this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(r.Description, r));
                }
            }
        }
    }
}