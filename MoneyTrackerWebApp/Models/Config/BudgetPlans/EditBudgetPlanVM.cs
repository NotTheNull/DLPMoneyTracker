using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using System.ComponentModel.DataAnnotations;

namespace MoneyTrackerWebApp.Models.Config.BudgetPlans
{
    public class EditBudgetPlanVM : IBudgetPlan
    {
        public Guid UID { get; set; } = Guid.NewGuid();

        private BudgetPlanType _type = BudgetPlanType.NotSet;
        [Required]
        public BudgetPlanType PlanType 
        {
            get { return _type; } 
            set
            {
                _type = value;
                this.UpdateLedgerTypes();
            }
        } 

        [Required]
        [MaxLength(200)]
        [MinLength(3, ErrorMessage = "Description must have at least 3 characters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.0d, 99999.99d, ErrorMessage = "Please provide a reasonable dollar amount")]
        public decimal ExpectedAmount { get; set; } = decimal.Zero;

        public List<LedgerType> ValidDebitAccountTypes { get; } = new List<LedgerType>();

        [Required]
        public IJournalAccount DebitAccount { get; set; }

        private Guid _selectedDebitId = Guid.Empty;
        public Guid DebitAccountId { get { return DebitAccount?.Id ?? _selectedDebitId; } set { _selectedDebitId = value; } }

        public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }

        public List<LedgerType> ValidCreditAccountTypes { get; } = new List<LedgerType>();

        [Required]
        public IJournalAccount CreditAccount { get; set; }

        private Guid _selectedCreditId = Guid.Empty;
        public Guid CreditAccountId { get { return CreditAccount?.Id ?? _selectedCreditId; } set { _selectedCreditId = value; } }

        public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }

        public IScheduleRecurrence Recurrence { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.Monthly;


        public DateTime NotificationDate { get; }

        public DateTime NextOccurrence { get; }

        public void Copy(IBudgetPlan plan)
        {
            if (plan is null) return;

            this.UID = plan.UID;
            this.Description = plan.Description;
            this.ExpectedAmount = plan.ExpectedAmount;
            this.DebitAccount = plan.DebitAccount;
            this.CreditAccount = plan.CreditAccount;
            this.Recurrence = plan.Recurrence;
            this.StartDate = plan.Recurrence.StartDate;
            this.Frequency = plan.Recurrence.Frequency;
        }

        private void UpdateLedgerTypes()
        {
            IBudgetPlan plan;
            this.ValidCreditAccountTypes.Clear();
            this.ValidDebitAccountTypes.Clear();

            switch (this.PlanType)
            {
                case BudgetPlanType.Receivable:
                    plan = new ReceivablePlan();
                    break;

                case BudgetPlanType.Payable:
                    plan = new PayablePlan();
                    break;

                case BudgetPlanType.Transfer:
                    plan = new TransferPlan();
                    break;

                case BudgetPlanType.DebtPayment:
                    plan = new DebtPaymentPlan();
                    break;

                default:
                    return;
            }

            this.ValidDebitAccountTypes.AddRange(plan.ValidDebitAccountTypes);
            this.ValidCreditAccountTypes.AddRange(plan.ValidCreditAccountTypes);

        }

        public bool IsValid()
        {
            return true;
        }
    }
}
