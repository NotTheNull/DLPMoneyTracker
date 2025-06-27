using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Plugins.JSON.Adapters;

namespace DLPMoneyTracker.Plugins.JSON.Models
{
    internal sealed class JournalPlanJSON
    {
        public Guid UID { get; set; }

        public BudgetPlanType PlanType { get; set; }

        public string Description { get; set; } = string.Empty;

        public string RecurrenceJSON { get; set; } = string.Empty;

        public Guid DebitAccountId { get; set; }

        public Guid CreditAccountId { get; set; }

        public decimal ExpectedAmount { get; set; }

        public void Copy(IBudgetPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);

            this.UID = plan.UID;
            this.PlanType = plan.PlanType;
            this.Description = plan.Description;
            this.DebitAccountId = plan.DebitAccountId;
            this.CreditAccountId = plan.CreditAccountId;
            this.ExpectedAmount = plan.ExpectedAmount;

            JSONScheduleRecurrenceAdapter adapter = new();
            adapter.Copy(plan.Recurrence);
            this.RecurrenceJSON = adapter.ExportJSON();
        }
    }
}