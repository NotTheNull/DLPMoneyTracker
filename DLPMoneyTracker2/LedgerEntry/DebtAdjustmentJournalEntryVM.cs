using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class DebtAdjustmentJournalEntryVM : BaseRecordJournalEntryVM
    {
        public DebtAdjustmentJournalEntryVM(ITrackerConfig config, IJournal journal) :
            base(journal, config, new List<JournalAccountType>() { JournalAccountType.LiabilityLoan, JournalAccountType.LiabilityCard }, new List<JournalAccountType>())
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

        public override string DebitHeader
        { get { return "Debt Account"; } }

        public override string CreditHeader
        { get { return "Action"; } }

        public override void LoadAccounts()
        {
            base.LoadAccounts();

            this.ValidCreditAccounts.Clear();
            this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(SpecialAccount.DebtInterest.Description, SpecialAccount.DebtInterest));
            this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(SpecialAccount.DebtReduction.Description, SpecialAccount.DebtReduction));
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