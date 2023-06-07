using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class DebtPaymentJournalEntryVM : BaseRecordJournalEntryVM
    {
        public DebtPaymentJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
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
        
        public override string CreditHeader { get { return "Bank"; } }
        public override string DebitHeader { get { return "Liability"; } }


        public override void LoadAccounts()
        {
            this.ValidCreditAccounts.Clear();
            var listBanks = _config.LedgerAccountsList.Where(x => x.JournalType == JournalAccountType.Bank);
            if(listBanks?.Any() == true)
            {
                foreach(var b in listBanks.OrderBy(o => o.Description))
                {
                    this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(b.Description, b));
                }
            }

            this.ValidDebitAccounts.Clear();
            var listLiability = _config.LedgerAccountsList.Where(x => x.JournalType == JournalAccountType.LiabilityCard || x.JournalType == JournalAccountType.LiabilityLoan);
            if(listLiability?.Any() == true)
            {
                foreach(var l in listLiability.OrderBy(o => o.Description))
                {
                    this.ValidDebitAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(l.Description, l));
                }
            }


        }

    }
}
