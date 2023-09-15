using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Collections.Generic;
using System.Linq;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class DebtPaymentJournalEntryVM : BaseRecordJournalEntryVM
    {
        public DebtPaymentJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
        {
            _validCreditTypes.Add(JournalAccountType.Bank);
            _validDebitTypes.Add(JournalAccountType.LiabilityCard);
            _validDebitTypes.Add(JournalAccountType.LiabilityLoan);
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

        public override string CreditHeader
        { get { return "Bank"; } }
        public override string DebitHeader
        { get { return "Liability"; } }

        
    }
}