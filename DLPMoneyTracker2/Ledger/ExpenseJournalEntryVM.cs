using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DLPMoneyTracker2.Ledger
{
    public class ExpenseJournalEntryVM : BaseRecordJournalEntryVM
    {
        public ExpenseJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
        {
            
        }

        private List<JournalAccountType> validCreditTypes = new List<JournalAccountType>()
        {
            JournalAccountType.Bank,
            JournalAccountType.LiabilityCard
        };

        public override bool IsValidTransaction
        {
            get
            {
                return validCreditTypes.Contains(this.SelectedCreditAccount.JournalType)
                    && this.SelectedDebitAccount.JournalType == JournalAccountType.Payable
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }

        public override string CreditHeader { get { return "Money Account"; } }
        public override string DebitHeader { get { return "Payable"; } }



        public override void LoadAccounts()
        {
            this.ValidCreditAccounts.Clear();
            var listCredits = _config.LedgerAccountsList.Where(x => validCreditTypes.Contains(x.JournalType));
            if(listCredits?.Any() == true)
            {
                foreach(var c in listCredits.OrderBy(o => o.Description))
                {
                    this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(c.Description, c));
                }
            }

            this.ValidDebitAccounts.Clear();
            var listDebits = _config.LedgerAccountsList.Where(x => x.JournalType == JournalAccountType.Payable);
            if(listDebits?.Any() == true)
            {
                foreach(var d in listDebits.OrderBy(o => o.Description))
                {
                    this.ValidDebitAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(d.Description, d));
                }
            }
        }
    }
}
