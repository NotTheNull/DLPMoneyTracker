using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System.Collections.Generic;
using System.Linq;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class CorrectionJournalEntryVM : BaseRecordJournalEntryVM
    {
        public CorrectionJournalEntryVM(ITrackerConfig config, IJournal journal) : base(journal, config)
        {
            this.SelectedCreditAccount = SpecialAccount.UnlistedAdjusment;

            _validDebitTypes.Add(JournalAccountType.Bank);
            _validDebitTypes.Add(JournalAccountType.LiabilityCard);
            _validDebitTypes.Add(JournalAccountType.LiabilityLoan);
        }

        // One side will always be the Special Account "Unlisted Adjustment"
        public override bool IsCreditEnabled
        { get { return false; } }

        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedDebitAccount != null
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount != decimal.Zero;
            }
        }

        public override string CreditHeader
        { get { return string.Empty; } }
        public override string DebitHeader
        { get { return "Account"; } }

        
    }
}