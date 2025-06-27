using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;

namespace DLPMoneyTracker.Core.Models.BudgetPlan
{
    public class DebtPaymentPlan : IBudgetPlan
    {
        public Guid UID { get; set; } = Guid.NewGuid();

        public BudgetPlanType PlanType => BudgetPlanType.DebtPayment;

        public string Description { get; set; } = string.Empty;

        public decimal ExpectedAmount { get; set; }

        private readonly List<LedgerType> _validDebits =
        [
            LedgerType.LiabilityCard,
            LedgerType.LiabilityLoan
        ];

        public List<LedgerType> ValidDebitAccountTypes => _validDebits;

        public IJournalAccount DebitAccount { get; set; } = SpecialAccount.InvalidAccount;
        public Guid DebitAccountId => DebitAccount.Id;
        public string DebitAccountName => DebitAccount.Description;

        private readonly List<LedgerType> _validCredits =
        [
            LedgerType.Bank
        ];

        public List<LedgerType> ValidCreditAccountTypes => _validCredits;

        public IJournalAccount CreditAccount { get; set; } = SpecialAccount.InvalidAccount;
        public Guid CreditAccountId => CreditAccount.Id;
        public string CreditAccountName => CreditAccount.Description;

        public IScheduleRecurrence? Recurrence { get; set; }
        public DateTime NotificationDate => Recurrence?.NotificationDate ?? DateTime.MinValue;
        public DateTime NextOccurrence => Recurrence?.NextOccurrence ?? DateTime.MinValue;

        public bool IsValid()
        {
            if (this.DebitAccount is null || this.CreditAccount is null) return false;
            if (!this.ValidDebitAccountTypes.Contains(DebitAccount.JournalType)) return false;
            if (!this.ValidCreditAccountTypes.Contains(CreditAccount.JournalType)) return false;
            if (string.IsNullOrWhiteSpace(this.Description)) return false;
            if (ExpectedAmount <= decimal.Zero) return false;

            return true;
        }

        public void Copy(IBudgetPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            if (plan.PlanType != this.PlanType) throw new InvalidOperationException("Plan types do not match");

            this.UID = plan.UID;
            this.CreditAccount = plan.CreditAccount;
            this.DebitAccount = plan.DebitAccount;
            this.Description = plan.Description;
            this.Recurrence = plan.Recurrence;
            this.ExpectedAmount = plan.ExpectedAmount;
        }
    }
}