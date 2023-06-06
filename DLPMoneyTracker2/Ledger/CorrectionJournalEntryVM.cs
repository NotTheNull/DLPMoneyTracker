using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Ledger
{
    public class CorrectionJournalEntryVM : BaseRecordJournalEntryVM
    {

        public CorrectionJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
        {
            this.SelectedCreditAccount = SpecialAccount.UnlistedAdjusment;    
        }

        // One side will always be the Special Account "Unlisted Adjustment"
        public override bool IsCreditEnabled { get { return false; } }

        public override bool IsValidTransaction 
        { 
            get
            {
                return this.SelectedDebitAccount != null
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount != decimal.Zero;
            }
        }

        public override string CreditHeader { get { return string.Empty; } }
        public override string DebitHeader { get { return "Account"; } }

        public override void LoadAccounts()
        {
            this.ValidDebitAccounts.Clear();
            var listAccounts = _config.LedgerAccountsList.Where(x => x.JournalType != JournalAccountType.NotSet);
            if(listAccounts?.Any() == true)
            {
                foreach(var a in listAccounts.OrderBy(o => o.Description))
                {
                    this.ValidDebitAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(a.Description, a));
                }
            }
        }
    }
}
