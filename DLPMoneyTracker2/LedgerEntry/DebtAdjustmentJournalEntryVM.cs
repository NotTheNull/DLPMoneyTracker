using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DLPMoneyTracker2.LedgerEntry
{
    public class DebtAdjustmentJournalEntryVM : BaseRecordJournalEntryVM
    {
        public DebtAdjustmentJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
        {
            
        }

        public override bool IsValidTransaction 
        {
            get
            {
                return this.SelectedCreditAccount != null
                    && this.SelectedDebitAccount != null
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }


        public override string DebitHeader { get { return "Debt Account"; } }

        public override string CreditHeader { get { return "Action"; } }

        public override void LoadAccounts()
        {
            this.ValidCreditAccounts.Clear();
            this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(SpecialAccount.DebtInterest.Description, SpecialAccount.DebtInterest));
            this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(SpecialAccount.DebtReduction.Description, SpecialAccount.DebtReduction));

            this.ValidDebitAccounts.Clear();
            var listLiability = _config.LedgerAccountsList.Where(x => x.JournalType == JournalAccountType.LiabilityCard || x.JournalType == JournalAccountType.LiabilityLoan);
            if (listLiability?.Any() == true)
            {
                foreach (var l in listLiability.OrderBy(o => o.Description))
                {
                    this.ValidDebitAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(l.Description, l));
                }
            }

        }

        /// <summary>
        /// Which account is Credit and which is debit will be determined by the Action.
        /// </summary>
        public override void SaveTransaction()
        {
            if (!IsValidTransaction) return;

            var liability = this.SelectedDebitAccount;
            var action = this.SelectedCreditAccount;

            JournalEntry record = new JournalEntry(_config)
            {
                TransactionAmount = this.Amount,
                TransactionDate = this.TransactionDate,
                Description = this.Description,
                CreditAccount = (action.Id == SpecialAccount.DebtInterest.Id) ? liability : action,
                DebitAccount = (action.Id == SpecialAccount.DebtInterest.Id) ? action : liability
            };
            _journal.AddTransaction(record);

        }

    }
}
